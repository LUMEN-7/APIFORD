using APIFORD.Model.Workspace.enums;

namespace APIFORD.Data.DTOS.Workspace;

public class AtualizarStatusDTO { public StatusAtividade Status { get; set; } }
public class CriarComentarioDTO { public string Conteudo { get; set; } = string.Empty; }
