namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public record CreateEspecificacaoDTO(
    string Potencia,
    string Torque, 
    string RpmPotencia,
    string RpmTorque, 
    string Transmissao, 
    string Tracao,
    DateTime? DataReferencia
);