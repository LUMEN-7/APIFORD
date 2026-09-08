namespace APIFORD.Data.DTOS.Annotations;

public class AtualizarBlocosDTO
{
    public List<BlocoDTO> Blocos { get; set; } = new();
}

public class AtualizarTextoBlocoDTO
{
    public string Texto { get; set; } = string.Empty;
}