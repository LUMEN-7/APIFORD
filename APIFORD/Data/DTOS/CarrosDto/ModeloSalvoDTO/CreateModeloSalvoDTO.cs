using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public record CreateModeloSalvoDTO(
    [ Range(1, int.MaxValue, ErrorMessage = "CarroId inválido.")] int LinhagemId
);