using DotNetCoreClientTest.Web2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DotNetCoreClientTest.Web2.Code.WSconsumer
{
    public class ConnectionTest
    {
        public TestWSModel TestConnectionWS()
        {
            TestWSModel model = new TestWSModel();

            var wc = new System.Net.WebClient();
            string data = wc.DownloadString(MyConfig.DefaultAddress + "/wsdl");

            Console.WriteLine("Test load data from URI.");
            if (data.Length > 100)
            {
                Console.WriteLine(data.Substring(0, 100));
                Console.WriteLine("...");
                model.Value1 = data.Substring(0, 100);
            }
            else
            {
                model.ErrorMsg = "WS data is too short.";
                Console.WriteLine("WS data is too short. ");
                Console.WriteLine(data);
            }
            // test 2
            Console.WriteLine("Test response from WS.");
            string postData = "<x:Envelope xmlns:x=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:rfe=\"http://www.dlubal.com/rfem.xsd\"> \n";
            postData += "<x:Header/>\n";
            postData += "  <x:Body>\n";
            postData += "    <rfe:get_information></rfe:get_information>\n";
            postData += "  </x:Body>\n";
            postData += "</x:Envelope>\n";

            var client = new HttpClient();
            var httpResponse = client.PostAsync(MyConfig.DefaultAddress, new StringContent(postData, System.Text.Encoding.UTF8, "text/xml")).Result;
            var data2 = httpResponse.Content.ReadAsStringAsync().Result;
            model.Value2 = httpResponse.Content.ReadAsStringAsync().Result;

            Console.WriteLine(data2);
            return model;
        }
    }
}
