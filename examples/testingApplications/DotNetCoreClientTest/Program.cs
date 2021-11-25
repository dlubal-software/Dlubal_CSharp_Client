using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using Dlubal.WS.RfemRstabSoapWsCoreLib;
using System.ServiceModel;
using Dlubal.WS.RfemRstabSoapWsCoreLib.Tools;
using System.Text;

#if RFEM
using Dlubal.WS.Rfem6.Application;
using ApplicationClient = Dlubal.WS.Rfem6.Application.RfemApplicationClient;
#elif RSTAB
using Dlubal.WS.Rstab9.Application;
using ApplicationClient = Dlubal.WS.Rstab9.Application.RstabApplicationClient;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    class Program
    {

#if RFEM
        public const string ApplicationName = "RFEM6";
#elif RSTAB
        public const string ApplicationName = "RSTAB9";
#endif
        static void Main(string[] args)
        {

            Console.WriteLine("Run");

            Console.WriteLine("Load settings");
            new Settings(); // load setting from config file
            Console.Write("Version 0.3 ");

            RfemSoapWsCoreClient.Info infoRfem = new RfemSoapWsCoreClient.Info();

            RstabSoapWsCoreClient.Info infoRstab = new RstabSoapWsCoreClient.Info();
            Console.Write($"Client {infoRstab.Verze()} ");

            Console.WriteLine();

            Console.WriteLine("Init DataLogger");
            DataLogger dataLogger = DataLogger.DummyDataLogger;
            DataLogger.InitializeNewDataLogger("DotNetClientTest", null, false, out dataLogger);
            Session.DataLogger = dataLogger;           

            if (args == null || args.Length == 0)
            {
                ConsoleHelp();
                return;
            }

            switch (args[0])
            {
                case "testws":
                    TestConnectionWS();
                    break;

                case "run":
                    if (args.Length == 1)
                    {
                        RunTest();
                    }
                    else if (args.Length == 2)
                    {
                        int outparam;
                        if (int.TryParse(args[1],out outparam))
                        {
                            RunTest(outparam);
                        }
                    }
                    break;

                case "list":
                    ListOfWsTest(); // help;
                    break;

                default:
                    Console.WriteLine($"Not valid argument.");
                    ConsoleHelp();
                    break;
            }

            Console.WriteLine("End");
            Console.ReadKey();  // not working in Visual Studio Code
        }

        /// <summary>
        /// Run WS test
        /// </summary>
        private static void RunTest(int numberSelectedTest = -1)
        {
            Console.WriteLine("Start test");
            Console.WriteLine("Try to connect to web service...");

            if (!Uri.TryCreate(Settings.DefaultAddress, UriKind.Absolute, out Uri uri) || ((uri.Scheme != Uri.UriSchemeHttp) && (uri.Scheme != Uri.UriSchemeHttps)))
            {
                Session.DataLogger.ReportError($"Address is not valid HTTP/HTTPS URL,{Environment.NewLine}please insert correct server address!");
                Console.WriteLine($"Address is not valid HTTP/HTTPS URL,{Environment.NewLine}please insert correct server address!");
                Session.DataLogger.AddSeparator();
                return;
            }

            Session.Address = new EndpointAddress(Settings.DefaultAddress);

            try
            {
                Session.SoapApplicationClient = new ApplicationClient(Session.Binding, Session.Address);
                application_information informationAp = TestingMethods.SoapApplicationClient.get_information();
                Session.DataLogger.AddText($"{informationAp.name}, {informationAp.language_name}");
                Session.DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
                Console.WriteLine("Connected.");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error");
                Console.WriteLine(exception.Message);
                if (Session.SoapApplicationClient != null)
                {
                    if (Session.SoapApplicationClient.State != CommunicationState.Faulted)
                    {
                        Session.SoapApplicationClient.Close();
                    }
                    else
                    {
                        Session.SoapApplicationClient.Abort();
                    }

                    Session.SoapApplicationClient = null;
                }

                Session.DataLogger.ReportError(exception);
                Session.DataLogger.AddSeparator();
                return;
            }

            Session.DataLogger.AddSeparator();

            //all test
            Console.WriteLine("Load list method");
            List<MethodInfo> methodList = new List<MethodInfo>();
            List<MethodInfo> methodListForTest = new List<MethodInfo>();

            MethodInfo[] methodInfos = typeof(TestingMethods).GetMethods(BindingFlags.Public | BindingFlags.Static);
            methodList = methodInfos.ToList();

            int i = 0;

            //test 
            foreach (var item in methodList)
            {
                if (!isValidTestByName(item.Name.Replace('\u2040', ' ')))
                {
                    continue;
                }

                i++;
                bool isDisableTest = false;
                foreach (var itemDisableTest in Settings.DisableTest)
                {
                    if (item.Name == itemDisableTest)
                    {
                        isDisableTest = true;
                        Console.WriteLine("Skipped: " + item.Name);
                    }
                }

                if (isDisableTest)
                {
                    continue; // continue - foreach
                }
                else
                {
                    if (numberSelectedTest == -1)   // all test
                    {
                        methodListForTest.Add(item);
                    }
                    else if (numberSelectedTest == i)   // all test)
                    {
                        methodListForTest.Add(item);
                    }

                }
            }

            Console.WriteLine("...");
            //Session.SoapApplicationClient = new ApplicationClient(Session.Binding, new EndpointAddress(Settings.DefaultAddressModelClient));
            Session.SoapApplicationClient = new ApplicationClient(Session.Binding, Session.Address);
            application_information information = TestingMethods.SoapApplicationClient.get_information();
            Session.DataLogger.AddText($"{information.name}, {information.language_name}");
            //Session.DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);

            string textResult;
            RunTests(methodListForTest, out textResult, null, true);

            Console.WriteLine(textResult);
            Console.WriteLine();
            Console.WriteLine($"End - Test count: {methodListForTest.Count}");
        }

        /// <summary>
        /// chceck test name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static bool isValidTestByName(string name)
        {
            string[] nameParts = name.Split('_');

            if (nameParts.Length < 2)
            {
                return false;
            }

            if (nameParts[0].ToLower() != "test")
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Show help
        /// </summary>
        private static void ConsoleHelp()
        {
            Console.WriteLine("Application command-line arguments ");
            Console.WriteLine("==================================");
            Console.WriteLine("run          Run test");
            Console.WriteLine("run XX       Run test with number XX");
            Console.WriteLine("testws       Call WS and write basic data");
            Console.WriteLine("list         Displays all available test");

            Console.WriteLine("==================================");

        }

        /// <summary>
        /// Show list of test method
        /// </summary>
        static void ListOfWsTest()
        {
            Console.WriteLine("List web service tests...");        
            
            MethodInfo[] methodInfos = typeof(TestingMethods).GetMethods(BindingFlags.Public | BindingFlags.Static);
            Console.WriteLine($"{methodInfos.Length} methods found.");        

            int countOfMethods = DislayWSTest(methodInfos, $"{Program.ApplicationName} Web Service Tests");
            Console.WriteLine($"===============================================================");
            Console.WriteLine($"{countOfMethods} methods parsed.");
            
        }

        /// <summary>
        /// Show list of test method
        /// </summary>
        /// <param name="methodInfos"></param>
        /// <param name="rootName"></param>
        /// <returns></returns>
        private static int DislayWSTest(MethodInfo[] methodInfos, string rootName)
        {
            int countOfMethods = 0;
            int indexRow = 0;

            for (int methodIndex = 0; methodIndex < methodInfos.Length; methodIndex++)
            {
                MethodInfo methodInfo = methodInfos[methodIndex];

                string[] nameParts = methodInfo.Name.Replace('\u2040', ' ').Split('_');

                if (!isValidTestByName(methodInfo.Name.Replace('\u2040', ' ')))
                {
                    continue;
                }

                //TreeItemMethodInfo currentItem = root;
                Console.WriteLine();
                Console.Write($"{++indexRow} - ");

                for (int partIndex = 1; partIndex < nameParts.Length; partIndex++)
                {
                    Console.Write($"{nameParts[partIndex]}");
                    //Console.Write($" - {methodInfo.ToString()}");
                    Console.Write("  ");
                   
                }
            }
            Console.WriteLine();
            countOfMethods = indexRow;
            return countOfMethods;
        }

        /// <summary>
        /// Test connection
        /// </summary>
        static void TestConnectionWS()
        {
            var wc = new System.Net.WebClient();
            string data = wc.DownloadString(Settings.DefaultAddress + "/wsdl");

            Console.WriteLine("Test load data from URI.");
            if (data.Length > 100)
            {
                Console.WriteLine(data.Substring(0, 100));
                Console.WriteLine("...");
            }
            else
            {
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
            var httpResponse = client.PostAsync(Settings.DefaultAddress, new StringContent(postData, System.Text.Encoding.UTF8, "text/xml")).Result;
            var data2 = httpResponse.Content.ReadAsStringAsync().Result;

            Console.WriteLine(data2);
        }
        private static bool RunTests(List<System.Reflection.MethodInfo> testList, out string textResult, Action<string> reportMethod = null, bool getTextResult = false)
        {
            textResult = string.Empty;

            Session.DataLogger.AddText($"{testList.Count} tests to pass");
            Session.DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            Session.DataLogger.AddSeparator();

            if (testList.Count == 0)
            {
                return false;
            }

            Session.DataLogger.AddLogStart("Passing through checked tests...");

            Dictionary<string, TestingMethods.TestResultType> resultDictionary = new Dictionary<string, TestingMethods.TestResultType>();
            int maxLength = 0;

            foreach (System.Reflection.MethodInfo method in testList)
            {
                string methodName = method.ConvertToString();

                if (Session.SoapApplicationClient == null)
                {
                    Session.DataLogger.AddLogStart(methodName + "...");
                    Session.DataLogger.ReportError($"Web Service Client Proxy is disconnected from server!{Environment.NewLine}Please try to reconnect it.", true);
                    Session.DataLogger.AddSeparator();
                    resultDictionary.Add(methodName, TestingMethods.TestResultType.SKIPPED);
                    continue;
                }

                Console.WriteLine("Test: " + methodName);
                reportMethod?.Invoke("Test: " + methodName);

                try
                {
                    bool result = (bool)method.Invoke(null, null);
                    resultDictionary.Add(methodName, result ? TestingMethods.TestResultType.PASSED : TestingMethods.TestResultType.FAILED);

                    maxLength = Math.Max(maxLength, methodName.Length);
                    Session.DataLogger.AddSeparator();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Test: {methodName} FAILED - Invoke");
                    Session.DataLogger.ReportError(ex.Message);
                    resultDictionary.Add(methodName, TestingMethods.TestResultType.FAILED);
                }
            }

            reportMethod?.Invoke(string.Empty);

            Session.DataLogger.AddText("Results of tests:");

            DataLogger.LogResultType testResult = DataLogger.LogResultType.DONE;

            StringBuilder stringBuilder = new StringBuilder();

            foreach (KeyValuePair<string, TestingMethods.TestResultType> result in resultDictionary)
            {
                string resultLine = $"{result.Key.PadRight(maxLength)} : {result.Value}";
                Session.DataLogger.AddText($"  {resultLine}");

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

            Session.DataLogger.AddLogEnd(testResult);
            Session.DataLogger.AddSeparator();

            textResult = stringBuilder.ToString();

            return testResult == DataLogger.LogResultType.DONE;
        }
    }
}
