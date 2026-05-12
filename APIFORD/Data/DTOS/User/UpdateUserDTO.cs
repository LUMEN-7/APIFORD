namespace APIFORD.Data.DTOS.User;

public class UpdateUserDTO
{
    public string? id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public List<string>? CarroIds { get; set; }
}
