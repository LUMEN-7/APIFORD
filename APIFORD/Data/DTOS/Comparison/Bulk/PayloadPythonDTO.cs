namespace APIFORD.Data.DTOS.Comparison.Bulk;

public class PayloadPythonDTO
{
    public string CarroBaseNome { get; set; }
    public Dictionary<string, decimal> ValoresDoCarroBase { get; set; } = new();
    public Dictionary<string, decimal> MediasDosConcorrentes { get; set; } = new();
    public int QuantidadeConcorrentes { get; set; }
}
