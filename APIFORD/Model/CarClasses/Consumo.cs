

namespace APIFORD.Model.CarroClasses;

public class Consumo
{

    public DateTime DataColeta { get; set; } = DateTime.UtcNow;

    public PropriedadeScraping<double> Cidade { get; set; } = new();
    public PropriedadeScraping<double> Estrada { get; set; } = new();

}
