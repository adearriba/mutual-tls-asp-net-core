using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MutualTLS.Client.Clients
{
    public class ClientWithCertificate : IClient
    {
        private IRestClient _client = null;

        public ClientWithCertificate()
        {
            _client = new RestClient("https://localhost:5001");

            var currentDirectory = Directory.GetCurrentDirectory();
            var pfx = $"{currentDirectory}\\Certificates\\localhost-mtls.pfx";

            var cert = new X509Certificate2(pfx);

            _client.ClientCertificates = new X509CertificateCollection { cert };
        }

        public IRestResponse Execute(IRestRequest request)
        {
            return _client.Execute(request);
        }
    }
}
