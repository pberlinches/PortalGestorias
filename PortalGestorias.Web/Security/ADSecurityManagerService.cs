
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using PortalGestorias.Infrastructure.Security;
using Serilog;
using Constants = IdentityServer3.Core.Constants;
using PortalGestorias.Infrastructure.Data;

namespace PortalGestorias.Web.Security
{
    public class ADSecurityManagerService : IDisposable, ISecurityManagerService
    {
        private readonly ICertificateService certificateService;
        private PrincipalContext principalContext;
        private readonly string domain;
        private readonly string userDomain;
        private readonly string passwordDomain;

        private readonly ILogger log = Log.ForContext<ADSecurityManagerService>();

        private readonly CrmDbContext DB;

        public PrincipalContext PrincipalContext
        {
            get
            {
                if (principalContext == null)
                {
                    try
                    {
                        principalContext = new PrincipalContext(ContextType.Domain, domain, userDomain, passwordDomain);
                    }
                    catch (PrincipalException ex)
                    {
                        log.Error(ex, $"Error creating a domain context into {domain} with user {userDomain}.");
                    }
                }
                return principalContext;
            }
        }

        public ADSecurityManagerService(CrmDbContext secContext, ICertificateService certificateService)
        {
            DB = secContext;
            this.certificateService = certificateService;
            domain = ConfigurationManager.AppSettings[DomainConfiguration.DomainName];
            userDomain = ConfigurationManager.AppSettings[DomainConfiguration.DomainUser];
            passwordDomain = DecryptPassword(ConfigurationManager.AppSettings[DomainConfiguration.DomainPassword]);

            try
            {
                principalContext = new PrincipalContext(ContextType.Domain, domain, userDomain, passwordDomain);
            }
            catch (PrincipalException ex)
            {
                principalContext = null;
                log.Error(ex, $"Error creating a domain context into {domain} with user {userDomain}.");
            }
        }

        private string DecryptPassword(string encryptedPass)
        {
            var pass = encryptedPass;
            var certificate = certificateService.Get();
            var privateKey = certificate.PrivateKey as RSACryptoServiceProvider;

            if (privateKey != null)
            {
                pass = Encoding.UTF8.GetString(privateKey.Decrypt(Convert.FromBase64String(encryptedPass), RSAEncryptionPadding.OaepSHA1));
            }

            return pass;
        }

        public bool ValidateCredentials(string username, string password)
        {
            return PrincipalContext != null && PrincipalContext.ValidateCredentials(username, password, ContextOptions.Negotiate);
        }

        public BasicUser GetIdentityInfoByUsername(string username)
        {
            if (PrincipalContext != null)
            {
                try
                {
                    var principal = Principal.FindByIdentity(PrincipalContext, IdentityType.SamAccountName, username);
                    return GetBasicUser(principal);

                }
                catch (PrincipalException ex)
                {
                    log.Error(ex, $"Error querying domain for user: [{username}]");
                }
            }

            return null;
        }

        public BasicUser GetIdentityInfoBySubject(string subject)
        {
            if (PrincipalContext != null)
            {
                try
                {
                    var principal = Principal.FindByIdentity(PrincipalContext, IdentityType.Sid, subject);
                    return GetBasicUser(principal);

                }
                catch (PrincipalException ex)
                {
                    log.Error(ex, $"Error querying domain for user sid: [{subject}]");
                }
            }

            return null;
        }

        private BasicUser GetBasicUser(Principal principal)
        {
            if (principal == null)
            {
                return null;
            }

            //var user = DB.Empleados.FirstOrDefault(u => u.Login == principal.Sid.Value);

            var claims = new List<Claim>();
            //claims.Add(new Claim(Constants.ClaimTypes.Id, user.Id.ToString()));
            //claims.Add(new Claim(Constants.ClaimTypes.Subject, user.Login));
            claims.Add(new Claim(Constants.ClaimTypes.Role, AppRoles.Usuario));

            //if (user.Administrador)
            //    claims.Add(new Claim(Constants.ClaimTypes.Role, AppRoles.Administrador));

            //if (user.GestorEmpleado)
            //    claims.Add(new Claim(Constants.ClaimTypes.Role, AppRoles.GestorEmpleado));

            //if (user.GestorParteHoras)
            //    claims.Add(new Claim(Constants.ClaimTypes.Role, AppRoles.GestorParteHoras));

            //if (user.GestorProyecto)
            //    claims.Add(new Claim(Constants.ClaimTypes.Role, AppRoles.GestorProyecto));

            //if (user.GestorAusencias)
            //    claims.Add(new Claim(Constants.ClaimTypes.Role, AppRoles.GestorAusencias));

            return new BasicUser
            {
                Subject = principal.Sid.ToString(),
                UserName = principal.SamAccountName,
                //Name = user?.Nombre,
                //Email = user?.EmailCorporativo,
                Claims = claims
            };
        }

        public void Dispose()
        {
            PrincipalContext?.Dispose();
            DB?.Dispose();
        }

        public struct DomainConfiguration
        {
            public const string DomainName = "DomainName";

            public const string DomainUser = "DomainUser";

            public const string DomainPassword = "DomainPassword";

            public const string DComApplicationId = "DComApplicationId";
        }
    }
}