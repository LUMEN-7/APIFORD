using APIFORD.Model.CarroClasses.Intermedians;

namespace APIFORD.Model.CarroClasses;

public class Dimensao
{
    public int Id { get; set; }
    public int CarroId { get; set; }
    public decimal Comprimento { get; set; }
    public decimal Largura { get; set; }
    public decimal Altura { get; set; }
    public decimal EntreEixos { get; set; }
    public bool Excluido { get; set; } = false;

    public virtual ICollection<DimensaoFonte> DimensaoFontes { get; set; } = new List<DimensaoFonte>();

}
