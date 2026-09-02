namespace APIFORD.Data;

using APIFORD.Model;
using APIFORD.Model.CarroClasses;
using APIFORD.Model.Notification;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

public class FordDbContext : IdentityDbContext<User>
{
    public FordDbContext(DbContextOptions<FordDbContext> options) : base(options)
    {
    }

    // ==========================================
    // APENAS AS TABELAS FÍSICAS REAIS
    // ==========================================
    public DbSet<Carro> Carros { get; set; }
    public DbSet<Fonte> Fontes { get; set; }
    public DbSet<ModeloSalvo> ModeloSalvos { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<ComparacaoSalva> ComparacoesSalvas { get; set; }
    public DbSet<NotificacaoEvento> NotificacoesEventos { get; set; }
    public DbSet<NotificacaoUsuario> NotificacoesUsuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ==========================================
        // TABELA NOTIFICACOES
        // ==========================================
        builder.Entity<NotificacaoEvento>(entity =>
        {});

        builder.Entity<NotificacaoUsuario>(entity =>
        {
            entity.HasOne(e => e.Evento)
                .WithMany(ev => ev.Destinatarios)
                .HasForeignKey(e => e.NotificacaoEventoId)
                .OnDelete(DeleteBehavior.Cascade);

            // query mais frequente do sistema: notificações não lidas de 1 usuário
            entity.HasIndex(e => new { e.UserId, e.Lida })
                .HasDatabaseName("ix_notificacoes_usuarios_user_lida");
        });



        // ==========================================
        // TABELA WORKER: JOBS
        // ==========================================
        builder.Entity<Job>(b =>
        {
            b.ToTable("jobs");
            b.HasKey(x => x.Id).HasName("pk_jobs");

            // Job.Id é Guid (não int) — o Postgres precisa gerar o UUID.
            b.Property(x => x.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

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
        // =========================================

        builder.Entity<ModeloSalvo>(entity =>
        {
            entity.HasKey(ms => new { ms.UserId, ms.CarroId });
            entity.HasOne(ms => ms.User).WithMany(u => u.ModelosSalvos).HasForeignKey(ms => ms.UserId);
            entity.HasOne(ms => ms.Carro).WithMany(c => c.SalvoUsuario).HasForeignKey(ms => ms.CarroId);
        });

        builder.Entity<Fonte>(entity => { });


        builder.Entity<ComparacaoSalva>(entity =>
        {

            entity.HasKey(c => c.Id);
            // 3. Mapeamento do Usuário (Com Índice para buscas rápidas)
            entity.Property(c => c.UserId)
                .IsRequired()
                .HasMaxLength(450); // Tamanho padrão seguro caso use o ASP.NET Core Identity no futuro

            entity.HasIndex(c => c.UserId); // Essencial para a listagem do histórico não ficar lenta!

            // 4. Mapeamento dos Dados Básicos
            entity.Property(c => c.Titulo)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(c => c.Tipo)
                .IsRequired()
                .HasMaxLength(50); // Vai guardar apenas "Direta" ou "Grupo"

            entity.Property(c => c.DataSalvamento)
                .IsRequired();

            // 5. A MÁGICA: Avisando ao PostgreSQL que essa string é um JSONB
            entity.Property(c => c.RequestPayload)
                .IsRequired()
                .HasColumnType("jsonb");
        });

        // ==========================================
        // 🔄 MÁGICA DO POSTGRES: MAPEAMENTO JSONB
        // ==========================================
        builder.Entity<Carro>(entity =>
        {

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