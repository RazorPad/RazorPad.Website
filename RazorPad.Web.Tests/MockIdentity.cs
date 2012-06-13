using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace RazorPad.Web
{
    /// <summary>
    /// Mock objects for IIdentity interface
    /// </summary>
    public class MockIdentity : IIdentity
    {
        public MockIdentity(string userName)
        {
            Name = userName;
        }

        /// <summary>
        /// Gets the mocked type of authentication used.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The type of authentication used to identify the user.
        /// </returns>
        public string AuthenticationType
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a mocked value that indicates whether the user has been authenticated.
        /// </summary>
        /// <value></value>
        /// <returns>true if the user was authenticated; otherwise, false.
        /// </returns>
        public bool IsAuthenticated
        {
            get { return !String.IsNullOrEmpty(Name); }
        }

        /// <summary>
        /// Gets the mocked name of the current user.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The name of the user on whose behalf the code is running.
        /// </returns>
        public string Name { get; private set; }
    }

}
