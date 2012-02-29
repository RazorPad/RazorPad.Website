namespace RazorPad.Web.Website.Models.Account
{
    public class ForgotPassword
    {
        public bool EmailNotFound { get; set; }
        public bool EmailSent { get; set; }
        public string Email { get; set; }
    }
}