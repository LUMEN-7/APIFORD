namespace APIFORD.Model.CarroClasses.Intermedians;

public class ModoFonte
{
    public int ModoId { get; set; }
    public virtual Modo Modo { get; set; } = null!;

    public int FonteId { get; set; }
    public virtual Fonte Fonte { get; set; } = null!;

    public virtual ICollection<ModoFonte> FontesModo { get; set; } = new List<ModoFonte>();

    // Quando o robô Python buscou a informação desse pneu
    public DateTime DataColeta { get; set; } = DateTime.UtcNow;

    // Quando o site original publicou ou atualizou a informação desse pneu
    public DateTime? DataReferencia { get; set; }
}
