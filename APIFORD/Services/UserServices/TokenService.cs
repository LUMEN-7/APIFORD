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


        //var chave = new SymmetricSecurityKey
        //    (Encoding.UTF8.GetBytes
        //    ("skdjhjasdhfodahas")
        //    );//literalmente uma das piores decisoes de segurança, pois esta literalmente no codigo a cave de encoding
        //    a chave para esse projeto é outra :)

        var chave = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes
            (_configuration["SymmetricSecurityKey"])
            ); //maneira certa

        var signingCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken
            (
            expires: DateTime.Now.AddMinutes(10),
            claims: claims,
            signingCredentials: signingCredentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
