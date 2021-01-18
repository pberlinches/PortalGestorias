using System.Security.Cryptography.X509Certificates;

namespace PortalGestorias.Infrastructure.Security
{
    public abstract class CertificateService : ICertificateService
    {
        protected string KeyName;

        protected CertificateService(string keyName)
        {
            KeyName = keyName;
        }

        public abstract X509Certificate2 Get();
    }
}