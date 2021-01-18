using System.Security.Cryptography.X509Certificates;

namespace PortalGestorias.Infrastructure.Security
{
    public interface ICertificateService
    {
        X509Certificate2 Get();
    }
}