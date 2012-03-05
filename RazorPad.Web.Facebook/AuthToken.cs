using System;

namespace RazorPad.Web.Facebook
{
    public class AuthToken
    {
        /// <summary>
        /// The Token that can be saved to identify the user later
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The expiration of the token (in number of seconds)
        /// </summary>
        public DateTime Expiration { get; set; }
    }
}
