using APIFORD.Model.Workspace.enums;

namespace APIFORD.Data.DTOS.Workspace;

public class CriarPostDTO
{
    public TipoPost Tipo { get; set; }
    public string Conteudo { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public string? ResponsavelUserId { get; set; }
    public StatusRevisao? Status { get; set; }
    public TipoConteudoVinculado? TipoConteudoVinculado { get; set; }
    public int? ConteudoVinculadoId { get; set; }
    public string? ConteudoVinculadoTitulo { get; set; }
}