using System;
using System.Linq;
using IdentityServer3.Core;

namespace PortalGestorias.Web.Security
{
    public class MockSecurityManagerService : ISecurityManagerService
    {
        public MockSecurityManagerService()
        {
            
        }

        public bool ValidateCredentials(string username, string password)
        {
            return Users.Get()
                .Any(u => u.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase) &&
                            u.Password.Equals(password));
        }

        public BasicUser GetIdentityInfoByUsername(string username)
        {
            return Users.Get()
                .Where(u => u.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase))
                .Select(u => new BasicUser { Subject = u.Subject, UserName = u.Username }).FirstOrDefault();
        }

        public BasicUser GetIdentityInfoBySubject(string subject)
        {
            return Users.Get()
                .Where(u => u.Subject.Equals(subject, StringComparison.InvariantCultureIgnoreCase))
                .Select(u => new BasicUser
                {
                    Subject = u.Subject,
                    UserName = u.Username,
                    Email = u.Claims.FirstOrDefault(c => c.Type == Constants.ClaimTypes.Email)?.Value,
                    Name = u.Claims.FirstOrDefault(c => c.Type == Constants.ClaimTypes.Name)?.Value,
                    Claims = u.Claims
                }).FirstOrDefault();
        }
    }
}