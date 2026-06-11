using System.Text.Json.Serialization;

namespace APIFORD.Data.DTOS;

public class PropriedadeScrapingDTO<T>
{
    public double Confianca { get; set; }
    public bool Conflito { get; set; }

    [JsonPropertyName("Fontes")] // É ISTO QUE RESOLVE O SEU ERRO!
    public List<ItemFonteScrapingDTO<T>> HistoricoFontes { get; set; } = new();
}
// O ITEM INTERNO: É o seu antigo ValorScrapingDto evoluído, contendo os dados individuais de cada fonte
public class ItemFonteScrapingDTO<T>
{
    
    public T Valor { get; set; } = default!;

    
    public double Confianca { get; set; }

    [JsonPropertyName("Fonte")]
    public string NomeFontePython { get; set; } = string.Empty; // Recebe o "icarros.com.br"

    // Estes campos ficam vazios no POST, mas serão preenchidos pelo C# antes de gravar
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int FonteId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTime DataColeta { get; set; }

    public DateTime? DataReferencia { get; set; }
}