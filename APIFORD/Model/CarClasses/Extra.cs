using APIFORD.Model.CarroClasses.Intermedians;

namespace APIFORD.Model.CarroClasses;

public class Extra
{
    public int Id { get; set; }
    public int CarroId { get; set; }
    public string CapacidadeTanque { get; set; } = string.Empty;
    public string TipoCombustivel { get; set; } = string.Empty;
    public string CapacidadeCarga { get; set; } = string.Empty;
    public string CapacidadeReboque { get; set; } = string.Empty;
    public bool Excluido { get; set; } = false;


    public virtual ICollection<ExtraFonte> ExtraFontes { get; set; } = new List<ExtraFonte>();
}
