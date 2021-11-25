using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DotNetCoreClientTest.Web2
{
    public class MyConfig
    {
        public static string DefaultAddress { get; private set; }
        public static string DefaultAddressModelClient { get; private set; }
        
        public static string DataFolder { get; private set; }
        public static string ObjectInformationFile { get; private set; }
        public static BindingWS BindingWSProperties { get; private set; }
        public static List<string> DisableTest { get; private set; }
        public static string LogPath { get; private set; }

        public MyConfig(IConfiguration Configuration)
        {
            
            DefaultAddress = Configuration.GetValue<string>("WebAppConfig:DefaultAddress"); //builder.Build().GetValue<string>("DefaultAddress");
            DefaultAddressModelClient = Configuration.GetValue<string>("WebAppConfig:DefaultAddressModelClient");
            DataFolder = Configuration.GetValue<string>("WebAppConfig:DataFolder");
            ObjectInformationFile = Configuration.GetValue<string>("WebAppConfig:ObjectInformationFile");
 
            DisableTest = Configuration.GetSection("WebAppConfig:DisableTest").Get<string[]>().ToList();
            LogPath = Configuration.GetValue<string>("WebAppConfig:LogPath");

            BindingWSProperties = Configuration.GetSection("WebAppConfig:BindingWS").Get<BindingWS>();

            // load config to lib
            Dlubal.WS.RfemRstabSoapWsCoreLib.ConfigLibrary.Load(DefaultAddress, DefaultAddressModelClient, DataFolder, LogPath, ObjectInformationFile, BindingWSProperties.Name, BindingWSProperties.MaxReceivedMessageSize, BindingWSProperties.SendTimeoutMinutes);
        }

        public class BindingWS
        {
            public string Name { get; set; }
            public int MaxReceivedMessageSize { get; set; }
            public int SendTimeoutMinutes { get; set; }
        }

    }
}
