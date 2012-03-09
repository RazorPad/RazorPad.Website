using System;

namespace RazorPad.Web.Authentication.OpenId
{
    public class AuthenticationException : OpenIdAuthenticationResponse
    {
        public Exception Exception { get; private set; }

        public AuthenticationException(Exception exception)
        {
            Exception = exception;
        }
    }
}