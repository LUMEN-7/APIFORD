
namespace APIFORD.Model.CarroClasses;

public class Extra
{
    

    public DateTime DataColeta { get; set; } = DateTime.UtcNow;

    public PropriedadeScraping<double> CapacidadeTanque { get; set; } = new();
    public PropriedadeScraping<string> TipoCombustivel { get; set; } = new();
    public PropriedadeScraping<double> CapacidadeCarga { get; set; } = new();
    public PropriedadeScraping<double> CapacidadeReboque { get; set; } = new();

    
}
