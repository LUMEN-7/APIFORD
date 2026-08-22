namespace APIFORD.Model.CarroClasses;

public class Dimensao
{


    public DateTime DataColeta { get; set; } = DateTime.UtcNow;

    public PropriedadeScraping<decimal> Comprimento { get; set; } = new();
    public PropriedadeScraping<decimal> Largura { get; set; } = new();
    public PropriedadeScraping<decimal> Altura { get; set; } = new();
    public PropriedadeScraping<decimal> EntreEixos { get; set; } = new();

}