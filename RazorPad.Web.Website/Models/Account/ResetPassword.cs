namespace RazorPad.Web.Website.Models.Account
{
    public class ResetPassword
    {
        public bool TokenNotFound { get; set; }
        public bool TokenExpiredOrInvalid { get; set; }
        public string UserId { get; set; }
    }
}