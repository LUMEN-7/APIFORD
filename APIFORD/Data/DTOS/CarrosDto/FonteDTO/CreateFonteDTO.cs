namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public record CreateFonteDTO(
    string Nome,
    string Url, // Adicionado
    decimal Confiabilidade,
    DateTime DataAdicao
);