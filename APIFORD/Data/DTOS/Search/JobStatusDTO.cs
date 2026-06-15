// APIFORD/Data/DTOS/Search/JobStatusDTO.cs
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

namespace APIFORD.Data.DTOS.Search;

public class JobStatusDTO
{
    public string Status { get; set; } = string.Empty; // pending | running | done | error | not_found
    public string? Error { get; set; }
    public ReadCarroDTO? Carro { get; set; }
}