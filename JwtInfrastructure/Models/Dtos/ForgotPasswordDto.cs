using System.ComponentModel.DataAnnotations;

namespace JwtInfrastructure.Models.Dtos
{
    public class ForgotPasswordDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
