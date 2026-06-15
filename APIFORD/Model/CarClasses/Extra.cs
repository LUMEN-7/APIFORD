using APIFORD.Model.CarroClasses.Intermedians;

namespace APIFORD.Model.CarroClasses;

public class Extra
{
    public int Id { get; set; }
    public int CarroId { get; set; }
    public bool Excluido { get; set; } = false;
    public DateTime DataColeta { get; set; } = DateTime.UtcNow;

    public PropriedadeScraping<double> CapacidadeTanque { get; set; } = new();
    public PropriedadeScraping<string> TipoCombustivel { get; set; } = new();
    public PropriedadeScraping<double> CapacidadeCarga { get; set; } = new();
    public PropriedadeScraping<double> CapacidadeReboque { get; set; } = new();

    public virtual Carro Carro { get; set; } = null!;
}
