using System.Collections.Generic;
using System.Security.Claims;

namespace PortalGestorias.Web.Security
{
    public class BasicUser
    {
        public string Subject { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public IEnumerable<Claim> Claims { get; set; }
    }
}