using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services.Default;

namespace PortalGestorias.Web.Security
{
    public class UserService : UserServiceBase
    {
        private readonly ISecurityManagerService securityManager;

        public UserService(ISecurityManagerService securityManager)
        {
            this.securityManager = securityManager;
        }

        public override Task AuthenticateLocalAsync(LocalAuthenticationContext context)
        {
            if (securityManager.ValidateCredentials(context.UserName, context.Password))
            {
                var user = securityManager.GetIdentityInfoByUsername(context.UserName);
                if (user != null)
                {
                    var contextAuthenticateResult = new AuthenticateResult(user.Subject, user.UserName);
                    context.AuthenticateResult = contextAuthenticateResult;
                }
            }

            return base.AuthenticateLocalAsync(context);
        }

        public override Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectClaim = context.Subject.Claims.First(c => c.Type.Equals(Constants.ClaimTypes.Subject));
            var identity = securityManager.GetIdentityInfoBySubject(subjectClaim.Value);

            if (identity != null)
            {
                var claims = new List<Claim>
                {
                    subjectClaim,
                    new Claim(Constants.ClaimTypes.Name, identity.Name ?? identity.UserName)
                };
                identity.Claims.ToList().ForEach(c => claims.Add(c));

                context.Subject = new ClaimsPrincipal(new ClaimsIdentity(claims, Constants.PrimaryAuthenticationType, Constants.ClaimTypes.Name, Constants.ClaimTypes.Role));
                context.IssuedClaims = claims;
            }
            return base.GetProfileDataAsync(context);
        }
    }
}