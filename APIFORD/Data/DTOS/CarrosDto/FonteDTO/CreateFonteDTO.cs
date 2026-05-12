namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public record CreateFonteDTO(
    string Nome,
    decimal Confiabilidade,
    DateTime DataAdicao
);