using JwtInfrastructure.Data;
using JwtInfrastructure.Models;
using JwtInfrastructure.Models.Dtos;
using JwtInfrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtInfrastructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserContext _userContext;
        private readonly ITokenService _tokenService;

        public AuthController(UserContext userContext, ITokenService tokenService)
        {
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        [HttpPost("login")]
        //[Bind("username", "password")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            if (loginModel is null)
            {
                return BadRequest("Invalid client request");
            }

            var user = _userContext.LoginModels.FirstOrDefault(u =>
                (u.UserName == loginModel.UserName) && (u.Password == loginModel.Password));
            if (user is null)
                return Unauthorized();

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, loginModel.UserName),
            new Claim(ClaimTypes.Role, user.Role)
        };
            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

            _userContext.SaveChanges();

            return Ok(new AuthenticatedResponse
            {
                //User=user,
                Token = accessToken,
                RefreshToken = refreshToken
            });
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

                var parameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidAudience = _config["Jwt:Audience"],
                    IssuerSigningKey = key,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = handler.ValidateToken(dto.Token, parameters, out var validatedToken);

                // Ensure token was generated for reset purpose
                if (principal.FindFirst("purpose")?.Value != "password-reset")
                    return BadRequest(new { message = "Invalid token purpose." });

                var email = principal.FindFirst("email")?.Value;
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null) return BadRequest(new { message = "Invalid user." });

                // Hash the new password
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                await _db.SaveChangesAsync();

                return Ok(new { message = "Password has been reset successfully." });
            }
            catch (SecurityTokenExpiredException)
            {
                return BadRequest(new { message = "Reset token has expired." });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Invalid reset token." });
            }
        }

    }
}
