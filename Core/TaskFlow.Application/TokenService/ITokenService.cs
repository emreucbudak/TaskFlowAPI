using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.TokenService
{
    public interface ITokenService
    {
        JwtSecurityToken CreateToken(User user, IList<string> roles);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
        string CreateRefreshToken();

    }
}
