using APIFORD.Model.CarroClasses.Intermedians;

namespace APIFORD.Model;

public class Modo
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty; // Ex: Sport, Eco, Comfort

    // Relacionamento Muitos-para-Muitos com Carro
    public virtual ICollection<ModoFonte> ModoFontes { get; set; } = new List<ModoFonte>();
    public ICollection<CarroModo> CarroModos { get; set; } = new List<CarroModo>();
}
