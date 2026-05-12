namespace APIFORD.Model.CarroClasses.Intermedians;

public class PneuFonte
{
    public int PneuId { get; set; }
    public virtual Pneu Pneu { get; set; } = null!;

    public int FonteId { get; set; }
    public virtual Fonte Fonte { get; set; } = null!;

    // Quando o robô Python buscou a informação desse pneu
    public DateTime DataColeta { get; set; } = DateTime.UtcNow;

    // Quando o site original publicou ou atualizou a informação desse pneu
    public DateTime? DataReferencia { get; set; }
}
