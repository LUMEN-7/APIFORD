using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.User;

public class CreateUserDTO
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "O email é obrigatória.")]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [Compare("Password", ErrorMessage = "As senhas não coincidem.")]
    public string ConfirmPassword { get; set; }

    public List<string> CarroIds { get; set; } = new List<string>();

}
