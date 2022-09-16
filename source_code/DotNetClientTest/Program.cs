using Dlubal.WS.Clients.DotNetClientTest.Dialogs;
using Dlubal.WS.Common.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Threading;
using System.Windows;

#if RFEM
using Dlubal.WS.Rfem6.Application;
using ApplicationClient = Dlubal.WS.Rfem6.Application.RfemApplicationClient;
#elif RSTAB
using Dlubal.WS.Rstab9.Application;
using ApplicationClient = Dlubal.WS.Rstab9.Application.RstabApplicationClient;
#elif RSECTION
using Dlubal.WS.RSection1.Application;
using ApplicationClient = Dlubal.WS.RSection1.Application.RSectionApplicationClient;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    class Program
    {
#if RFEM
        public const string ApplicationName = "RFEM6";
#elif RSTAB
        public const string ApplicationName = "RSTAB9";
#elif RSECTION
        public const string ApplicationName = "RSECTION1";
#endif
        private const int ResultOk = 0;
        private const int ResultTestFailed = 1;
        private const int ResultException = 2;

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool FreeConsole();

        /// <summary>
        /// We can start application as a command line tool or as a GUI application.
        /// </summary>
        /// <param name="args">Arguments passed to the application.</param>
        /// <remarks>
        /// -url=URL          - End point URL where the SOAP WS server is listening.
        /// -definitions=PATH - Path to input file containing methods definition to be tested.
        /// -results=PATH     - Path to output file containing results of tests.
        /// </remarks>
        [STAThread]
        static int Main(string[] args)
        {
            int result = ResultOk;
            IsConsoleApplication = args.Length > 0;

            if (!IsConsoleApplication)
            {
                DataLogger dataLogger = DataLogger.DummyDataLogger;
                DataLogger.InitializeNewDataLogger("DotNetClientTest", null, false, out dataLogger);
                Session.DataLogger = dataLogger;

                // console in not needed, free it
                FreeConsole();

                string root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                TestingMethods.ScriptDirectoryPaths = new List<string>() { Path.Combine(root, @"..\..\..\..\..\scripts") };

                // run in window
                Application application = new Application();
                MainWindow window = new MainWindow(dataLogger);
                application.Run(window);
            }
            else
            {
                // run in console
                if (string.IsNullOrWhiteSpace(args[0]) || args[0] == "-?" || args[0] == "/?" || string.Equals(args[0], "-help", StringComparison.CurrentCultureIgnoreCase) || string.Equals(args[0], "/help", StringComparison.CurrentCultureIgnoreCase))
                {
                    Console.WriteLine("Command line parameters:");
                    Console.WriteLine("------------------------");
                    Console.WriteLine($"{AppDomain.CurrentDomain.FriendlyName} -url=URL [-definitions=PATH] [-results=PATH]");
                    Console.WriteLine("");
                    Console.WriteLine("-url=URL          - End point URL where the SOAP WS server is listening.");
                    Console.WriteLine("-logs=PATH        - A path to directory with logs.");
                    Console.WriteLine("-definitions=PATH - A path to input file containing method definitions to be tested.");
                    Console.WriteLine("-results=PATH     - A path to output file containing results of tests.");
                    Console.WriteLine("-scripts=PATH     - A path to folder with JS scripts to be used for automatic tests.");
                    Console.WriteLine("-skipScripts=LIST - List of scripts to be skipped in automatic tests.");
                    Console.WriteLine("");
                    Console.WriteLine("Examples:");
                    Console.WriteLine("-url=http://localhost:8081/");
                    Console.WriteLine("-definitions=\"C:\\Temp\\test_methods.txt\"");
                    Console.WriteLine("-results=\"C:\\Temp\\test_results.txt\"");
                    Console.WriteLine("-scripts=\"C:\\Temp\\scripts\"");
                    Console.WriteLine("-skipScripts=\"nodes, members\"");
                }
                else
                {
                    try
                    {
                        string logPath = null;
                        string definitionFilePath = string.Empty;
                        string resultFilePath = string.Empty;

                        // Ziskani dat z argumentu.
                        foreach (string arg in args)
                        {
                            if (arg.StartsWith("-logs="))
                            {
                                logPath = arg.Substring(arg.IndexOf('=') + 1);
                            }
                            else if (arg.StartsWith("-url=") || arg.StartsWith("/url="))
                            {
                                Session.Address = new EndpointAddress(arg.Substring(arg.IndexOf('=') + 1));
                            }
                            else if (arg.StartsWith("-definitions="))
                            {
                                definitionFilePath = arg.Substring(arg.IndexOf('=') + 1);
                            }
                            else if (arg.StartsWith("-results="))
                            {
                                resultFilePath = arg.Substring(arg.IndexOf('=') + 1);
                            }
                            else if (arg.StartsWith("-scripts="))
                            {
                                string directoryPaths = arg.Substring(arg.IndexOf('=') + 1).Replace("\"", string.Empty);
                                TestingMethods.ScriptDirectoryPaths = directoryPaths.Split(',').ToList();
                            }
                            else if (arg.StartsWith("-skipScripts="))
                            {
                                string ignored = arg.Substring(arg.IndexOf('=') + 1).Replace(" ", string.Empty);
                                TestingMethods.SkipScripts = ignored.Split(',').ToList();
                            }
                        }

                        DataLogger dataLogger = DataLogger.DummyDataLogger;
                        DataLogger.InitializeNewDataLogger("DotNetClientTest", logPath, false, out dataLogger);
                        Session.DataLogger = dataLogger;

                        // Nastaveni cesty k souboru s definicemi, pokud neni zadana.
                        if (string.IsNullOrWhiteSpace(definitionFilePath))
                        {
                            string dataFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Properties.Settings.Default.DataFolder);
                            if (!Directory.Exists(dataFolderPath))
                            {
                                Directory.CreateDirectory(dataFolderPath);
                            }
                            definitionFilePath = Path.Combine(dataFolderPath, Properties.Settings.Default.TestMethodsFile);
                        }

                        // Nastaveni cesty k souboru s vysledky, pokud neni zadana.
                        if (string.IsNullOrWhiteSpace(resultFilePath))
                        {
                            string dataFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Properties.Settings.Default.DataFolder);
                            if (!Directory.Exists(dataFolderPath))
                            {
                                Directory.CreateDirectory(dataFolderPath);
                            }
                            resultFilePath = Path.Combine(dataFolderPath, Properties.Settings.Default.TestResultsFile);
                        }

                        // Nacteni souboru s nazvy spoustenych testu.
                        List<string> testNameList = new List<string>();
                        using (StreamReader streamReader = new StreamReader(definitionFilePath))
                        {
                            string line;
                            while ((line = streamReader.ReadLine()) != null)
                            {
                                if (!line.Trim().StartsWith("//"))
                                {
                                    testNameList.Add(line);
                                }
                            }
                            streamReader.Close();
                        }

                        MethodInfo[] methodInfos = typeof(TestingMethods).GetMethods(BindingFlags.Public | BindingFlags.Static);
                        MethodInfo[] testMethodList = GetParseTestMethods(methodInfos);

                        Session.DataLogger.WriteToLogAndConsole($"{testMethodList.Length} test methods in the application");

                        // Serazeni spoustenych testu podle seznamu v txt souboru.
                        // ToDo: Neoptimalizovane reseni pomoci where-contains.
                        List<MethodInfo> selectedMethods = methodInfos.Where(m => testNameList.Contains(m.Name)).OrderBy(x => testNameList.IndexOf(x.Name)).ToList();

                        Session.DataLogger.WriteToLogAndConsole($"{selectedMethods.Count} test methods selected");

                        // Pripojeni k SOAP WS serveru.
                        Session.SoapApplicationClient = new ApplicationClient(Session.Binding, Session.Address);

                        application_information information;

                        // Time to time happens in Squish tests that the test is trying to use WS but opening of databases is still running.
                        // In this situation Server.operation_fault is thrown with message
                        // "It is not possible to execute the operation. Another operation is probably running in the application or a dialog is open."
                        // and test fails.
                        int iteration = 0;
                        do
                        {
                            try
                            {
                                information = Session.SoapApplicationClient.get_information();
                            }
                            catch (Exception ex)
                            {
                                information = null;

                                if (ex is FaultException fault &&
                                    fault.Code.Name.Equals("Server.operation_fault", StringComparison.CurrentCultureIgnoreCase) &&
                                    ++iteration < 30 /*wait max 30s*/)
                                {
                                    Thread.Sleep(1000);
                                    continue;
                                }

                                throw;
                            }

                        } while (information == null);

                        Session.DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);

                        Console.WriteLine($"Connected to application server {information.name}");
                        Console.WriteLine();

                        bool succeeded = Session.RunTests(selectedMethods, out string textResult, Console.WriteLine, getTextResult: true);
                        result = succeeded ? ResultOk : ResultTestFailed;

                        using (StreamWriter streamWriter = new StreamWriter(resultFilePath))
                        {
                            streamWriter.Write(textResult + (succeeded ? "Succeeded" : "Failed"));
                            streamWriter.Close();
                        }
                    }
                    catch (Exception ex)
                    {
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

                        result = ResultException;
                        Console.WriteLine($"FAILED: {ex.Message}");
                        Session.DataLogger.ReportError(ex, true);
                        Session.DataLogger.AddSeparator();
                    }
                    finally
                    {
                        Console.WriteLine("Closing SOAP WS clients");
                        Session.CloseSoapClients();
                    }

                    if (Debugger.IsAttached)
                    {
                        Console.WriteLine("Press key to exit program.");
                        Console.ReadKey();
                    }
                }
            }

            return result;
        }

        public static bool IsConsoleApplication { get; private set; }

        private static MethodInfo[] GetParseTestMethods(MethodInfo[] methodInfos)
        {
            List<MethodInfo> methods = new List<MethodInfo>();

            foreach (var methodInfo in methodInfos)
            {
                string[] nameParts = methodInfo.Name.Replace('\u2040', ' ').Split('_');

                if (nameParts.Length < 2)
                {
                    continue;
                }

                if (nameParts[0].ToLower() != "test")
                {
                    continue;
                }

                methods.Add(methodInfo);
            }

            return methods.ToArray();
        }
    }
}