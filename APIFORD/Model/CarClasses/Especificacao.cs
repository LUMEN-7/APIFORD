using APIFORD.Model.CarroClasses.Intermedians;

namespace APIFORD.Model.CarroClasses;

public class Especificacao
{
    public int Id { get; set; }
    public int CarroId { get; set; }
    public bool Excluido { get; set; } = false;
    public DateTime DataColeta { get; set; } = DateTime.UtcNow;

    public PropriedadeScraping<int> Potencia { get; set; } = new();
    public PropriedadeScraping<int> Torque { get; set; } = new();
    public PropriedadeScraping<int> PotenciaRpm { get; set; } = new();
    public PropriedadeScraping<int> TorqueRpm { get; set; } = new();
    public PropriedadeScraping<string> Transmissao { get; set; } = new();
    public PropriedadeScraping<string> Tracao { get; set; } = new();

    public virtual Carro Carro { get; set; } = null!;
}
