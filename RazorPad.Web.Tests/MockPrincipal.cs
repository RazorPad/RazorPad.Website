using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace RazorPad.Web
{
    /// <summary>
    /// Mocked obejct for the IPrincipal class
    /// </summary>
    public class MockPrincipal : IPrincipal
    {
        private readonly string[] _roles;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockPrincipal"/> class.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <param name="roles">The roles.</param>
        public MockPrincipal(IIdentity identity, string[] roles)
        {
            Identity = identity;
            _roles = roles;
        }

        /// <summary>
        /// Gets the identity of the current principal.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The <see cref="T:System.Security.Principal.IIdentity"/> object associated with the current principal.
        /// </returns>
        public IIdentity Identity { get; set; }

        /// <summary>
        /// Determines whether the current principal belongs to the specified role.
        /// </summary>
        /// <param name="role">The name of the role for which to check membership.</param>
        /// <returns>
        /// true if the current principal is a member of the specified role; otherwise, false.
        /// </returns>
        public bool IsInRole(string role)
        {
            return _roles != null && _roles.Contains(role);
        }
    }

}
