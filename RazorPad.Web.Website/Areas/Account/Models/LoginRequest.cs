using System.ComponentModel.DataAnnotations;

namespace RazorPad.Web.Website.Areas.Account.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Invalid username")]
        [StringLength(int.MaxValue, MinimumLength = 4)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Invalid password")]
        [StringLength(int.MaxValue, MinimumLength = 4)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }


        public LoginRequest()
        {
        }

        public LoginRequest(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}