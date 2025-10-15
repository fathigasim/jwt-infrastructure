using System.Security.Claims;

namespace JwtInfrastructure.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        // 🔽 Add these for reset password
        string GeneratePasswordResetToken(int userId);
        //Guid? ValidatePasswordResetToken(string token);
        int? ValidatePasswordResetToken(string token);
    }
}
