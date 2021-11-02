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
    public class ClientWithoutCertificate : IClient
    {
        private IRestClient _client = null;

        public ClientWithoutCertificate()
        {
            _client = new RestClient("https://localhost:5001");
        }

        public IRestResponse Execute(IRestRequest request)
        {
            return _client.Execute(request);
        }
    }
}
