namespace APIFORD.Data.DTOS.Search;

public class BuscaDTO
{
    public string Model { get; set; }
    public string Brand { get; set; }
    public string Year { get; set; }
    public List<string>? Urls { get; set; }

}
