using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace PortalGestorias.Infrastructure.Security
{
    public class CertificateLocalService : CertificateService
    {
        public CertificateLocalService(string keyName) : base(keyName)
        {
        }

        public override X509Certificate2 Get()
        {
            var path = LoadPath(KeyName);
            var password = LoadPassword(KeyName);

            var certificate = new X509Certificate2(path, password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            return certificate;
        }

        public virtual string GetPhysicalPath(string path)
        {
            return path;
        }

        private string LoadPath(string type)
        {
            var path = ConfigurationManager.AppSettings[$"{type}:{Certificates.LocalType}:{Certificates.PathKey}"];
            return GetPhysicalPath(path);
        }

        private static string LoadPassword(string type)
        {
            var password = ConfigurationManager.AppSettings[$"{type}:{Certificates.LocalType}:{Certificates.PasswordKey}"];
            return password;
        }
    }
}