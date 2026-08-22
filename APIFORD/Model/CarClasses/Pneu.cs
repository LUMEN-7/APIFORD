

namespace APIFORD.Model.CarroClasses;

public class Pneu
{

    public DateTime DataColeta { get; set; } = DateTime.UtcNow;

    public PropriedadeScraping<string> Tipo { get; set; } = new();
    public PropriedadeScraping<int> Aro { get; set; } = new();
    public PropriedadeScraping<int> Largura { get; set; } = new();
    public PropriedadeScraping<int> Perfil { get; set; } = new();
}
