using APIFORD.Data;
using APIFORD.Data.DTOS;
using APIFORD.Hubs;
using APIFORD.Model.User;
using APIFORD.Perfils;
using APIFORD.Services;
using APIFORD.Services.Schedule;
using APIFORD.Services.Search;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IO.Compression;
using System.Text;
using System.Text.Json.Serialization;





WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. VARIÁVEIS DE AMBIENTE / APPSETTINGS
// ==========================================
var securityKey = builder.Configuration["SymmetricSecurityKey"];
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


// ==========================================
// 2. BANCO DE DADOS E IDENTITY
// ==========================================
//builder.Services.AddDbContext<FordDbContext>(opts =>
//    opts.UseSqlServer(connectionString));
builder.Services.AddDbContext<FordDbContext>(opts =>
    opts.UseNpgsql(connectionString,
        npgsqlOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5, // Tenta reconectar até 5 vezes antes de desistir
                maxRetryDelay: TimeSpan.FromSeconds(30), // Espera até 30 segundos entre as tentativas
                errorCodesToAdd: null); // Usa a lista padrão de erros transitórios do SQL Server
        })
    );


builder.Services
    .AddIdentity<User, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<FordDbContext>()
    .AddDefaultTokenProviders();

// ==========================================
// 3. SEGURANÇA (AUTENTICAÇÃO E AUTORIZAÇÃO)
// ==========================================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
    {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey)),
        ValidateAudience = false,
        ValidateIssuer = false,
        ClockSkew = TimeSpan.Zero,
    };

    // O handshake WebSocket do SignalR não manda header Authorization,
    // então o front precisa mandar o token assim: /hubs/notificacao?access_token=SEU_TOKEN
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            if (!string.IsNullOrEmpty(accessToken) &&
                context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };

 });

// Politicas de autorização customizadas, caso queira usar. Exemplo de política de idade minima, onde o requisito é ter mais de 18 anos para acessar determinado recurso.
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("IdadeMinima", policy => policy.AddRequirements(new IdadeMinima(18)));
//});
//builder.Services.AddSingleton<IAuthorizationHandler, IdadeAuthorization>();


// ==========================================
// 4. INJEÇÃO DE DEPENDÊNCIA (SEUS SERVIÇOS)
// ==========================================
builder.Services.AddHttpClient();
builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes
        .InNamespaces("APIFORD.Services")
        .Where(type => type.Name.EndsWith("Service") && !typeof(IHostedService).IsAssignableFrom(type))
    )
    .AsSelf()
    .AsImplementedInterfaces()
    .WithScopedLifetime()
);

builder.Services.AddHostedService<BuscaJobWatcherService>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddHostedService<AgendamentoWorker>();


// ==========================================
// 5. CONFIGURAÇÕES DA API E FERRAMENTAS
// ==========================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddAutoMapper(config => {
    config.AddProfile<UserPerfil>();
}, typeof(Program).Assembly);
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "APIFORD",
        Version = "v1",
        Description = "API Gateway / BFF para orquestração de serviços e raspagem de dados."
    });

    //// Código para Carroregar os comentários XML do Controller
    //var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    //c.IncludeXmlComments(xmlPath);
});




builder.Services.AddDbContext<FordDbContext>(options =>
    options.UseNpgsql(connectionString)
           .UseSnakeCaseNamingConvention());

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true; // por padrão vem desabilitado em HTTPS, precisa ligar
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
});

builder.Services.Configure<BrotliCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);
builder.Services.Configure<GzipCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);
builder.Services.AddSignalR();
builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontEnd", policy =>
        policy.WithOrigins("https://beyond-compare.vercel.app") // porta real do front em dev — ajusta se for outra
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ==========================================
// PIPELINE DE REQUISIÇÃO (MIDDLEWARES)
// ==========================================
WebApplication app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var armazenamento = scope.ServiceProvider.GetRequiredService<IArmazenamentoService>();
    await armazenamento.GarantirBucketExisteAsync();

    // seed, uma vez (Program.cs no startup, ou um endpoint temporário de admin)
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var admin = await userManager.FindByEmailAsync("geronimoaugustonsantos@gmail.com");
    if (admin != null && !await userManager.IsInRoleAsync(admin, "Admin"))
        await userManager.AddToRoleAsync(admin, "Admin");

}

app.UseHttpsRedirection();
app.UseCors("FrontEnd");
app.UseResponseCompression();

app.UseSwagger();
app.UseSwaggerUI();


app.MapHub<NotificacaoHub>("/hubs/notificacao");

app.UseAuthentication();
app.UseAuthorization();


app.UseMiddleware<APIFORD.Middleware.ExceptionMiddleware>();
app.MapControllers();

app.Run();