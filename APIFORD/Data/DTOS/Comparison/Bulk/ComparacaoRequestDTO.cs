namespace APIFORD.Data.DTOS.Comparison.Bulk;

public class ComparacaoRequestDTO
{
    public int CarroBaseId { get; set; }
    public string? Categoria { get; set; }

    // Agora o usuário envia uma lista de regras matemáticas!
    public List<FiltroComparacaoDTO> Filtros { get; set; } = new();
}