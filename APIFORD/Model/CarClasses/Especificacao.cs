

namespace APIFORD.Model.CarroClasses;

public class Especificacao
{

    public DateTime DataColeta { get; set; } = DateTime.UtcNow;

    public PropriedadeScraping<int> Potencia { get; set; } = new();
    public PropriedadeScraping<int> Torque { get; set; } = new();
    public PropriedadeScraping<int> PotenciaRpm { get; set; } = new();
    public PropriedadeScraping<int> TorqueRpm { get; set; } = new();
    public PropriedadeScraping<string> Transmissao { get; set; } = new();
    public PropriedadeScraping<string> Tracao { get; set; } = new();

}
