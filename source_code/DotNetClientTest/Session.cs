using Dlubal.WS.Clients.DotNetClientTest.Tools;
using Dlubal.WS.Common.Tools;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using System.Web.Services.Discovery;

#if RFEM
using Dlubal.WS.Rfem6.Model;
using ApplicationClient = Dlubal.WS.Rfem6.Application.RfemApplicationClient;
using ModelClient = Dlubal.WS.Rfem6.Model.RfemModelClient;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
using ApplicationClient = Dlubal.WS.Rstab9.Application.RstabApplicationClient;
using ModelClient = Dlubal.WS.Rstab9.Model.RstabModelClient;
#elif RSECTION
using Dlubal.WS.RSection1.Model;
using ApplicationClient = Dlubal.WS.RSection1.Application.RSectionApplicationClient;
using ModelClient = Dlubal.WS.RSection1.Model.RSectionModelClient;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static class Session
    {
        public static readonly string DEFAULT_ADDRESS = Properties.Settings.Default.WebServiceAddress;

#if RFEM
        private const string SOAP_APPLICATION_WSDL_NAME = "RfemApplication.wsdl";
        private const int SOAP_PORT_FIRST_NUMBER = 8081;
        private const int SOAP_PORT_LAST_NUMBER = 8089;
#elif RSTAB
        private const string SOAP_APPLICATION_WSDL_NAME = "RstabApplication.wsdl";
        private const int SOAP_PORT_FIRST_NUMBER = 8091;
        private const int SOAP_PORT_LAST_NUMBER = 8099;
#elif RSECTION
        private const string SOAP_APPLICATION_WSDL_NAME = "RSectionApplication.wsdl";
        private const int SOAP_PORT_FIRST_NUMBER = 8101;
        private const int SOAP_PORT_LAST_NUMBER = 8109;
#endif

        public static EndpointAddress Address { get; set; } = new EndpointAddress(DEFAULT_ADDRESS);

        public static BasicHttpBinding Binding => new BasicHttpBinding("DlubalSoapBinding");

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
                        soapModelClient = new ModelClient(Binding, new EndpointAddress(endpoint));

#if RFEM || RSTAB
                        soapModelClient.get_object_count(object_types.E_OBJECT_TYPE_NODE, 0);
#elif RSECTION
                        soapModelClient.get_object_count(object_types.E_OBJECT_TYPE_POINT, 0);
#endif

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

                        Console.WriteLine($"FAILED: {exception.Message}");
                        DataLogger.ReportError(exception, Program.IsConsoleApplication);
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
                        soapModelClient.close_connection();
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
            DataLogger.AddLogStart($"Getting listening {Program.ApplicationName} application endpoints...");

            List<string> endpoints = new List<string>();

            try
            {
                DiscoveryClientProtocol discoveryClientProtocol = new DiscoveryClientProtocol();

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

                            DiscoveryDocument document = discoveryClientProtocol.DiscoverAny(url + "/wsdl");
                            if (document != null)
                            {
                                foreach (DiscoveryReference reference in document.References)
                                {
                                    DataLogger.AddText($"Document name = {reference.DefaultFilename}");

                                    if (reference.DefaultFilename.Equals(SOAP_APPLICATION_WSDL_NAME, StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        endpoints.Add(url);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            DataLogger.AddException(ex.Message);
                        }
                    }
                }

                discoveryClientProtocol.Dispose();
                discoveryClientProtocol = null;

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
                    SoapApplicationClient = new ApplicationClient(Binding, Address);
                }

                Console.Error.WriteLine(Utilities.ParseException(exception));
                DataLogger.ReportError(exception, true);
            }
            else if (exception is CommunicationException)
            {
                if (SoapApplicationClient != null)
                {
                    SoapApplicationClient.Abort();
                    SoapApplicationClient = null;
                }

                Console.Error.WriteLine(Utilities.ParseException(exception));
                DataLogger.ReportError(exception, true);
            }
            else
            {
                if (SoapApplicationClient != null)
                {
                    SoapApplicationClient.Abort();
                    SoapApplicationClient = null;
                }

                Console.Error.WriteLine(Utilities.ParseException(exception));
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

                Console.Error.WriteLine(Utilities.ParseException(exception));
                DataLogger.ReportError(exception, true);
            }
            else if (exception is CommunicationException)
            {
                if (soapModelClient != null)
                {
                    soapModelClient.Abort();
                    soapModelClient = null;
                }

                Console.Error.WriteLine(Utilities.ParseException(exception));
                DataLogger.ReportError(exception, true);
            }
            else
            {
                if (soapModelClient != null)
                {
                    soapModelClient.Abort();
                    soapModelClient = null;
                }

                Console.Error.WriteLine(Utilities.ParseException(exception));
                DataLogger.ReportError(exception, true);
            }
        }

        public static bool RunTests(List<System.Reflection.MethodInfo> testList, out string textResult, Action<string> reportMethod = null, bool getTextResult = false)
        {
            textResult = string.Empty;

            DataLogger.AddText($"{testList.Count} tests to pass");
            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            DataLogger.AddSeparator();

            if (testList.Count == 0)
            {
                return false;
            }

            DataLogger.AddLogStart("Passing through checked tests...");

            Dictionary<string, TestingMethods.TestResultType> resultDictionary = new Dictionary<string, TestingMethods.TestResultType>();
            int maxLength = 0;

            foreach (System.Reflection.MethodInfo method in testList)
            {
                string methodName = method.ConvertToString();

                if (SoapApplicationClient == null)
                {
                    DataLogger.AddLogStart(methodName + "...");
                    DataLogger.ReportError($"Web Service Client Proxy is disconnected from server!{Environment.NewLine}Please try to reconnect it.", true);
                    DataLogger.AddSeparator();
                    resultDictionary.Add(methodName, TestingMethods.TestResultType.SKIPPED);
                    continue;
                }

                reportMethod?.Invoke($"Testing method '{methodName}'...");

                bool result = (bool)method.Invoke(null, null);

                reportMethod?.Invoke("..." + (result ? TestingMethods.TestResultType.PASSED : TestingMethods.TestResultType.FAILED));

                resultDictionary.Add(methodName, result ? TestingMethods.TestResultType.PASSED : TestingMethods.TestResultType.FAILED);

                maxLength = Math.Max(maxLength, methodName.Length);
                DataLogger.AddSeparator();
            }

            reportMethod?.Invoke(string.Empty);

            DataLogger.AddText("Results of tests:");

            DataLogger.LogResultType testResult = DataLogger.LogResultType.DONE;

            StringBuilder stringBuilder = new StringBuilder();

            foreach (KeyValuePair<string, TestingMethods.TestResultType> result in resultDictionary)
            {
                string resultLine = $"{result.Key.PadRight(maxLength)} : {result.Value}";
                DataLogger.AddText($"  {resultLine}");

                if (getTextResult)
                {
                    stringBuilder.AppendLine(resultLine);
                }

                if ((result.Value == TestingMethods.TestResultType.FAILED) ||
                    (result.Value == TestingMethods.TestResultType.SKIPPED))
                {
                    testResult = DataLogger.LogResultType.FAILED;
                }
            }

            DataLogger.AddLogEnd(testResult);
            DataLogger.AddSeparator();

            textResult = stringBuilder.ToString();

            return testResult == DataLogger.LogResultType.DONE;
        }
    }
}