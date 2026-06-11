using APIFORD.Model.CarroClasses.Intermedians;

namespace APIFORD.Model.CarroClasses;

public class Dimensao
{
    public int Id { get; set; }
    public int CarroId { get; set; }
    public bool Excluido { get; set; } = false;
    public DateTime DataColeta { get; set; } = DateTime.UtcNow;

    public PropriedadeScraping<decimal> Comprimento { get; set; } = new();
    public PropriedadeScraping<decimal> Largura { get; set; } = new();
    public PropriedadeScraping<decimal> Altura { get; set; } = new();
    public PropriedadeScraping<decimal> EntreEixos { get; set; } = new();

    public virtual Carro Carro { get; set; } = null!;
}