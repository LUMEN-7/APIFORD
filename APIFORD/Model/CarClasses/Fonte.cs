
namespace APIFORD.Model.CarroClasses;

public class Fonte
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty; // Ex: "Carros na Web"
    public string Url { get; set; } = string.Empty;  // Ex: "https://www.carrosnaweb.com.br/..."
    public decimal Confiabilidade { get; set; }
    public DateTime DataAdicao { get; set; } = DateTime.UtcNow;
    public bool Excluido { get; set; } = false;
}
