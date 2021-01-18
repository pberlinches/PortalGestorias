using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using PortalGestorias.Web.Security;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;

namespace PortalGestorias.Web
{
    public class AuthorizationManager : ResourceAuthorizationManager
    {
        private static List<string> readAllResources = new List<string>(){
            Controllers.Controllers.Home,
            Controllers.Controllers.HomeActions.Signin,
            Controllers.Controllers.HomeActions.Index,
        };

        private static List<string> editAllResources = new List<string>(){
            Controllers.Controllers.Home,
            Controllers.Controllers.HomeActions.Signin,
            Controllers.Controllers.HomeActions.Index,
        };

        private static List<string> manageEmpleados = new List<string>(){            
            Controllers.Controllers.Empleados,
            Controllers.Controllers.Estados,
            Controllers.Controllers.Marcas,
            Controllers.Controllers.Modelos,
            Controllers.Controllers.TiposModelos,
            Controllers.Controllers.Ubicaciones,
        };

        private static List<string> manageModificacion = new List<string>(){
            Controllers.Controllers.Productos,
        };

        public override Task<bool> CheckAccessAsync(ResourceAuthorizationContext context)
        {
            var action = context.Action.FirstOrDefault();
            var resource = context.Resource.FirstOrDefault()?.Value;
            if (action == null || !context.Principal.Identity.IsAuthenticated)
            {
                return Nok();
            }

            switch (action.Value)
            {
                case DataOperation.Read:
                    return Eval(UserCanRead(context.Principal, resource));
                case DataOperation.Create:
                case DataOperation.Update:
                    return Eval(UserCanCreate(context.Principal, resource));
                case DataOperation.Delete:
                    return Eval(UserCanDelete(context.Principal, resource));
            }

            return Nok();
        }

        private bool UserCanRead(ClaimsPrincipal principal, string resource)
        {
            if (editAllResources.Contains(resource)) return true;

            var userRoles = principal.Claims.Where(c => c.Type == IdentityServer3.Core.Constants.ClaimTypes.Role).ToList();

            var userCanRead = false;

            if (readAllResources.Contains(resource)) userCanRead = true;

            if (manageEmpleados.Contains(resource) && userRoles.Any(r => r.Value.Equals(AppRoles.GestorEmpleado))) userCanRead = true;

            if (userRoles.Any(r => r.Value.Equals(AppRoles.GestorConsulta))) userCanRead = true;

            if (userRoles.Any(r => r.Value.Equals(AppRoles.GestorModificacion))) userCanRead = true;

            return userCanRead;
        }

        private bool UserCanCreate(ClaimsPrincipal principal, string resource)
        {
            var roles = principal.Claims
                .Where(c => c.Type == IdentityServer3.Core.Constants.ClaimTypes.Role)
                .Select(c => c.Value).ToList();

            if (editAllResources.Contains(resource)) return true;


            var validRoles = new List<string> { };

            if (manageEmpleados.Contains(resource))
            {
                validRoles.Add(AppRoles.GestorEmpleado);
            }

            if (manageModificacion.Contains(resource))
            {
                validRoles.Add(AppRoles.GestorModificacion);
            }

            var userCanCreate = roles.Any(s => validRoles.Contains(s));

            return userCanCreate;
        }

        private bool UserCanDelete(ClaimsPrincipal principal, string resource)
        {
            var userRoles = principal.Claims
                .Where(c => c.Type == IdentityServer3.Core.Constants.ClaimTypes.Role)
                .Select(c=>c.Value).ToList();

            if (editAllResources.Contains(resource)) return true;

            var validRoles = new List<string> {  };

            if (manageEmpleados.Contains(resource))
            {
                validRoles.Add(AppRoles.GestorEmpleado);
            }

            if (manageModificacion.Contains(resource))
            {
                validRoles.Add(AppRoles.GestorModificacion);
            }

            var userCanDelete = userRoles.Any(s => validRoles.Contains(s));

            return userCanDelete;
        }

    }
}