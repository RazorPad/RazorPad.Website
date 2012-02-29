using System.ComponentModel.DataAnnotations;

namespace RazorPad.Web.Website.Models.Account
{
    public class ResetPasswordRequest : PasswordRequest
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Token { get; set; }
    }
}