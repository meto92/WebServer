using System.Collections.Generic;
using System.Linq;

namespace SIS.MvcFramework.Identity
{
    public class Principal
    {
        public Principal()
            => this.Roles = new List<string>();

        public string Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public bool IsAdmin => this.Roles.Any(role => role.ToUpper() == "ADMIN");

        public List<string> Roles { get; set; }
    }
}