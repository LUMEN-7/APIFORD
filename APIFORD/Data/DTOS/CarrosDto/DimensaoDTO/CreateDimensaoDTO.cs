namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public record CreateDimensaoDTO(
    decimal Length, 
    decimal Largura, 
    decimal Altura, 
    decimal EntreEixos,
    DateTime? DataReferencia
    );