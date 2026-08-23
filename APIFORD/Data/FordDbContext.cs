namespace APIFORD.Data;

using APIFORD.Model;
using APIFORD.Model.CarroClasses;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class FordDbContext : IdentityDbContext<User>
{
    public FordDbContext(DbContextOptions<FordDbContext> options) : base(options)
    {
    }

    // ==========================================
    // APENAS AS TABELAS FÍSICAS REAIS
    // ==========================================
    public DbSet<Notificacao> Notifications { get; set; }
    public DbSet<Carro> Carros { get; set; }
    public DbSet<Fonte> Fontes { get; set; }
    public DbSet<ModeloSalvo> ModeloSalvos { get; set; }
    public DbSet<Job> Jobs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ==========================================
        // TABELA WORKER: JOBS
        // ==========================================
        builder.Entity<Job>(b =>
        {
            b.ToTable("jobs");
            b.HasKey(x => x.Id).HasName("pk_jobs");

            // Assumindo que Job.Id virou int, removemos o gerador UUID
            b.Property(x => x.Id).HasColumnName("id");

            b.Property(x => x.Status).HasColumnName("status").HasDefaultValue("pending");

            // Forçamos o Postgres a entender essas strings como jsonb também
            b.Property(x => x.Payload).HasColumnName("payload").HasColumnType("jsonb");
            b.Property(x => x.Result).HasColumnName("result").HasColumnType("jsonb");
            b.Property(x => x.Error).HasColumnName("error");

            b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // ==========================================
        // TABELAS AUXILIARES E RELACIONAMENTOS
        // ==========================================
        builder.Entity<Notificacao>(entity =>
        {
            entity.Property(e => e.DataCriacao).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        builder.Entity<ModeloSalvo>(entity =>
        {
            entity.HasKey(ms => new { ms.UserId, ms.CarroId });
            entity.HasOne(ms => ms.User).WithMany(u => u.ModelosSalvos).HasForeignKey(ms => ms.UserId);
            entity.HasOne(ms => ms.Carro).WithMany(c => c.SalvoUsuario).HasForeignKey(ms => ms.CarroId);
        });

        builder.Entity<Fonte>(entity => { });

        // ==========================================
        // 🔄 MÁGICA DO POSTGRES: MAPEAMENTO JSONB
        // ==========================================
        builder.Entity<Carro>(entity =>
        {
            entity.HasIndex(c => c.Especificacoes).HasMethod("gin");
            entity.HasIndex(c => c.Consumos).HasMethod("gin");
            entity.HasIndex(c => c.Dimensoes).HasMethod("gin");
            entity.HasIndex(c => c.Extras).HasMethod("gin");

            // Se você for usar 'Categoria' e 'Modos' frequentemente em filtros:
            entity.HasIndex(c => c.Categoria).HasMethod("gin");
            entity.HasIndex(c => c.Modos).HasMethod("gin");
            // === NOSSOS NOVOS ENVELOPES ===

            // Categoria (string)
            entity.OwnsOne(c => c.Categoria, cat => {
                cat.ToJson();
                cat.OwnsMany(x => x.Fontes);
            });

            // ModosCarro (List<string>)
            entity.OwnsOne(c => c.Modos, mod => {
                mod.ToJson();
                mod.OwnsMany(x => x.Fontes);
            });

            // === O RESTANTE DA ESTRUTURA ===

            entity.OwnsMany(c => c.Especificacoes, specs =>
            {
                specs.ToJson();
                specs.OwnsOne(e => e.Potencia, b => b.OwnsMany(x => x.Fontes));
                specs.OwnsOne(e => e.Torque, b => b.OwnsMany(x => x.Fontes));
                specs.OwnsOne(e => e.PotenciaRpm, b => b.OwnsMany(x => x.Fontes));
                specs.OwnsOne(e => e.TorqueRpm, b => b.OwnsMany(x => x.Fontes));
                specs.OwnsOne(e => e.Transmissao, b => b.OwnsMany(x => x.Fontes));
                specs.OwnsOne(e => e.Tracao, b => b.OwnsMany(x => x.Fontes));
            });

            entity.OwnsMany(c => c.Consumos, consumos =>
            {
                consumos.ToJson();
                consumos.OwnsOne(c => c.Cidade, b => b.OwnsMany(x => x.Fontes));
                consumos.OwnsOne(c => c.Estrada, b => b.OwnsMany(x => x.Fontes));
            });

            entity.OwnsMany(c => c.Dimensoes, dimensoes =>
            {
                dimensoes.ToJson();
                dimensoes.OwnsOne(d => d.Comprimento, b => b.OwnsMany(x => x.Fontes));
                dimensoes.OwnsOne(d => d.Largura, b => b.OwnsMany(x => x.Fontes));
                dimensoes.OwnsOne(d => d.Altura, b => b.OwnsMany(x => x.Fontes));
                dimensoes.OwnsOne(d => d.EntreEixos, b => b.OwnsMany(x => x.Fontes));
            });

            entity.OwnsMany(c => c.Pneus, pneus =>
            {
                pneus.ToJson();
                pneus.OwnsOne(p => p.Tipo, b => b.OwnsMany(x => x.Fontes));
                pneus.OwnsOne(p => p.Aro, b => b.OwnsMany(x => x.Fontes));
                pneus.OwnsOne(p => p.Largura, b => b.OwnsMany(x => x.Fontes));
                pneus.OwnsOne(p => p.Perfil, b => b.OwnsMany(x => x.Fontes));
            });

            entity.OwnsMany(c => c.Extras, extras =>
            {
                extras.ToJson();
                extras.OwnsOne(e => e.CapacidadeTanque, b => b.OwnsMany(x => x.Fontes));
                extras.OwnsOne(e => e.TipoCombustivel, b => b.OwnsMany(x => x.Fontes));
                extras.OwnsOne(e => e.CapacidadeCarga, b => b.OwnsMany(x => x.Fontes));
                extras.OwnsOne(e => e.CapacidadeReboque, b => b.OwnsMany(x => x.Fontes));
            });
        });
    }
}