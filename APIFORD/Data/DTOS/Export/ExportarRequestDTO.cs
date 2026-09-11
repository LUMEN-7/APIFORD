using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.Export;

/// <summary>Request — dispara a geração de um arquivo de exportação.</summary>
public class ExportarRequestDTO : IValidatableObject
{
    [Required(ErrorMessage = "Informe ao menos um item para exportar.")]
    [MinLength(1, ErrorMessage = "Informe ao menos um item para exportar.")]
    public List<ItemExportacaoDTO> Itens { get; set; } = new();

    [RegularExpression("^(csv|xlsx|json|xml)$", ErrorMessage = "Formato inválido. Use csv, xlsx, json ou xml.")]
    public string Formato { get; set; } = "csv";

    [RegularExpression("^[,;]$", ErrorMessage = "Separador inválido. Use ',' ou ';'.")]
    public string Separador { get; set; } = ","; // "," (padrão) ou ";" (Excel pt-BR)

    public ModoResolucaoFonte Modo { get; set; } = ModoResolucaoFonte.Automatico;

    public List<EscolhaManualDTO>? Escolhas { get; set; } // só usado no modo Manual

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Modo == ModoResolucaoFonte.Manual && (Escolhas == null || Escolhas.Count == 0))
            yield return new ValidationResult("Informe as escolhas de fonte quando o modo é Manual.", new[] { nameof(Escolhas) });
    }
}

public enum ModoResolucaoFonte { Automatico, Manual }