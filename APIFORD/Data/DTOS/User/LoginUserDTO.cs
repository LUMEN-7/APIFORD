using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.User;

public class LoginUserDTO
{
    [Required(ErrorMessage = "O nome do usuário ou o e-mail é obrigatório.")]
    public string UserIdentifier { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

}
