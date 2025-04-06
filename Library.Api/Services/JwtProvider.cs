using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Library.Api.Services;

public sealed class JwtProvider
{
    public string CreateToken()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my secret key mysecret key my secret key mysecret key my secret key mysecret key my secret key mysecret key"));
        JwtSecurityToken jwtSecurityToken = new(
            issuer: "Issuer",
            audience: "Audience",
            claims: null,
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddMonths(1),
            signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512));

        JwtSecurityTokenHandler handler = new();
        string token = handler.WriteToken(jwtSecurityToken);

        return token;
    }
}