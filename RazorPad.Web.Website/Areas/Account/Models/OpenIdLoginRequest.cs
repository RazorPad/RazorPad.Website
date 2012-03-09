namespace RazorPad.Web.Website.Areas.Account.Models
{
    public class OpenIdLoginRequest
    {
        public string ClaimsId { get; set; }
        public string ReturnUrl { get; set; }

        public OpenIdLoginRequest()
        {
            ReturnUrl = "~/";
        }
    }
}