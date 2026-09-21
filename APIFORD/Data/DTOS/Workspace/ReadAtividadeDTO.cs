using APIFORD.Model.Workspace.enums;

namespace APIFORD.Data.DTOS.Workspace;

public class ReadAtividadeDTO
{
    public int Id { get; set; }
    public TipoAtividade Tipo { get; set; }
    public string AtorNome { get; set; }
    public string AlvoNome { get; set; }
    public int? PostId { get; set; }
    public string PostConteudoResumo { get; set; }
    public string StatusNovo { get; set; }
    public DateTime DataCriacao { get; set; }
}
