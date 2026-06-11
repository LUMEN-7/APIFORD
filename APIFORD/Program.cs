using APIFORD.Data;
using APIFORD.Model;
using APIFORD.Perfils;
using APIFORD.Services;
using APIFORD.Services.Search;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
builder.Services.AddDbContext<FordDbContext>(opts =>
    opts.UseSqlServer(connectionString));

builder.Services
    .AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<FordDbContext>()
    .AddDefaultTokenProviders();

// ==========================================
// 3. SEGURANÇA (AUTENTICAÇÃO E AUTORIZAÇÃO)
// ==========================================
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//}).AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey)),
//        ValidateAudience = false,
//        ValidateIssuer = false,
//        ClockSkew = TimeSpan.Zero,
//    };
//});

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
        .Where(type => type.Name.EndsWith("Service")) 
    )
    .AsSelf() // Regista como
    .AsImplementedInterfaces() // Regista como IBaseService<...>
    .WithScopedLifetime() // Define o tempo de vida como Scoped
);
// Adicione esta linha no seu Program.cs


// ==========================================
// 5. CONFIGURAÇÕES DA API E FERRAMENTAS
// ==========================================
builder.Services.AddControllers().AddNewtonsoftJson();
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
    options.UseSqlServer(connectionString)
           .UseSnakeCaseNamingConvention());

// ==========================================
// PIPELINE DE REQUISIÇÃO (MIDDLEWARES)
// ==========================================
WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.UseMiddleware<APIFORD.Middleware.ExceptionMiddleware>();
app.MapControllers();

app.Run();