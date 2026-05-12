using APIFORD.Model.CarroClasses.Intermedians;

namespace APIFORD.Model.CarroClasses;

public class Especificacao
{
    public int Id { get; set; }
    public int CarroId { get; set; }
    public int Potencia { get; set; } = 0;
    public int Torque { get; set; } = 0;
    public int RpmPotencia { get; set; } = 0;
    public int RpmTorque { get; set; } = 0;
    public string Transmissao { get; set; } = string.Empty;
    public string Tracao { get; set; } = string.Empty;
    public virtual Carro Carro { get; set; } = null!;
    public bool Excluido { get; set; } = false;

    public virtual ICollection<EspecificacaoFonte> EspecificacaoFontes { get; set; } = new List<EspecificacaoFonte>();

}
