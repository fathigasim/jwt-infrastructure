using JwtInfrastructure.Resources;
using System.ComponentModel.DataAnnotations;

namespace JwtInfrastructure.Models.Dtos
{
    public class ForgotPasswordDto
    {
        [Required (ErrorMessageResourceName = "Email_is_Required", ErrorMessageResourceType = typeof(CommonResources))]

        public string Email { get; set; }
    }
}
