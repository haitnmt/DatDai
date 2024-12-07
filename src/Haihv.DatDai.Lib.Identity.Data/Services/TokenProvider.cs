using System.Security.Claims;
using System.Text;
using Haihv.DatDai.Lib.Identity.Data.Entities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Haihv.DatDai.Lib.Identity.Data.Services;

public sealed class TokenProvider(string secretKey = "Jwt:SecretKey", string issuer = "Jwt:Issuer", string audience = "Jwt:Audience", int expiryMinutes = 10)
{
    public string GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var tokenHandler = new JsonWebTokenHandler();
        
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.UserName),
        };
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
            Issuer = issuer,
            Audience  = audience,
            SigningCredentials = credentials
        };

        return tokenHandler.CreateToken(tokenDescriptor);
    }
}