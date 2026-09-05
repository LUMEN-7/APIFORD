namespace APIFORD.Data.DTOS.Export;

public class ItemExportacaoDTO
{
    public int LinhagemId { get; set; }
    public int? CarroId { get; set; } // se vier preenchido, usa essa versão específica; se não vier, pega a mais recente da linhagem
}
