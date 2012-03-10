using System;

namespace RazorPad.Web.Authentication
{
    public class IntegratedAuthenticationCredential : Credential
    {
        public DateTime? Expiration { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
    }
}