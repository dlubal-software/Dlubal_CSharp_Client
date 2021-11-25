using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public class Settings
    {
        public static string DefaultAddress { get; private set; }
        public static string DefaultAddressModelClient { get; private set; }
        
        public static string DataFolder { get; private set; }
        public static string ObjectInformationFile { get; private set; }
        public static string LogPath { get; private set; }

        public static BindingWS BindingWSProperties { get; private set; }
        public static List<string> DisableTest { get; private set; }

        public Settings()
        {
            var path = Directory.GetCurrentDirectory();
            Console.WriteLine($"config file {path}");

            var builder = new ConfigurationBuilder()
//             . SetBasePath(Directory.GetCurrentDirectory() + "/DotNetCoreClientTest/")        //for Visual Studio Code
               .SetBasePath(Directory.GetCurrentDirectory())                                    //for regular Visual Studio
               .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            DefaultAddress = builder.Build().GetValue<string>("DefaultAddress");
            DefaultAddressModelClient = builder.Build().GetValue<string>("DefaultAddressModelClient");
            DataFolder = builder.Build().GetValue<string>("DataFolder");
            ObjectInformationFile = builder.Build().GetValue<string>("ObjectInformationFile");
            DisableTest = builder.Build().GetSection("DisableTest").Get<string[]>().ToList();

            LogPath = builder.Build().GetValue<string>("LogPath");

            BindingWSProperties = config.GetSection("BindingWS").Get<BindingWS>();

            // load config to lib
            Dlubal.WS.RfemRstabSoapWsCoreLib.ConfigLibrary.Load(DefaultAddress, DefaultAddressModelClient, DataFolder, LogPath, ObjectInformationFile, BindingWSProperties.Name, BindingWSProperties.MaxReceivedMessageSize, BindingWSProperties.SendTimeoutMinutes);

        }

        public class BindingWS
        {
            public string Name { get; set; }
            public int MaxReceivedMessageSize { get; set; }
            public int SendTimeoutMinutes { get; set; }
        }

        public class Test
        {
            public List<string> DisableTest { get; set; }
        }

    }
}
