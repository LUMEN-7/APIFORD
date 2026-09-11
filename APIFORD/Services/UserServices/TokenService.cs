using APIFORD.Model.User;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APIFORD.Services.UserServices;

public class TokenService
{
    private IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        Claim[] claims = new Claim[]
        {
            new Claim("username", user.UserName),
            new Claim("email", user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim("loginTimestamp", DateTime.UtcNow.ToString())
        };

        var chave = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes
            (_configuration["SymmetricSecurityKey"])
            ); //maneira certa

        var signingCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var horasExpiracao = _configuration.GetValue<double>("TokenExpiracaoHoras", 12);

        var token = new JwtSecurityToken(
            expires: DateTime.Now.AddHours(horasExpiracao), // era AddMinutes(10)
            claims: claims,
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
