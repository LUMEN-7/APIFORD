using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APIFORD.Data.DTOS;

public class PropriedadeScrapingDTO<T>
{
    [JsonPropertyName("Confianca")]
    [Range(0.0, 1.0, ErrorMessage = "Confiança deve estar entre 0 e 1.")]
    public double Confianca { get; set; }

    [JsonPropertyName("Conflito")]
    public bool Conflito { get; set; }

    [JsonPropertyName("Fontes")]
    [MinLength(1, ErrorMessage = "Informe ao menos uma fonte.")]
    public List<ItemFonteScrapingDTO<T>> Fontes { get; set; } = new();
}

public class ItemFonteScrapingDTO<T>
{
    [JsonPropertyName("Valor")]
    [Required(ErrorMessage = "O valor é obrigatório.")]
    public T Valor { get; set; } = default!;

    [JsonPropertyName("Confianca")]
    [Range(0.0, 1.0, ErrorMessage = "Confiança deve estar entre 0 e 1.")]
    public double Confianca { get; set; }

    [JsonPropertyName("Fonte")] // resumo da historia essa merda serva pra merda nenhuma
    public string Fonte { get; set; } = string.Empty;

    [JsonIgnore]
    public int FonteId { get; set; }
    [JsonIgnore]
    public DateTime DataColeta { get; set; }
    [JsonIgnore]
    public DateTime? DataReferencia { get; set; }
}