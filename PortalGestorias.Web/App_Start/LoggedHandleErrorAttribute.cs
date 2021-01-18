using System.Web.Mvc;
using Serilog;

namespace PortalGestorias.Web
{
    public class LoggedHandleErrorAttribute : HandleErrorAttribute
    {
        private readonly Serilog.ILogger log = Log.ForContext("PortalGestorias", null);

        public override void OnException(ExceptionContext filterContext)
        {
            log.Error(filterContext.Exception, "Unhandled error caught.");
            base.OnException(filterContext);
        }
    }
}