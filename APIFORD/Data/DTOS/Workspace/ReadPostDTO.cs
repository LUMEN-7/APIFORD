using APIFORD.Model.Workspace.enums;

namespace APIFORD.Data.DTOS.Workspace;

public class ReadPostDTO
{
    public int Id { get; set; }
    public AutorResumoDTO Autor { get; set; } = null!;
    public TipoPost Tipo { get; set; }
    public string Conteudo { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public AutorResumoDTO? Responsavel { get; set; }
    public StatusRevisao? Status { get; set; }
    public TipoConteudoVinculado? TipoConteudoVinculado { get; set; }
    public int? ConteudoVinculadoId { get; set; }
    public string? ConteudoVinculadoTitulo { get; set; }
    public bool Fixado { get; set; }
    public DateTime CriadoEm { get; set; }
    public int TotalCurtidas { get; set; }
    public bool CurtidoPeloUsuarioAtual { get; set; }
    public List<ReadComentarioDTO> Comentarios { get; set; } = new();
}