using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Model.CarroClasses;

namespace APIFORD.Data.DTOS.CarrosDto.SavedModel;

public class ReadModeloSalvoDTO
{
    public string UserId { get; set; }
    public List<ReadCarroDTO> FavoriteCarros { get; set; } = new List<ReadCarroDTO>();
}
