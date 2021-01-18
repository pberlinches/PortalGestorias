using System;
using System.Collections.Generic;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;

namespace PortalGestorias.Web.Security
{
    public static class Clients
    {
        public static IList<Client> Get()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientName = "John Deere Inventario",
                    ClientId = "PortalGestorias",
                    Flow = Flows.Hybrid,
                    AllowAccessTokensViaBrowser = false,

                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },

                    AllowedScopes = new List<string>
                    {
                        Constants.StandardScopes.OpenId,
                        Constants.StandardScopes.Profile,
                        Constants.StandardScopes.Email,
                        Constants.StandardScopes.Roles,
                        Constants.StandardScopes.OfflineAccess,
                        "read",
                        "write"
                    },

                    ClientUri = IdSrvConstants.AspNetWebAppUrl,

                    RequireConsent = false,
                    AccessTokenType = AccessTokenType.Reference,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    UpdateAccessTokenClaimsOnRefresh = true,

                    RedirectUris = new List<string>
                    {
                        IdSrvConstants.AspNetWebAppUrl
                    },

                    PostLogoutRedirectUris = new List<string>
                    {
                        IdSrvConstants.AspNetWebAppUrl
                    },

                    LogoutUri = $"{IdSrvConstants.AspNetWebAppUrl}/Home/OidcSignOut",
                    LogoutSessionRequired = true
                }
            };
        }
    }
}