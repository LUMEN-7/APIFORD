using APIFORD.Model.CarroClasses.Intermedians;

namespace APIFORD.Model.CarroClasses;

public class Consumo
{
    public int Id { get; set; }
    public int CarroId { get; set; }
    public string Cidade { get; set; } 
    public string Estrada { get; set; }
    public bool Excluido { get; set; } = false;


    public virtual ICollection<ConsumoFonte> ConsumoFontes { get; set; } = new List<ConsumoFonte>();
}
