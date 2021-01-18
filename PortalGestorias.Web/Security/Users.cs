using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer3.Core;
using IdentityServer3.Core.Services.InMemory;

namespace PortalGestorias.Web.Security
{
    public static class Users
    {
        public static List<InMemoryUser> Get()
        {
            return new List<InMemoryUser>
            {
                new InMemoryUser
                {
                    Username = "admin",
                    Password = "admin",
                    Subject = "48AB4374-FEE5-42AC-A77D-A80DE262FCDA",

                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.Email, "admin@test.com"),
                        new Claim(Constants.ClaimTypes.PreferredUserName, "admin"),
                        new Claim(Constants.ClaimTypes.Subject, "48AB4374-FEE5-42AC-A77D-A80DE262FCDA"),
                        new Claim(Constants.ClaimTypes.Role, AppRoles.Administrador),
                        new Claim(Constants.ClaimTypes.Role, AppRoles.Usuario),
                        new Claim(Constants.ClaimTypes.Id, "1")
                    }
                },
                new InMemoryUser
                {
                    Username = "mgarciag",
                    Password = "mgarciag",
                    Subject = "0258E7EE-6674-4447-8E1A-BB7574DFCB51",

                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.Email, "admin@test.com"),
                        new Claim(Constants.ClaimTypes.PreferredUserName, "mgarciag"),
                        new Claim(Constants.ClaimTypes.Subject, "0258E7EE-6674-4447-8E1A-BB7574DFCB51"),
                        new Claim(Constants.ClaimTypes.Name, "María García"),
                        new Claim(Constants.ClaimTypes.Role, AppRoles.Usuario),
                        new Claim(Constants.ClaimTypes.Id, "79")
                    }
                },
                new InMemoryUser
                {
                    Username = "acarbonell",
                    Password = "acarbonell",
                    Subject = "0258E7EE-6674-4447-8E1A-BB7574DFCB52",

                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.Email, "admin@test.com"),
                        new Claim(Constants.ClaimTypes.PreferredUserName, "acarbonell"),
                        new Claim(Constants.ClaimTypes.Subject, "0258E7EE-6674-4447-8E1A-BB7574DFCB52"),
                        new Claim(Constants.ClaimTypes.Name, "Alberto Carbonell"),
                        new Claim(Constants.ClaimTypes.Role, AppRoles.Usuario),
                        new Claim(Constants.ClaimTypes.Id, "14")
                    }
                },
                new InMemoryUser
                {
                    Username = "aguardiola",
                    Password = "aguardiola",
                    Subject = "8F8EB0EC-6250-4A76-8B07-F28168574FF0",

                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.Email, "admin@test.com"),
                        new Claim(Constants.ClaimTypes.PreferredUserName, "aguardiola"),
                        new Claim(Constants.ClaimTypes.Subject, "8F8EB0EC-6250-4A76-8B07-F28168574FF0"),
                        new Claim(Constants.ClaimTypes.Name, "Alberto Guardiola"),
                        new Claim(Constants.ClaimTypes.Role, AppRoles.Usuario),
                        new Claim(Constants.ClaimTypes.Role, AppRoles.GestorEmpleado),
                        new Claim(Constants.ClaimTypes.Id, "63")
                    }
                },
                new InMemoryUser
                {
                    Username = "aperezr",
                    Password = "aperezr",
                    Subject = "8F8EB0EC-6250-4A76-8B07-F28168574FF1",

                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.Email, "admin@test.com"),
                        new Claim(Constants.ClaimTypes.PreferredUserName, "aperezr"),
                        new Claim(Constants.ClaimTypes.Subject, "8F8EB0EC-6250-4A76-8B07-F28168574FF1"),
                        new Claim(Constants.ClaimTypes.Name, "Alvariño de la Pradera"),
                        new Claim(Constants.ClaimTypes.Role, AppRoles.Usuario),
                        new Claim(Constants.ClaimTypes.Id, "81")
                    }
                },
                new InMemoryUser
                {
                    Username = "JRCOGOLLUDO",
                    Password = "JRCOGOLLUDO",
                    Subject = "8F8EB0EC-6250-4A76-8B07-F28168574FF2",

                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.Email, "admin@test.com"),
                        new Claim(Constants.ClaimTypes.PreferredUserName, "JRCOGOLLUDO"),
                        new Claim(Constants.ClaimTypes.Subject, "8F8EB0EC-6250-4A76-8B07-F28168574FF2"),
                        new Claim(Constants.ClaimTypes.Name, "José Ramón Cogolludo"),
                        new Claim(Constants.ClaimTypes.Role, AppRoles.Usuario),
                        new Claim(Constants.ClaimTypes.Role, AppRoles.Administrador),
                        new Claim(Constants.ClaimTypes.Id, "62")
                    }
                },
                new InMemoryUser
                {
                    Username = "mjhernan",
                    Password = "mjhernan",
                    Subject = "8F8EB0EC-6250-4A76-8B07-F28168574FF3",

                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.Email, "admin@test.com"),
                        new Claim(Constants.ClaimTypes.PreferredUserName, "mjhernan"),
                        new Claim(Constants.ClaimTypes.Subject, "8F8EB0EC-6250-4A76-8B07-F28168574FF3"),
                        new Claim(Constants.ClaimTypes.Name, "Mari Jose Hernán"),
                        new Claim(Constants.ClaimTypes.Role, AppRoles.Usuario),
                        new Claim(Constants.ClaimTypes.Role, AppRoles.GestorEmpleado),
                        new Claim(Constants.ClaimTypes.Id, "71")
                    }
                },
                new InMemoryUser
                {
                    Username = "yllarena",
                    Password = "yllarena",
                    Subject = "8F8EB0EC-6250-4A76-8B07-F28168574FF4",

                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.Email, "admin@test.com"),
                        new Claim(Constants.ClaimTypes.PreferredUserName, "yllarena"),
                        new Claim(Constants.ClaimTypes.Subject, "8F8EB0EC-6250-4A76-8B07-F28168574FF4"),
                        new Claim(Constants.ClaimTypes.Name, "Yaiza Llarena"),
                        new Claim(Constants.ClaimTypes.Role, AppRoles.Usuario),
                        new Claim(Constants.ClaimTypes.Role, AppRoles.GestorEmpleado),
                        new Claim(Constants.ClaimTypes.Id, "66")
                    }
                }

            };
        }
    }
}