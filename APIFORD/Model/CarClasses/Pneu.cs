using APIFORD.Model.CarroClasses.Intermedians;

namespace APIFORD.Model.CarroClasses;

public class Pneu
{
    public int Id { get; set; }
    public int CarroId { get; set; }
    public string Tipo { get; set; }
    public int Aro { get; set; }
    public int Largura { get; set; }
    public int Perfil { get; set; }
    public bool Excluido { get; set; } = false;


    public virtual ICollection<PneuFonte> PneuFontes { get; set; } = new List<PneuFonte>();
}
