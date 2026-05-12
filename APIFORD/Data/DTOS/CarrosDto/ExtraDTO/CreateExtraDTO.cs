namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public record CreateExtraDTO(
    string CapacidadeTanque, 
    string TipoCombustivel, 
    string CapacidadeCarga, 
    string CapacidadeReboque,
    DateTime? DataReferencia
    );
