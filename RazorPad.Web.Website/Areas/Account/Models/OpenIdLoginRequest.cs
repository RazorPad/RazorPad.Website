namespace RazorPad.Web.Website.Models.OpenId
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