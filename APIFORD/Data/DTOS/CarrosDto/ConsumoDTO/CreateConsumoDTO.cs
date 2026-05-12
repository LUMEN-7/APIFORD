namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public record CreateConsumoDTO(
    string Cidade, 
    string Estrada,
    DateTime? DataReferencia
);