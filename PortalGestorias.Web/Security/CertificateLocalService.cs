using System.Web.Hosting;

namespace PortalGestorias.Web.Security
{
    public class CertificateLocalService : Infrastructure.Security.CertificateLocalService
    {
        public CertificateLocalService(string keyName) : base(keyName)
        {
        }

        public override string GetPhysicalPath(string path)
        {
            return System.IO.Path.Combine(HostingEnvironment.ApplicationPhysicalPath, path);
        }
    }

}