using System.Security.Cryptography.X509Certificates;

namespace Orion.Core.Utils;

public static class SSLUtils
{
    public static X509Certificate2 LoadCertificate(string fileName, string? password = null)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("Certificate path is required.");
        }

        return X509Certificate2.CreateFromPemFile(fileName, password);

    }
}
