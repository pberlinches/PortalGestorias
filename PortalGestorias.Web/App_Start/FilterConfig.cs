using System.Web.Mvc;
using PortalGestorias.Web.Security;
using Thinktecture.IdentityModel.Mvc;

namespace PortalGestorias.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ErrorHandler.AiHandleErrorAttribute());
            filters.Add(new LoggedHandleErrorAttribute());
            filters.Add(new ResourceAuthorizeAttribute(DataOperation.Read, Controllers.Controllers.Home));
        }
    }
}
