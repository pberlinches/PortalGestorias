using Castle.Windsor;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(PortalGestorias.Web.App_Start.WindsorActivator), "PreStart")]
[assembly: ApplicationShutdownMethodAttribute(typeof(PortalGestorias.Web.App_Start.WindsorActivator), "Shutdown")]

namespace PortalGestorias.Web.App_Start
{
    public static class WindsorActivator
    {
        static ContainerBootstrapper bootstrapper;

        public static IWindsorContainer Container
        {
            get { return bootstrapper.Container; }
        }

        public static void PreStart()
        {
            bootstrapper = ContainerBootstrapper.Bootstrap();
        }
        
        public static void Shutdown()
        {
            if (bootstrapper != null)
                bootstrapper.Dispose();
        }
    }
}