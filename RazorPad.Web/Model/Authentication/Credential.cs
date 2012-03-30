using System;

namespace RazorPad.Web.Authentication
{
    public abstract class Credential
    {
        public Guid Id { get; private set; }
    }
}