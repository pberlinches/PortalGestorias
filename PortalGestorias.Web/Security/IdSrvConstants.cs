using System.Configuration;

namespace PortalGestorias.Web.Security
{
    public static class IdSrvConstants
    {
        public const string AuthorityKey = "Authentication:Authority";
        //public const string BaseAddress = "http://localhost:55815/idsrv";

        public const string AuthorizeEndpoint = "/connect/authorize";
        public const string LogoutEndpoint = "/connect/endsession";
        public const string TokenEndpoint = "/connect/token";
        public const string UserInfoEndpoint = "/connect/userinfo";
        public const string IdentityTokenValidationEndpoint = "/connect/identitytokenvalidation";
        public const string TokenRevocationEndpoint = "/connect/revocation";

        public const string ThisWebAppUrlKey = "Authentication:ThisWebAppUrl";
        public const string AspNetWebAppClientId = "PortalGestorias";

        public static string AuthorityBaseAddress { get; set; }

        public static string AspNetWebAppUrl
        {
            get { return ConfigurationManager.AppSettings[IdSrvConstants.ThisWebAppUrlKey]; }
        }

        public static string GetEndpoint(string path)
        {
            return AuthorityBaseAddress + path;
        }
    }
}