using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.User;

public class UpdateUserDTO
{
    
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    public string? UserName { get; set; }

    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string? Email { get; set; }
}