using System;

using SIS.MvcFramework.Identity;

namespace SIS.MvcFramework.Attributes.Security
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AuthorizeAttribute : Attribute
    {
        private readonly string role;

        public AuthorizeAttribute(string role = null)
            => this.role = role;

        private bool IsLoggedIn(Principal principal)
            => principal != null;

        private bool IsRolePresent(Principal principal)
            => principal.Roles.Contains(this.role);

        public bool IsAuthorized(Principal principal)
            => this.role == null
                ? this.IsLoggedIn(principal)
                : this.IsRolePresent(principal);
    }
}