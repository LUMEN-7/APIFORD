using System.Text.Json.Serialization;

namespace APIFORD.Data.DTOS;

public class PropriedadeScrapingDTO<T>
{
    [JsonPropertyName("Confianca")]
    public double Confianca { get; set; }

    [JsonPropertyName("Conflito")]
    public bool Conflito { get; set; }

    [JsonPropertyName("Fontes")]
    public List<ItemFonteScrapingDTO<T>> Fontes { get; set; } = new();
}

public class ItemFonteScrapingDTO<T>
{
    [JsonPropertyName("Valor")]
    public T Valor { get; set; } = default!;

    [JsonPropertyName("Confianca")]
    public double Confianca { get; set; }

    [JsonPropertyName("Fonte")] // resumo da historia essa merda serva pra merda nenhuma
    public string Fonte { get; set; } = string.Empty;

    // Coloque JsonIgnore simples para o C# focar apenas naquilo que vem do Python
    [JsonIgnore]
    public int FonteId { get; set; }
    [JsonIgnore]
    public DateTime DataColeta { get; set; }
    [JsonIgnore]
    public DateTime? DataReferencia { get; set; }
}