namespace Dlubal.WS.RfemRstabSoapWsCoreLib
{
    public class ConfigLibrary
    {
        public static string DefaultAddress { get; private set; }
        public static string DefaultAddressModelClient { get; private set; }

        public static string DataFolder { get; private set; }
        public static string ObjectInformationFile { get; private set; }
        public static BindingWS BindingWSProperties { get; private set; }
        public static string LogPath { get; private set; }

        /// <summary>
        /// Load configuration.
        /// </summary>
        /// <param name="defaultAddress">eg. http://localhost:8081</param>
        /// <param name="defaultAddressModelClient">eg. http://localhost:8082 </param>
        /// <param name="dataFolder">eg. Data</param>
        /// <param name="logPath">eg. C:\temp\WSlog\</param>
        /// <param name="objectInformationFile">eg. ObjectInformation.xml</param>
        /// <param name="bindingName">eg. DlubalSoapBinding</param>
        /// <param name="bindingMaxReceivedMessageSize">eg. 10000000</param>
        /// <param name="bindingSendTimeoutMinutes">eg. 10</param>
        public static void Load(string defaultAddress, string defaultAddressModelClient, string dataFolder, string logPath, string objectInformationFile, string bindingName, int bindingMaxReceivedMessageSize, int bindingSendTimeoutMinutes)
        {
            DefaultAddress = defaultAddress;
            DefaultAddressModelClient = defaultAddressModelClient;
            DataFolder = dataFolder;
            ObjectInformationFile = objectInformationFile;

            LogPath = logPath;

            BindingWS bindingWS = new BindingWS();
            bindingWS.MaxReceivedMessageSize = bindingMaxReceivedMessageSize;
            bindingWS.Name = bindingName;
            bindingWS.SendTimeoutMinutes = bindingSendTimeoutMinutes;

            BindingWSProperties = bindingWS;
        }
        public class BindingWS
        {
            public string Name { get; set; }
            public int MaxReceivedMessageSize { get; set; }
            public int SendTimeoutMinutes { get; set; }
        }    
    }
}
