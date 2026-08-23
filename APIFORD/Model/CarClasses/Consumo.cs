

namespace APIFORD.Model.CarroClasses;

public class Consumo
{

    public DateTime DataColeta { get; set; } = DateTime.UtcNow;

    public PropriedadeScraping<decimal> Cidade { get; set; } = new();
    public PropriedadeScraping<decimal> Estrada { get; set; } = new();

}
