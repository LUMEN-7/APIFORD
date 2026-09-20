using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.User;

public class ShowUserDTO
{
    public string Id { get; set; }
    public string NomeExibicao { get; set; }
    public string Email { get; set; }
    public List<string> CarroIds { get; set; } = new List<string>();
    public bool IsDeleted { get; set; }
}
