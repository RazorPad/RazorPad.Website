using System;

namespace RazorPad.Web
{
    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public DateTime DateCreated { get; set; }
        public string ForgotPasswordToken { get; set; }
    }
}