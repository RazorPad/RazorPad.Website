using System;
namespace RazorPad.Website.Models
{
    public class User
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime DateCreated { get; set; }
        public string ForgotPasswordToken { get; set; }
    }

    public class ForgotPassword
    {
        public bool EmailNotFound { get; set; }
        public bool EmailSent { get; set; }
        public string Email { get; set; }
    }

    public class ResetPassword
    {
        public bool TokenNotFound { get; set; }
        public bool TokenExpiredOrInvalid { get; set; }
        public string UserId { get; set; }
    }
}