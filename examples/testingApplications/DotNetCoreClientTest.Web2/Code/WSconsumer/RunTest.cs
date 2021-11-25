using Dlubal.WS.Clients.DotNetClientTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using Dlubal.WS.RfemRstabSoapWsCoreLib.Tools;
using Dlubal.WS.RfemRstabSoapWsCoreLib;
using System.Text;

#if RFEM
using Dlubal.WS.Rfem6.Application;
using ApplicationClient = Dlubal.WS.Rfem6.Application.RfemApplicationClient;
#elif RSTAB
using Dlubal.WS.Rstab9.Application;
using ApplicationClient = Dlubal.WS.Rstab9.Application.RstabApplicationClient;
#endif

namespace DotNetCoreClientTest.Web2.Code.WSconsumer
{
    public class RunTest
    {
        /// <summary>
        /// Run WS test
        /// </summary>
        public void Run(Models.ResultTestModel model, int idTest = -1)
        {

            if (!Uri.TryCreate(MyConfig.DefaultAddress, UriKind.Absolute, out Uri uri) || ((uri.Scheme != Uri.UriSchemeHttp) && (uri.Scheme != Uri.UriSchemeHttps)))
            {
                Session.DataLogger.ReportError($"Address is not valid HTTP/HTTPS URL,{Environment.NewLine}please insert correct server address!");
                Console.WriteLine($"Address is not valid HTTP/HTTPS URL,{Environment.NewLine}please insert correct server address!");
                Session.DataLogger.AddSeparator();
                model.Error = $"Address is not valid HTTP/HTTPS URL,{Environment.NewLine}please insert correct server address!";

                return;
            }

            Session.Address = new EndpointAddress(MyConfig.DefaultAddress);

            try
            {
                Session.SoapApplicationClient = new ApplicationClient(Session.Binding, Session.Address);
                application_information informationAp = TestingMethods.SoapApplicationClient.get_information();
                Session.DataLogger.AddText($"{informationAp.name}, {informationAp.language_name}");
                Session.DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
                model.Messages.Add("Connected.");
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

                model.Error = exception.Message;


                return;
            }

            Session.DataLogger.AddSeparator();

            Session.DataLogger.LogBox = ""; // smaže předchozí vysledky - pozor na další instance/uživatele

            //all test
            Console.WriteLine("Load list method");
            model.Messages.Add("Load list method");
            List<MethodInfo> methodList = new List<MethodInfo>();
            List<MethodInfo> methodListForTest = new List<MethodInfo>();

            MethodInfo[] methodInfos = typeof(TestingMethods).GetMethods(BindingFlags.Public | BindingFlags.Static);
            methodList = methodInfos.ToList();

            int i = 0;

            //test 
            foreach (var item in methodList)
            {
                if (!Common.IsValidTestByName(item.Name.Replace('\u2040', ' ')))
                {
                    continue;
                }

                i++;
                bool isDisableTest = false;
                foreach (var itemDisableTest in MyConfig.DisableTest)
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
                    if (idTest == -1)   // all test
                    {
                        methodListForTest.Add(item);
                    }
                    else if (idTest == i)   // all test)
                    {
                        methodListForTest.Add(item);
                        model.Name = item.Name;
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

            // \n to new line on frontend
            foreach (var item in Session.DataLogger.LogBox.Split('\n'))
            {
                model.Messages.Add(item);
            }

            //model.Messages.Add(Session.DataLogger.LogBox);

            Console.WriteLine(textResult);
            model.Messages.Add(textResult);
            model.Result = textResult;
            Console.WriteLine();
            Console.WriteLine($"End - Test count: {methodListForTest.Count}");
            model.Messages.Add($"End - Test count: {methodListForTest.Count}");
        }

        private bool RunTests(List<System.Reflection.MethodInfo> testList, out string textResult, Action<string> reportMethod = null, bool getTextResult = false)
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
