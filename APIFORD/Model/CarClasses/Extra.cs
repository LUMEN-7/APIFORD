
namespace APIFORD.Model.CarroClasses;

public class Extra
{
    

    public DateTime DataColeta { get; set; } = DateTime.UtcNow;

    public PropriedadeScraping<decimal> CapacidadeTanque { get; set; } = new();
    public PropriedadeScraping<string> TipoCombustivel { get; set; } = new();
    public PropriedadeScraping<decimal> CapacidadeCarga { get; set; } = new();
    public PropriedadeScraping<decimal> CapacidadeReboque { get; set; } = new();

    public PropriedadeScraping<string> Performance { get; set; } = new();
    public PropriedadeScraping<string> Seguranca { get; set; } = new();
    public PropriedadeScraping<string> Conforto { get; set; } = new();
    public PropriedadeScraping<string> Tecnologia { get; set; } = new();


}
