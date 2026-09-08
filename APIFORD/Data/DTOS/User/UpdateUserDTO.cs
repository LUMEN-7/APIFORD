using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.User;

public class UpdateUserDTO
{
    public string? id { get; set; }

    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    public string? UserName { get; set; }

    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string? Email { get; set; }

    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "A senha deve ter pelo menos 8 caracteres.")]
    public string? Password { get; set; }

    public List<string>? CarroIds { get; set; }
}