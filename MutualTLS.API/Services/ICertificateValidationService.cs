using System.Security.Cryptography.X509Certificates;

namespace MutualTLS.API.Services
{
    public interface ICertificateValidationService
    {
        bool IsValid(X509Certificate2 certificate);
    }
}
