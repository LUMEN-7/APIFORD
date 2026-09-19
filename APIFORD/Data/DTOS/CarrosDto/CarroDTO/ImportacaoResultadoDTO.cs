using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

namespace APIFORD.Data.DTOS.CarrosDto.CarroDTO;

public class ImportacaoResultadoDTO
{
    public ReadCarroDTO Carro { get; set; } = null!;
    public List<string> CamposNaoAplicados { get; set; } = new();
}
