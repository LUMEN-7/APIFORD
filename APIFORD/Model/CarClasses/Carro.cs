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


    // Relacionamentos 1:N (Permitem flexibilidade para múltiplas fontes)
    public ICollection<ModeloSalvo> SalvoUsuario { get; set; } = new List<ModeloSalvo>();
    public ICollection<Especificacao> Especificacaos { get; set; } = new List<Especificacao>();
    public ICollection<Consumo> Consumos { get; set; } = new List<Consumo>();
    public ICollection<Dimensao> Dimensoes { get; set; } = new List<Dimensao>();
    public ICollection<Pneu> Pneus { get; set; } = new List<Pneu>();
    public ICollection<Extra> Extras { get; set; } = new List<Extra>();
    public ICollection<CarroModo> ModosCarro { get; set; } = new List<CarroModo>();
}
