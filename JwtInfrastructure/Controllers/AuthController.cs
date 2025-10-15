using JwtInfrastructure.Data;
using JwtInfrastructure.Models;
using JwtInfrastructure.Models.Dtos;
using JwtInfrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        IEmailService emailService;
        public AuthController(UserContext userContext, ITokenService tokenService, IEmailService _emailService)
        {
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            emailService = _emailService ?? throw new ArgumentNullException(nameof(_emailService));
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
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
            
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



        [HttpPost("forgot")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userContext.LoginModels.Where(p => p.Email == forgotPasswordDto.Email).FirstOrDefaultAsync();//await _userService.GetByEmailAsync(email);
            if (user == null)
                return Ok(); // don’t leak info

            var token = _tokenService.GeneratePasswordResetToken(user.Id);
            var resetUrl = $"http://localhost:5173/reset?token={token}";
             await emailService.SendAsync(user.Email,"ResetPassword" ,resetUrl);

            return Ok(new { message = "If the email exists, a reset link was sent." });
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var userId = _tokenService.ValidatePasswordResetToken(dto.Token);
            if (userId == null)
                return BadRequest(new { message = "Invalid or expired token." });
            var user = await _userContext.LoginModels.Where(p=>p.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                return BadRequest("something went wrong");
            }
            user.Password = dto.NewPassword;
            _userContext.LoginModels.Update(user);
            _userContext.SaveChanges();
            //  await _userService.UpdatePasswordAsync(userId.Value, dto.NewPassword);
            return Ok(new { message = "Password successfully reset." });
        }

    }
}
