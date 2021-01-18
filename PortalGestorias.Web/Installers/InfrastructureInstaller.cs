using System;
using System.Configuration;
using System.Security.Principal;
using System.Web;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using PortalGestorias.Business.Validations;
using PortalGestorias.Infrastructure.Data;
using PortalGestorias.Infrastructure.Security;
using PortalGestorias.Web.Security;
using CertificateLocalService = PortalGestorias.Web.Security.CertificateLocalService;
using PortalGestorias.Business.Actions;
using PortalGestorias.Business.Services;

namespace PortalGestorias.Web.Installers
{
    public class InfrastructureInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AddFacility<TypedFactoryFacility>();
            container.Register(Component.For<CrmDbContext>().LifestylePerWebRequest());
            container.Register(Component.For<IIdentity>().UsingFactoryMethod(() => HttpContext.Current.User==null ? WindowsIdentity.GetCurrent() : HttpContext.Current.User.Identity).LifestylePerWebRequest());
            container.Register(Classes
                                .FromAssembly(typeof(ActionsBase<>).Assembly)
                                .BasedOn(typeof(ActionsBase<>))
                                .WithServiceSelf()
                                .LifestylePerWebRequest());
            container.Register(Classes
                    .FromAssembly(typeof(MailService).Assembly)
                    .BasedOn(typeof(IService))
                    .WithServiceSelf()
                    .LifestylePerWebRequest());
            container.Register(Classes
                                .FromAssembly(typeof(IEntityValidator<>).Assembly)
                                .BasedOn(typeof(IEntityValidator<>))
                                .WithServiceAllInterfaces()
                                .LifestylePerWebRequest());
            container.Register(Component.For<ISecurityManagerService>().ImplementedBy<ADSecurityManagerService>().LifestyleTransient());
            container.Register(Component.For<ICertificateService>().UsingFactoryMethod(() =>
            {
                ICertificateService result;
                switch (ConfigurationManager.AppSettings[$"IdSrv:{Certificates.CertificateServiceTypeKey}"])
                {
                    case Certificates.LocalType:
                        result = new CertificateLocalService("IdSrv");
                        break;

                    case Certificates.StoreType:
                        result = new CertificateStoreService("IdSrv");
                        break;

                    default:
                        throw new Exception("Invalid Configuration for signing certificate");
                }

                return result;
            }));
        }
    }
}