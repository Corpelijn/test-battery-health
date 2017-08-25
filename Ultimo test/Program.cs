using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ultimo_test.Ultimo;

namespace Ultimo_test
{
    class Program
    {
        static void Main(string[] args)
        {
            SoapConnectorClient client = new SoapConnectorClient("BasicHttpBinding_SoapConnector");
            client.Endpoint.Address = new System.ServiceModel.EndpointAddress("http://servicedesk/webservices/soapconnector.svc");
            client.ImportData("JOB_IMPORT", "importjob", "importjob", System.IO.File.ReadAllText(@"C:\Users\Beheerder\data.xml"));
            client.Close();
        }
    }
}