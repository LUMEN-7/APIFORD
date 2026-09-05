namespace APIFORD.Data.DTOS.Export;

public class ExportarRequestDTO
{
    public List<ItemExportacaoDTO> Itens { get; set; } = new();
    public string Formato { get; set; } = "csv";
    public string Separador { get; set; } = ","; // "," (padrão) ou ";" (Excel pt-BR)
    public ModoResolucaoFonte Modo { get; set; } = ModoResolucaoFonte.Automatico;
    public List<EscolhaManualDTO>? Escolhas { get; set; } // só usado no modo Manual
}

public enum ModoResolucaoFonte { Automatico, Manual }