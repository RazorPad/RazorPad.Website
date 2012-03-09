using System.ComponentModel.DataAnnotations;
using RazorPad.Web.Util;

namespace RazorPad.Web.Website.Models.Account
{
    public class PasswordRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Password { get; set; }

        [MatchesProperty("Password")]
        [Display(Name = "Confirm Password")]
        public string PasswordConfirm { get; set; }
    }
}