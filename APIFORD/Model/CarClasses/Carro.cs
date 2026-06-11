using APIFORD.Model.CarroClasses.Intermedians;
using static System.Net.Mime.MediaTypeNames;

namespace APIFORD.Model.CarroClasses;

public class Carro
{
    public int Id { get; set; }
    public string Modelo { get; set; }
    public string Marca { get; set; }
    public int Ano { get; set; }
    public bool Excluido { get; set; } = false;

    public virtual ICollection<ModeloSalvo> SalvoUsuario { get; set; } = new List<ModeloSalvo>();
    public virtual ICollection<Especificacao> Especificacoes { get; set; } = new List<Especificacao>();
    public virtual ICollection<Consumo> Consumos { get; set; } = new List<Consumo>();
    public virtual ICollection<Dimensao> Dimensoes { get; set; } = new List<Dimensao>();
    public virtual ICollection<Pneu> Pneus { get; set; } = new List<Pneu>();
    public virtual ICollection<Extra> Extras { get; set; } = new List<Extra>();
    public virtual ICollection<CarroModo> ModosCarro { get; set; } = new List<CarroModo>();
}
