using System;
using System.ComponentModel.DataAnnotations;
using RazorPad.Web.Authentication;

namespace RazorPad.Web.Website.Areas.Account.Models
{
    public class IntegratedAuthenticationCredentialsRequest : CredentialRequest
    {
        public long? Expiration { get; set; }

        [Required]
        public string Token { get; set; }

        public string UserId { get; set; }


        public override Credential ToCredential()
        {
            var credential = new IntegratedAuthenticationCredential
                                 {
                                     Token = Token,
                                     UserId = UserId,
                                 };

            if(Expiration.HasValue)
                credential.Expiration = new DateTime(Expiration.Value);

            return credential;
        }
    }
}