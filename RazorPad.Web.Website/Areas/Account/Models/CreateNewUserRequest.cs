using System.ComponentModel.DataAnnotations;

namespace RazorPad.Web.Website.Areas.Account.Models
{
    public class CreateNewUserRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [RegularExpression(
            @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", 
            ErrorMessage = "{0} must be a valid email address")]
        public string EmailAddress { get; set; }

        public string RedirectUrl { get; set; }


        public CreateNewUserRequest()
        {
            RedirectUrl = "~/";
        }
    }
}