using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.Comparison.Bulk;

/// <summary>Request — compara um carro base contra todos os que atendem a uma lista de filtros.</summary>
public class ComparacaoRequestDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "CarroBaseId inválido.")]
    public int CarroBaseId { get; set; }

    public string? Categoria { get; set; }

    // Agora o usuário envia uma lista de regras matemáticas! Cada item aqui é validado
    // automaticamente pelo model binder (o ASP.NET Core percorre listas de complex types).
    public List<FiltroComparacaoDTO> Filtros { get; set; } = new();
}