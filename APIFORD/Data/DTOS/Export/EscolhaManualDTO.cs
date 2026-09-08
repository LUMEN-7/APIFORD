using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.Export;

/// <summary>Request — escolha manual de fonte para um campo específico (modo Manual).</summary>
public class EscolhaManualDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "LinhagemId inválido.")]
    public int LinhagemId { get; set; }

    [Required(ErrorMessage = "O campo é obrigatório.")]
    public string CampoCompleto { get; set; } = string.Empty; // ex: "Especificacoes.Potencia"

    [Required(ErrorMessage = "A fonte escolhida é obrigatória.")]
    public string FonteEscolhida { get; set; } = string.Empty;
}