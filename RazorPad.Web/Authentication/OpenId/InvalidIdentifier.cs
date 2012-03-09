using System;

namespace RazorPad.Web.Authentication.OpenId
{
    public class InvalidIdentifier : AuthenticationException
    {
        public string Identifier { get; private set; }

        public InvalidIdentifier(string identifier, Exception exception)
            : base(exception)
        {
            Identifier = identifier;
        }
    }
}