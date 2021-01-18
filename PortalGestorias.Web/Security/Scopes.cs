using System.Collections.Generic;
using IdentityServer3.Core.Models;

namespace PortalGestorias.Web.Security
{
    public static class Scopes
    {
        public static IEnumerable<Scope> Get()
        {
            var scopes = new List<Scope>
            {
                new Scope
                {
                    Enabled = true,
                    Name = "roles",
                    Type = ScopeType.Identity,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim("role")
                    }
                }
            };

            scopes.AddRange(StandardScopes.All);
            scopes.Add(StandardScopes.OfflineAccess);

            return scopes;
        }
    }

}