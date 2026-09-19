namespace APIFORD.Model.Workspace;

public class WorkspaceCurtida
{
    public int Id { get; set; }
    public int WorkspacePostId { get; set; }
    public WorkspacePost WorkspacePost { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
}