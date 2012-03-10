using RazorPad.Web.Authentication;

namespace RazorPad.Web.Website.Areas.Account.Models
{
    public abstract class CredentialRequest
    {
        public abstract Credential ToCredential();
    }
}