using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace PortalGestorias.Infrastructure.Security
{
    public class CertificateStoreService : CertificateService
    {
        public CertificateStoreService(string keyName) : base(keyName)
        {
        }

        public override X509Certificate2 Get()
        {
            var store = LoadStoreName(KeyName);
            var location = LoadStoreLocation(KeyName);
            var thumbprint = LoadThumbprint(KeyName);

            var certificateStore = new X509Store(store, location);
            certificateStore.Open(OpenFlags.ReadOnly);
            var certificateCollection = certificateStore.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
            certificateStore.Close();

            X509Certificate2 result;
            switch (certificateCollection.Count)
            {
                case 0:
                    throw new ArgumentException($"Could not load certificate: {thumbprint}, not found");
                case 1:
                    result = certificateCollection[0];
                    break;
                default:
                    throw new ArgumentException($"Could not load certificate: {thumbprint}, multiple certificates found");
            }

            return result;
        }

        private static StoreName LoadStoreName(string type)
        {
            var storeNameString = ConfigurationManager.AppSettings[$"{type}:{Certificates.StoreType}:{Certificates.StoreNameKey}"];
            var storeName = (StoreName)Enum.Parse(typeof(StoreName), storeNameString);
            return storeName;
        }

        private static StoreLocation LoadStoreLocation(string type)
        {
            var storeLocationString = ConfigurationManager.AppSettings[$"{type}:{Certificates.StoreType}:{Certificates.StoreLocationKey}"];
            var storeLocation = (StoreLocation)Enum.Parse(typeof(StoreLocation), storeLocationString);
            return storeLocation;
        }

        private static string LoadThumbprint(string type)
        {
            var thumbprint = ConfigurationManager.AppSettings[$"{type}:{Certificates.StoreType}:{Certificates.ThumbprintKey}"];
            return thumbprint.ToUpper();
        }
    }
}
