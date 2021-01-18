using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using PortalGestorias.Domain.Entities;
using PortalGestorias.Infrastructure.Data;
using PortalGestorias.Web.App_Start;
using PortalGestorias.Web.Models;
using Serilog;
using System;
using System.Web.Http;

namespace PortalGestorias.Web
{
    public class MvcApplication : HttpApplication
    {
        void Session_Start() { }

        protected void Application_Start()
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .CreateLogger();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(RouteConfig.ApiRoutes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ModelBinders.Binders.Add(typeof(DatoMaestro), new DatoMaestroModelBinder(() => WindsorActivator.Container.Resolve<CrmDbContext>()));
            ModelBinders.Binders.Add(typeof(DateTime), new CustomDateTimeModelBinder());
            //ModelBinders.Binders.Add(typeof(Empleado), new EmpleadoModelBinder(() => WindsorActivator.Container.Resolve<CrmDbContext>()));
            //ModelBinders.Binders.Add(typeof(Departamento), new DepartamentoModelBinder(() => WindsorActivator.Container.Resolve<CrmDbContext>()));
        }
    }
}
