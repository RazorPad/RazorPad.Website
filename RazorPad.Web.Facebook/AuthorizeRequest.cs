using System;

namespace RazorPad.Web.Facebook
{
    public class AuthorizeRequest
    {
        public string Code { get; set; }
        public string Error { get; set; }
        // ReSharper disable InconsistentNaming
        public string Error_Reason { get; set; }
        public string Error_Description { get; set; }
        // ReSharper restore InconsistentNaming

        public bool Succeeded
        {
            get { return String.IsNullOrWhiteSpace(Code) == false; }
        }

        public bool DeniedByUser
        {
            get { return Error_Reason == "user_denied"; }
        }
    }
}