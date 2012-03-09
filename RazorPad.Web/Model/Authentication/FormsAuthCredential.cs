using System;
using System.Web.Security;
using RazorPad.Web.Services;

namespace RazorPad.Web.Authentication
{
    public class FormsAuthCredential : Credential, IEquatable<Credential>
    {
        public string Hash { get; set; }
        public string ForgotPasswordToken { get; private set; }


        public bool Equals(Credential other)
        {
            return (other is FormsAuthCredential) 
                   && (Hash == ((FormsAuthCredential)other).Hash);
        }

        public void SetForgotPasswordToken()
        {
            ForgotPasswordToken = Guid.NewGuid().ToString("N");
        }

        public void SetPassword(string password)
        {
            Hash = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
        }

        public static FormsAuthCredential Create(string password)
        {
            var credential = new FormsAuthCredential();
            
            credential.SetPassword(password);

            return credential;
        }
    }
}