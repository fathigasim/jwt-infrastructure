using JwtInfrastructure.Resources;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace JwtInfrastructure.Models.Dtos
{
    public class RegisterDto
    {
        [Required(ErrorMessageResourceName ="Username_is_Required",ErrorMessageResourceType = typeof(CommonResources))]
        public string UserName { get; set; }
        [Required(ErrorMessageResourceName = "Email_is_Required", ErrorMessageResourceType = typeof(CommonResources))]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", ErrorMessageResourceName = "Please_enter_a_valid_email_address", ErrorMessageResourceType = typeof(CommonResources))]
        public string? Email { get; set; }
        [Required(ErrorMessageResourceName = "Password_is_required", ErrorMessageResourceType = typeof(CommonResources))]
        [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d\s]).{8,}$",
        ErrorMessageResourceName = "Password_Policy", ErrorMessageResourceType = typeof(CommonResources)
    )]
        public string? Password { get; set; }

        public string? Role { get; set; } = "User";
    }
}
