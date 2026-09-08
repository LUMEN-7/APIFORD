using System.Text.Json.Serialization;

namespace APIFORD.Data.DTOS.Search;

public class BuscaDTO
{
    
    public string Model { get; set; }
    public string Brand { get; set; }
    public int? Year { get; set; }
    //public List<string> Urls { get; set; } = new();

}
