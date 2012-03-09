using System;

namespace RazorPad.Web.Authentication
{
    public class IntegratedAuthenticationCredential : Credential
    {
        public string Token { get; set; }
        public DateTime? Expiration { get; set; }
    }
}