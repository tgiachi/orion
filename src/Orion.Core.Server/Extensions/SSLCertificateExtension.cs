using System.Security.Cryptography.X509Certificates;
using Orion.Core.Server.Data.Config.Sections;

namespace Orion.Core.Server.Extensions;

public static class SSLCertificateExtension
{
    public static X509Certificate2 LoadCertificate(this SSLConfig sslConfig)
    {
        if (string.IsNullOrWhiteSpace(sslConfig.CertificatePath))
        {
            throw new ArgumentException("Certificate path is required.");
        }

        return X509Certificate2.CreateFromPemFile(sslConfig.CertificatePath, sslConfig.Password);
    }
}
