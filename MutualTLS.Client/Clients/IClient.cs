using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MutualTLS.Client.Clients
{
    public interface IClient
    {
        IRestResponse Execute(IRestRequest request);
    }
}
