using APIFORD.Model.CarroClasses.Intermedians;

namespace APIFORD.Model.CarroClasses;

public class Consumo
{
    public int Id { get; set; }
    public int CarroId { get; set; }
    public bool Excluido { get; set; } = false;
    public DateTime DataColeta { get; set; } = DateTime.UtcNow;

    public PropriedadeScraping<double> Cidade { get; set; } = new();
    public PropriedadeScraping<double> Estrada { get; set; } = new();

    public virtual Carro Carro { get; set; } = null!;
}
