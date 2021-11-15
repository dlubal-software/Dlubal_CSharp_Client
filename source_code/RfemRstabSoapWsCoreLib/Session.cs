using Dlubal.WS.RfemRstabSoapWsCoreLib.Tools;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using System.Net.Http;
using System.Xml;



#if RFEM
using Dlubal.WS.Rfem6.Model;
using ApplicationClient = Dlubal.WS.Rfem6.Application.RfemApplicationClient;
using ModelClient = Dlubal.WS.Rfem6.Model.RfemModelClient;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
using ApplicationClient = Dlubal.WS.Rstab9.Application.RstabApplicationClient;
using ModelClient = Dlubal.WS.Rstab9.Model.RstabModelClient;
#endif

namespace Dlubal.WS.RfemRstabSoapWsCoreLib
{
    public static class Session
    {

#if RFEM
        private const string SOAP_APPLICATION_WSDL_NAME = "RfemApplication.wsdl";
        private const int SOAP_PORT_FIRST_NUMBER = 8081;
        private const int SOAP_PORT_LAST_NUMBER = 8089;
#elif RSTAB
        private const string SOAP_APPLICATION_WSDL_NAME = "RstabApplication.wsdl";
        private const int SOAP_PORT_FIRST_NUMBER = 8091;
        private const int SOAP_PORT_LAST_NUMBER = 8099;
#endif

        public static EndpointAddress Address { get; set; } = new EndpointAddress(ConfigLibrary.DefaultAddress);

        //public static BasicHttpBinding Binding => new BasicHttpBinding("DlubalSoapBinding");

        public static BasicHttpBinding Binding = new BasicHttpBinding()
        {
            Name = ConfigLibrary.BindingWSProperties.Name,
            MaxReceivedMessageSize = ConfigLibrary.BindingWSProperties.MaxReceivedMessageSize,
            SendTimeout = new TimeSpan(0, ConfigLibrary.BindingWSProperties.SendTimeoutMinutes, 0),
        };

        // Reference to logging tool.
        public static DataLogger DataLogger { get; set; } = DataLogger.DummyDataLogger;

        // Reference to application client proxy object.
        public static ApplicationClient SoapApplicationClient { get; set; } = null;

        // Reference to model client proxy object.
        private static ModelClient soapModelClient;
        public static ModelClient SoapModelClient
        {
            get
            {
                if (soapModelClient == null)
                {
                    DataLogger.AddLogStart("Initialization of Model WS Client Proxy...");

                    string endpoint;

                    try
                    {
                        endpoint = SoapApplicationClient.get_active_model();
                    }
                    catch (Exception exception)
                    {
                        ApplicationWsExceptionHandler(exception);
                        return null;
                    }

                    try
                    {
                        /*
                        BasicHttpBinding Binding2 = new BasicHttpBinding();
                        Binding2.Security.Mode = BasicHttpSecurityMode.None;
                        Binding2.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                        Binding2.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;

                        Binding2.Name = Settings.BindingWSProperties.Name;
                        Binding2.MaxReceivedMessageSize = 10000000;
                        Binding2.SendTimeout = new TimeSpan(0, Settings.BindingWSProperties.SendTimeoutMinutes, 0);
                        Binding2.OpenTimeout = new TimeSpan(0, 10, 0);
                        Binding2.ReceiveTimeout = new TimeSpan(0, 10, 0);
                        Binding2.CloseTimeout = new TimeSpan(0, 10, 0);
                        Binding2.MaxBufferSize = 10000000;
                        Binding2.MaxBufferPoolSize = 10000000;
                        */

                        soapModelClient = new ModelClient(Binding, new EndpointAddress(ConfigLibrary.DefaultAddressModelClient));
                        soapModelClient.get_object_count(object_types.E_OBJECT_TYPE_NODE, 0);
                        DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
                    }
                    catch (Exception exception)
                    {
                        if (soapModelClient != null)
                        {
                            if (soapModelClient.State != CommunicationState.Faulted)
                            {
                                soapModelClient.Close();
                            }
                            else
                            {
                                soapModelClient.Abort();
                            }

                            soapModelClient = null;
                        }

                        DataLogger.ReportError(exception);
                        DataLogger.AddSeparator();
                    }
                }

                return soapModelClient;
            }

            set
            {
                soapModelClient = value;
            }
        }

        public static object MySettings { get; private set; }

        public static void CloseApplicationClient()
        {
            if (SoapApplicationClient != null)
            {
                try
                {
                    if (SoapApplicationClient.State != CommunicationState.Faulted)
                    {
                        SoapApplicationClient.Close();
                    }
                    else
                    {
                        SoapApplicationClient.Abort();
                    }
                }
                catch (Exception)
                {
                    SoapApplicationClient.Abort();
                }
                finally
                {
                    SoapApplicationClient = null;
                }
            }
        }

        public static void CloseModelClient()
        {
            if (soapModelClient != null)
            {
                try
                {
                    if (soapModelClient.State != CommunicationState.Faulted)
                    {
                        soapModelClient.Close();
                    }
                    else
                    {
                        soapModelClient.Abort();
                    }
                }
                catch (Exception)
                {
                    soapModelClient.Abort();
                }
                finally
                {
                    soapModelClient = null;
                }
            }
        }

        public static void CloseSoapClients()
        {
            CloseModelClient();
            CloseApplicationClient();
        }

        /// <summary>
        /// Gets list of listening RFEM6/RSTAB9 application endpoints.
        /// </summary>
        /// <returns>List of endpoints</returns>
        public static string[] GetEndpoints()
        {
            DataLogger.AddLogStart($"Getting listening application endpoints...");

            List<string> endpoints = new List<string>();

            try
            {
                IPEndPoint[] tcpListeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
                foreach (IPEndPoint endPoint in tcpListeners)
                {
                    // v ramci testovani zatim hledame pouze v IPv4 rozsahu portu 8081 - 8089 pro RFEM6 nebo 8091 - 8099 pro RSTAB8
                    if (endPoint.AddressFamily == AddressFamily.InterNetwork && endPoint.Port >= SOAP_PORT_FIRST_NUMBER && endPoint.Port <= SOAP_PORT_LAST_NUMBER)
                    {
                        DataLogger.AddText($"Address = {endPoint.Address}, Port = {endPoint.Port}");

                        try
                        {
                            string url;
                            if (endPoint.Address.Equals(IPAddress.Any))
                            {
                                url = $"http://{IPAddress.Loopback}:{endPoint.Port}";
                            }
                            else
                            {
                                url = $"http://{endPoint.Address}:{endPoint.Port}";
                            }

                            var wc = new System.Net.WebClient();
                            string data = wc.DownloadString(url + "/wsdl");  //TODO: dodelat

                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(data);  // je to XML

                            if (url == ConfigLibrary.DefaultAddress)
                            {
                               endpoints.Add(url);
                            }

                            //TODO: zjistit, zda to staci


                            //XmlNode root = doc.FirstChild;
                            //then output it

                            /*
                            string name = root["definitions"].Attributes["name"].InnerText;

                            DataLogger.AddText($"Document name = {reference.DefaultFilename}");

                            endpoints.Add(url);


                            discoverydocument document = discoveryclientprotocol.discoverany(url + "/wsdl");
                            if (document != null)
                            {
                                foreach (discoveryreference reference in document.references)
                                {
                                    datalogger.addtext($"document name = {reference.defaultfilename}");

                                    if (reference.defaultfilename.equals(soap_application_wsdl_name, stringcomparison.currentcultureignorecase))
                                    {
                                        endpoints.add(url);
                                    }
                                }
                            }
                            */
                        }
                        catch (Exception ex)
                        {
                            DataLogger.AddException(ex.Message);
                        }
                    }
                }

                DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            }
            catch (Exception exception)
            {
                DataLogger.ReportError(exception);
            }

            DataLogger.AddSeparator();
            return endpoints.ToArray();
        }

        public static void ApplicationWsExceptionHandler(Exception exception)
        {
            if ((exception is TimeoutException) || (exception is FaultException))
            {
                if (SoapApplicationClient != null && SoapApplicationClient.State != CommunicationState.Opened)
                {
                    SoapApplicationClient.Abort();
                    SoapApplicationClient = new ApplicationClient(Binding, Address); //TODO: zkontolovat
                }

                DataLogger.ReportError(exception, true);
            }
            else if (exception is CommunicationException)
            {
                if (SoapApplicationClient != null)
                {
                    SoapApplicationClient.Abort();
                    SoapApplicationClient = null;
                }

                DataLogger.ReportError(exception, true);
            }
            else
            {
                if (SoapApplicationClient != null)
                {
                    SoapApplicationClient.Abort();
                    SoapApplicationClient = null;
                }

                DataLogger.ReportError(exception, true);
            }
        }

        public static void ModelWsExceptionHandler(Exception exception)
        {
            if ((exception is TimeoutException) || (exception is FaultException))
            {
                if (soapModelClient != null && soapModelClient.State != CommunicationState.Opened)
                {
                    soapModelClient.Abort();
                    soapModelClient = null;
                }

                DataLogger.ReportError(exception, true);
            }
            else if (exception is CommunicationException)
            {
                if (soapModelClient != null)
                {
                    soapModelClient.Abort();
                    soapModelClient = null;
                }

                DataLogger.ReportError(exception, true);
            }
            else
            {
                if (soapModelClient != null)
                {
                    soapModelClient.Abort();
                    soapModelClient = null;
                }

                DataLogger.ReportError(exception, true);
            }
        }
    }
}