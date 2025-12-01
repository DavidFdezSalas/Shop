using System.Security.Claims;

namespace Shop.APIIdentity.Services.Auth
{
    public interface ITokenService
    {
        TokenResult GenerateToken(IEnumerable<Claim> claims);
    }

    public record TokenResult(string Token, DateTime Expiration);
}
