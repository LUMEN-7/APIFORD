using APIFORD.Model.User;

namespace APIFORD.Model.CarroClasses;

public class Carro
{
    public int Id { get; set; }
    public string Modelo { get; set; }
    public string Marca { get; set; }
    public int Ano { get; set; }
    public bool Excluido { get; set; } = false;
    public int LinhagemId { get; set; }
    public int? VersaoAnteriorId { get; set; }
    public string ImagemUrl { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public  ICollection<ModeloSalvo> SalvoUsuario { get; set; } = new List<ModeloSalvo>();
    public  ICollection<Especificacao> Especificacoes { get; set; } = new List<Especificacao>();
    public  ICollection<Consumo> Consumos { get; set; } = new List<Consumo>();
    public  ICollection<Dimensao> Dimensoes { get; set; } = new List<Dimensao>();
    public  ICollection<Pneu> Pneus { get; set; } = new List<Pneu>();
    public  ICollection<Extra> Extras { get; set; } = new List<Extra>();
    public PropriedadeScraping<List<string>> Modos { get; set; }
    public PropriedadeScraping<string> Categoria { get; set; }
}
