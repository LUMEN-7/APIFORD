using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.User;

public class CreateUserDTO
{

    [Required(ErrorMessage = "O email é obrigatória.")]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public string? FotoPerfilUrl { get; set; }


}
