using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.Annotations;

/// <summary>Request — cria uma anotação vazia.</summary>
public class CriarAnotacaoDTO
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(200, ErrorMessage = "O título deve ter no máximo 200 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "O subtítulo deve ter no máximo 200 caracteres.")]
    public string? Subtitulo { get; set; }
}