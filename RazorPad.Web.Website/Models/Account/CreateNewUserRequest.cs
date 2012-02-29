using System.ComponentModel.DataAnnotations;

namespace RazorPad.Web.Website.Models.Account
{
    public class CreateNewUserRequest : PasswordRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [RegularExpression(
            @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", 
            ErrorMessage = "{0} must be a valid email address")]
        public string Email { get; set; }
    }
}