using APIFORD.Model.CarroClasses.Intermedians;

namespace APIFORD.Model.CarroClasses;

public class Pneu
{
    public int Id { get; set; }
    public int CarroId { get; set; }
    public bool Excluido { get; set; } = false;
    public DateTime DataColeta { get; set; } = DateTime.UtcNow;

    public PropriedadeScraping<string> Tipo { get; set; } = new();
    public PropriedadeScraping<int> Aro { get; set; } = new();
    public PropriedadeScraping<int> Largura { get; set; } = new();
    public PropriedadeScraping<int> Perfil { get; set; } = new();

    public virtual Carro Carro { get; set; } = null!;
}
