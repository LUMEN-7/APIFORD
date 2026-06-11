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

    public DbSet<Notificacao> Notifications { get; set; }
    public DbSet<Carro> Carros { get; set; }
    public DbSet<Fonte> Fontes { get; set; }
    public DbSet<Modo> Modos { get; set; }
    public DbSet<ModeloSalvo> ModeloSalvos { get; set; }
    public DbSet<Especificacao> Especificacoes { get; set; }
    public DbSet<Consumo> Consumos { get; set; }
    public DbSet<Dimensao> Dimensoes { get; set; }
    public DbSet<Pneu> Pneus { get; set; }
    public DbSet<Extra> Extras { get; set; }
    public DbSet<CarroModo> CarroModos { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Notificacao>(entity =>
        {

            entity.Property(e => e.DataCriacao).HasDefaultValueSql("GETUTCDATE()");
        });

        builder.Entity<ModeloSalvo>(entity =>
        {
            entity.HasKey(ms => new { ms.UserId, ms.CarroId });
            entity.HasOne(ms => ms.User).WithMany(u => u.ModelosSalvos).HasForeignKey(ms => ms.UserId);
            entity.HasOne(ms => ms.Carro).WithMany(c => c.SalvoUsuario).HasForeignKey(ms => ms.CarroId);
        });

        builder.Entity<Fonte>(entity =>
        {
        });

        builder.Entity<Modo>(entity =>
        {
        });

        builder.Entity<CarroModo>(entity =>
        {
            entity.HasKey(cm => new { cm.CarroId, cm.ModoId });
            entity.HasOne(cm => cm.Carro).WithMany(c => c.ModosCarro).HasForeignKey(cm => cm.CarroId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(cm => cm.Modo).WithMany(m => m.CarroModos).HasForeignKey(cm => cm.ModoId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Especificacao>(entity =>
        {
            entity.OwnsOne(e => e.Potencia, b => { b.ToJson(); b.OwnsMany(x => x.Fontes); });
            entity.OwnsOne(e => e.Torque, b => { b.ToJson(); b.OwnsMany(x => x.Fontes); });
            entity.OwnsOne(e => e.PotenciaRpm, b => { b.ToJson(); b.OwnsMany(x => x.Fontes); });
            entity.OwnsOne(e => e.TorqueRpm, b => { b.ToJson(); b.OwnsMany(x => x.Fontes); });
            entity.OwnsOne(e => e.Transmissao, b => { b.ToJson(); b.OwnsMany(x => x.Fontes); });
            entity.OwnsOne(e => e.Tracao, b => { b.ToJson(); b.OwnsMany(x => x.Fontes); });
        });

        builder.Entity<Consumo>(entity =>
        {
            entity.OwnsOne(c => c.Cidade, b => { b.ToJson(); b.OwnsMany(x => x.Fontes); });
            entity.OwnsOne(c => c.Estrada, b => { b.ToJson(); b.OwnsMany(x => x.Fontes); });
        });

        builder.Entity<Dimensao>(entity =>
        {
            entity.OwnsOne(d => d.Comprimento, b => { b.ToJson(); b.OwnsMany(x => x.Fontes); });
            entity.OwnsOne(d => d.Largura, b => { b.ToJson(); b.OwnsMany(x => x.Fontes); });
            entity.OwnsOne(d => d.Altura, b => { b.ToJson(); b.OwnsMany(x => x.Fontes); });
            entity.OwnsOne(d => d.EntreEixos, b => { b.ToJson(); b.OwnsMany(x => x.Fontes); });
        });

        builder.Entity<Pneu>(entity =>
        {
            entity.OwnsOne(p => p.Tipo, b => { b.ToJson(); b.OwnsMany(x => x.Fontes); });
            entity.OwnsOne(p => p.Aro, b => { b.ToJson(); b.OwnsMany(x => x.Fontes); });
            entity.OwnsOne(p => p.Largura, b => { b.ToJson(); b.OwnsMany(x => x.Fontes); });
            entity.OwnsOne(p => p.Perfil, b => { b.ToJson(); b.OwnsMany(x => x.Fontes); });
        });

        builder.Entity<Extra>(entity =>
        {
            entity.OwnsOne(e => e.CapacidadeTanque, b => { b.ToJson(); b.OwnsMany(x => x.Fontes); });
            entity.OwnsOne(e => e.TipoCombustivel, b => { b.ToJson(); b.OwnsMany(x => x.Fontes); });
            entity.OwnsOne(e => e.CapacidadeCarga, b => { b.ToJson(); b.OwnsMany(x => x.Fontes); });
            entity.OwnsOne(e => e.CapacidadeReboque, b => { b.ToJson(); b.OwnsMany(x => x.Fontes); });
        });
    }
}