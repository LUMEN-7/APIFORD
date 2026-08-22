namespace APIFORD.Data.DTOS.Comparison.Bulk;

public class ComparacaoRequestDTO
{
    public int CarroBaseId { get; set; }

    // Ex: ["Potencia", "CapacidadePortaMalas", "Preco"]
    public List<string> Criterios { get; set; } = new List<string>();

    // Ex: 10 (Busca carros com valores 10% para cima ou para baixo do carro base)
    public decimal ToleranciaPercentual { get; set; } = 10;

    // Opcional para refinar a busca
    public string? Categoria { get; set; }
}
