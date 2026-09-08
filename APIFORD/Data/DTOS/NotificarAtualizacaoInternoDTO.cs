using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS;

public class NotificarAtualizacaoInternoDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "LinhagemId inválido.")]
    public int LinhagemId { get; set; }

    [Required(ErrorMessage = "A marca é obrigatória.")]
    public string Marca { get; set; } = string.Empty;

    [Required(ErrorMessage = "O modelo é obrigatório.")]
    public string Modelo { get; set; } = string.Empty;
}