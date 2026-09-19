
namespace APIFORD.Model.CarroClasses;

public class Extra
{
    

    public DateTime DataColeta { get; set; } = DateTime.UtcNow;

    public PropriedadeScraping<decimal> CapacidadeTanque { get; set; } = new();
    public PropriedadeScraping<string> TipoCombustivel { get; set; } = new();
    public PropriedadeScraping<decimal> CapacidadeCarga { get; set; } = new();
    public PropriedadeScraping<decimal> CapacidadeReboque { get; set; } = new();

    public PropriedadeScraping<string> Performace { get; set; } = new();
    public PropriedadeScraping<string> Segurança { get; set; } = new();
    public PropriedadeScraping<string> Conforto { get; set; } = new();
    public PropriedadeScraping<string> Tecnoligas { get; set; } = new();


}
