using System.ComponentModel.DataAnnotations;
using RazorPad.Web.Website.Models.Util;

namespace RazorPad.Web.Website.Models.Account
{
    public class CreateNewUserRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Username { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Password { get; set; }

        [MatchesProperty("Password")]
        [Display(Name = "Confirm Password")]
        public string PasswordConfirm { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [RegularExpression(
            @"\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b", 
            ErrorMessage = "{0} must be a valid email address")]
        public string Email { get; set; }


        public CreateNewUserRequest()
        {
        }

        public CreateNewUserRequest(string username, string password, string email)
        {
            Username = username;
            Password = password;
            Email = email;
        }
    }
}