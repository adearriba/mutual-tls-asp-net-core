using MutualTLS.Client.Clients;
using RestSharp;
using System;

namespace MutualTLS.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var clientWithCert = new ClientWithCertificate();
            var clientWithoutCert = new ClientWithoutCertificate();

            var request = new RestRequest("MTLS", Method.GET);

            var resWithCert = clientWithCert.Execute(request);
            var resWithoutCert = clientWithoutCert.Execute(request);

            Console.WriteLine($"With Certificate: Status code {resWithCert.StatusCode}");
            Console.WriteLine($"Without Certificate: Status code {resWithoutCert.StatusCode}");

            Console.ReadLine();
        }
    }
}
