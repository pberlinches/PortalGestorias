using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Helpers;
using PortalGestorias.Infrastructure.Security;
using PortalGestorias.Web.App_Start;
using PortalGestorias.Web.Security;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using IdentityModel.Client;
using IdentityServer3.Core.Services.Default;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using Serilog;
using AuthenticationOptions = IdentityServer3.Core.Configuration.AuthenticationOptions;

namespace PortalGestorias.Web
{
    public class Startup
    {
        private readonly ILogger log = Log.ForContext<Startup>();

        public void Configuration(IAppBuilder app)
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = IdentityServer3.Core.Constants.ClaimTypes.Subject;
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            //ConfigureIdentityServer(app);

            try
            {
                ConfigureAuthentication(app);
            }
            catch (Exception ex)
            {
                log.Fatal(ex, "Error configuring authentication layer.");
                throw;
            }

            app.UseResourceAuthorization(new AuthorizationManager());
        }

        private static void ConfigureAuthentication(IAppBuilder app)
        {
            var authority = ConfigurationManager.AppSettings[IdSrvConstants.AuthorityKey];
            var thisAppUrl = ConfigurationManager.AppSettings[IdSrvConstants.ThisWebAppUrlKey];
            IdSrvConstants.AuthorityBaseAddress = authority;

            app.UseCookieAuthentication(new CookieAuthenticationOptions { AuthenticationType = "Cookies", SlidingExpiration = true, ExpireTimeSpan = TimeSpan.FromDays(15) });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = IdSrvConstants.AspNetWebAppClientId,
                Authority = authority,
                RedirectUri = thisAppUrl,
                PostLogoutRedirectUri = thisAppUrl,
                ResponseType = "code id_token",
                Scope = "openid profile",
                UseTokenLifetime = false,
                

                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = IdentityServer3.Core.Constants.ClaimTypes.Name,
                    RoleClaimType = IdentityServer3.Core.Constants.ClaimTypes.Role
                },

                SignInAsAuthenticationType = "Cookies",

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthorizationCodeReceived = AuthorizationCodeReceived,
                    RedirectToIdentityProvider = RedirectToIdentityProvider,
                    AuthenticationFailed = Failed
                }
            });
        }


        private static Task Failed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> arg)
        {
            if (arg.Exception is OpenIdConnectProtocolInvalidNonceException)
            {
                if (arg.Exception.Message.Contains("IDX10311"))
                {
                    arg.SkipToNextMiddleware();
                }
            }
            return Task.FromResult(0);
        }

        private static Task RedirectToIdentityProvider(RedirectToIdentityProviderNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> n)
        {
            // if signing out, add the id_token_hint
            if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
            {
                var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

                if (idTokenHint != null)
                {
                    n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                }
            }

            return Task.FromResult(0);
        }

        private static async Task AuthorizationCodeReceived(AuthorizationCodeReceivedNotification n)
        {
            // use the code to get the access and refresh token
            var tokenClient = new TokenClient(IdSrvConstants.GetEndpoint(IdSrvConstants.TokenEndpoint), IdSrvConstants.AspNetWebAppClientId, "secret");

            var tokenResponse = await tokenClient.RequestAuthorizationCodeAsync(n.Code, n.RedirectUri);

            if (tokenResponse.IsError)
            {
                throw new Exception(tokenResponse.Error);
            }

            // use the access token to retrieve claims from userinfo
            var userInfoClient = new UserInfoClient(new Uri(IdSrvConstants.GetEndpoint(IdSrvConstants.UserInfoEndpoint)), tokenResponse.AccessToken);

            var userInfoResponse = await userInfoClient.GetAsync();

            // create new identity
            var id = new ClaimsIdentity(n.AuthenticationTicket.Identity.AuthenticationType);
            id.AddClaims(userInfoResponse.GetClaimsIdentity().Claims);

            id.AddClaim(new Claim("access_token", tokenResponse.AccessToken));
            id.AddClaim(new Claim("expires_at", DateTime.Now.AddSeconds(tokenResponse.ExpiresIn).ToLocalTime().ToString(CultureInfo.InvariantCulture)));
            if (!string.IsNullOrWhiteSpace(tokenResponse.RefreshToken))
            {
                id.AddClaim(new Claim("refresh_token", tokenResponse.RefreshToken));
            }
            id.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));
            id.AddClaim(new Claim("sid", n.AuthenticationTicket.Identity.FindFirst("sid").Value));

            n.AuthenticationTicket = new AuthenticationTicket(new ClaimsIdentity(id.Claims, n.AuthenticationTicket.Identity.AuthenticationType, "name", "role"), n.AuthenticationTicket.Properties);
        }

        private static void ConfigureIdentityServer(IAppBuilder app)
        {
            app.Map("/idsrv", idsrvApp =>
            {
                AuthenticationType authType;
                if (!Enum.TryParse(ConfigurationManager.AppSettings["AuthenticationType"], out authType))
                {
                    authType = AuthenticationType.TestingLocal;
                }

                var isFactory = new IdentityServerServiceFactory()
                    .UseInMemoryClients(Clients.Get())
                    .UseInMemoryScopes(Scopes.Get());
                isFactory.UserService = new Registration<IUserService, UserService>();
                isFactory.CorsPolicyService= new Registration<ICorsPolicyService>(new DefaultCorsPolicyService { AllowAll = true });
                isFactory.ConfigureUserServiceCache(TimeSpan.FromDays(2));

               

                switch (authType)
                {
                    case AuthenticationType.TestingLocal:
                        isFactory.Register(new Registration<ISecurityManagerService>(new MockSecurityManagerService()));
                        break;
                    case AuthenticationType.Domain:
                        isFactory.Register(new Registration<ISecurityManagerService>(WindsorActivator.Container.Resolve<ISecurityManagerService>()));
                        break;
                }

                //TODO replace styles for IdentityServer views (login/logout)
                //isFactory.ConfigureDefaultViewService(new DefaultViewServiceOptions{Stylesheets = });

                idsrvApp.UseIdentityServer(new IdentityServerOptions
                {
                    SiteName = "Embedded IdentityServer",
                    SigningCertificate = WindsorActivator.Container.Resolve<ICertificateService>().Get(),

                    Factory = isFactory,
                    RequireSsl = false,
                    CspOptions = new CspOptions
                    {
                        ImgSrc = "* data:"
                    },
                    AuthenticationOptions = new AuthenticationOptions
                    {
                        EnablePostSignOutAutoRedirect = true
                    }
                });
            });
        }

        public enum AuthenticationType
        {
            Domain,
            TestingLocal
        }
    }
}

