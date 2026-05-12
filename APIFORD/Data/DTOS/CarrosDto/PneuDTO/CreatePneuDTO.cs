namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public record CreatePneuDTO(
    string Tipo, 
    int Aro, 
    int Largura, 
    int Perfil,
    DateTime? DataReferencia
    );