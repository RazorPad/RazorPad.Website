using System.ComponentModel.DataAnnotations;

namespace RazorPad.Web.Website.Areas.Account.Models
{
    public class ResetPasswordRequest : PasswordRequest
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Token { get; set; }

        public string RedirectUrl { get; set; }

        public ResetPasswordRequest()
        {
            RedirectUrl = "~/";
        }
    }
}