namespace APIFORD.Data;

using APIFORD.Model;
using APIFORD.Model.CarroClasses;
using APIFORD.Model.CarroClasses.Intermedians;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class FordDbContext : IdentityDbContext<User>
{
    public FordDbContext(DbContextOptions<FordDbContext> options) : base(options)
    {
    }

    // --- Tabelas de Notificações ---
    public DbSet<Notificacao> Notifications { get; set; }

    // --- Tabelas Principais ---
    public DbSet<Carro> Carros { get; set; }
    public DbSet<Fonte> Fontes { get; set; }
    public DbSet<Modo> Modos { get; set; }
    public DbSet<ModeloSalvo> ModeloSalvos { get; set; } // Adicionado para Favoritos 

    // --- Tabelas Satélites (Dados Técnicos) ---
    public DbSet<Especificacao> Especificacoes { get; set; }
    public DbSet<Consumo> Consumos { get; set; }
    public DbSet<Dimensao> Dimensoes { get; set; }
    public DbSet<Pneu> Pneus { get; set; }
    public DbSet<Extra> Extras { get; set; }
    // --- Tabelas de Ligação (Muitos-para-Muitos / Intermediárias) ---
    // Expor esses DbSets evita erros de resolução em tempo de design (Migrations)
    public DbSet<CarroModo> CarroModos { get; set; }
    public DbSet<ExtraFonte> ExtraFontes { get; set; }
    public DbSet<ConsumoFonte> ConsumoFontes { get; set; }
    public DbSet<DimensaoFonte> DimensaoFontes { get; set; }
    public DbSet<ModoFonte> ModoFontes { get; set; }
    public DbSet<EspecificacaoFonte> EspecificacaoFontes { get; set; }
    public DbSet<PneuFonte> PneuFontes { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // 1. Configuração de Notificações
        builder.Entity<Notificacao>(entity =>
        {
        entity.Property(e => e.DataCriacao)
              .HasDefaultValueSql("GETUTCDATE()");
        });

        // Configuração de Favoritos (Usuario <-> Carro) [cite: 1922, 1925]
        builder.Entity<ModeloSalvo>(entity =>
        {
            entity.HasKey(ms => new { ms.UserId, ms.CarroId });
            entity.HasOne(ms => ms.User).WithMany(u => u.ModelosSalvos).HasForeignKey(ms => ms.UserId);
            entity.HasOne(ms => ms.Carro).WithMany(c => c.SalvoUsuario).HasForeignKey(ms => ms.CarroId);
        });

        // 2. Configuração Principal do Carro e Relacionamentos 1:N
        // O uso de HasMany garante flexibilidade para múltiplas fontes no futuro
        builder.Entity<Carro>(entity =>
        {
            entity.HasMany(c => c.Especificacaos).WithOne(e => e.Carro).HasForeignKey(e => e.CarroId).OnDelete(DeleteBehavior.Cascade); 
            entity.HasMany(c => c.Consumos).WithOne().HasForeignKey(co => co.CarroId).OnDelete(DeleteBehavior.Cascade); 
            entity.HasMany(c => c.Dimensoes).WithOne().HasForeignKey(d => d.CarroId).OnDelete(DeleteBehavior.Cascade); 
            entity.HasMany(c => c.Pneus).WithOne().HasForeignKey(p => p.CarroId).OnDelete(DeleteBehavior.Cascade); 
            entity.HasMany(c => c.Extras).WithOne().HasForeignKey(a => a.CarroId).OnDelete(DeleteBehavior.Cascade); 
        });

        //// 3. Configuração de Tabelas Satélites Individuais (Para integridade em PATCH)
        //builder.Entity<Specification>(entity => { entity.HasKey(e => e.Id); });
        //builder.Entity<Consume>(entity => { entity.HasKey(c => c.Id); });
        //builder.Entity<Tire>(entity => { entity.HasKey(t => t.Id); });

        // Configuração da Tabela Intermediária (Muitos-para-Muitos)
        builder.Entity<CarroModo>(entity =>
        {
            entity.HasKey(cm => new { cm.CarroId, cm.ModoId });
            entity.HasOne(cm => cm.Carro).WithMany(c => c.ModosCarro).HasForeignKey(cm => cm.CarroId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(cm => cm.Modo).WithMany(m => m.CarroModos).HasForeignKey(cm => cm.ModoId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<ExtraFonte>(entity =>
        {
            entity.HasKey(ans => new { ans.ExtraId, ans.FonteId });
            entity.HasOne(ans => ans.Extra).WithMany(a => a.ExtraFontes).HasForeignKey(ans => ans.ExtraId).OnDelete(DeleteBehavior.Cascade); 
            entity.HasOne(ans => ans.Fonte).WithMany(s => s.ExtraFontes).HasForeignKey(ans => ans.FonteId).OnDelete(DeleteBehavior.Restrict); 
        });
        builder.Entity<ConsumoFonte>(entity =>
        {
            entity.HasKey(cs => new { cs.ConsumoId, cs.FonteId });
            entity.HasOne(cs => cs.Consumo).WithMany(c => c.ConsumoFontes).HasForeignKey(cs => cs.ConsumoId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(cs => cs.Fonte).WithMany(s => s.ConsumoFontes).HasForeignKey(cs => cs.FonteId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<DimensaoFonte>(entity =>
        {
            entity.HasKey(ds => new { ds.DimensaoId, ds.FonteId });
            entity.HasOne(ds => ds.Dimensao).WithMany(d => d.DimensaoFontes).HasForeignKey(ds => ds.DimensaoId).OnDelete(DeleteBehavior.Cascade); 
            entity.HasOne(ds => ds.Fonte).WithMany(s => s.DimensaoFontes).HasForeignKey(ds => ds.FonteId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<ModoFonte>(entity =>
        {
            entity.HasKey(ms => new { ms.ModoId, ms.FonteId});
            entity.HasOne(ms => ms.Modo).WithMany(m => m.ModoFontes).HasForeignKey(ms => ms.ModoId).OnDelete(DeleteBehavior.Cascade); 
            entity.HasOne(ms => ms.Fonte).WithMany(s => s.ModoFontes).HasForeignKey(ms => ms.FonteId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<EspecificacaoFonte>(entity =>
        {
            entity.HasKey(ss => new { ss.EspecificacaoId, ss.FonteId });
            entity.HasOne(ss => ss.Especificacao).WithMany(s => s.EspecificacaoFontes).HasForeignKey(ss => ss.EspecificacaoId).OnDelete(DeleteBehavior.Cascade); 
            entity.HasOne(ss => ss.Fonte).WithMany(s => s.EspecificacaoFontes).HasForeignKey(ss => ss.FonteId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<PneuFonte>(entity =>
        {
            entity.HasKey(ts => new { ts.PneuId, ts.FonteId });
            entity.HasOne(ts => ts.Pneu).WithMany(t => t.PneuFontes).HasForeignKey(ts => ts.PneuId).OnDelete(DeleteBehavior.Cascade); 
            entity.HasOne(ts => ts.Fonte).WithMany(s => s.PneuFontes).HasForeignKey(ts => ts.FonteId).OnDelete(DeleteBehavior.Restrict);
        });

    }
}