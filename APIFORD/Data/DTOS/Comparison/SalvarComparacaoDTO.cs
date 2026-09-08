using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.Comparison;

/// <summary>Request — salva o "blueprint" de uma comparação (o payload da pesquisa, não o resultado).</summary>
public class SalvarComparacaoDTO
{
    [StringLength(200, ErrorMessage = "O título deve ter no máximo 200 caracteres.")]
    public string? Titulo { get; set; } // opcional — fallback "Comparação sem título" já existe no Service

    [Required(ErrorMessage = "O tipo da comparação é obrigatório.")]
    public string Tipo { get; set; } = string.Empty; // ex: "Direta" ou "Bulk"

    [Required(ErrorMessage = "O payload da requisição é obrigatório.")]
    public string RequestPayload { get; set; } = string.Empty; // O front-end envia o JSON do request como string
}