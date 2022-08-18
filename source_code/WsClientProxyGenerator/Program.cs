using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Services.Description;
using System.Xml;
using System.Xml.Schema;

namespace Dlubal.WS.Tools.WsClientProxyGenerator
{
    public class Program
    {
        // Error codes
        private const int ERROR_SUCCESS = 0x0;
        private const int ERROR_INITIALIZATION_FAILED = 0x1;
        private const int ERROR_FILE_NOT_FOUND = 0x2;
        private const int ERROR_GENERATION_FAILED = 0x3;
        private const int ERROR_REBUILD_WS_PROJECTS_FAILED = 0x4;

        enum EApplicationType : short
        {
            E_APPLICATION_TYPE_UNKNOWN = 0,
            E_APPLICATION_TYPE_RFEM6 = 1,
            E_APPLICATION_TYPE_RSTAB9 = 2
        }

        private static EApplicationType activeApplication;

        // Application Server RFEM
        private const string RFEM6_APPLICATION_SERVER_NAME = "RfemApplication";
        private const string RFEM6_APPLICATION_SERVER_PROXY = "RfemApplicationProxy";
        internal const string RFEM6_APPLICATION_SERVER_INTERFACE_NAME = "IRfemApplication";
        internal const string RFEM6_APPLICATION_CLIENT_NAME = "RfemApplicationClient";

        // Application Server RSTAB
        private const string RSTAB9_APPLICATION_SERVER_NAME = "RstabApplication";
        private const string RSTAB9_APPLICATION_SERVER_PROXY = "RstabApplicationProxy";
        internal const string RSTAB9_APPLICATION_SERVER_INTERFACE_NAME = "IRstabApplication";
        internal const string RSTAB9_APPLICATION_CLIENT_NAME = "RstabApplicationClient";

        // Model Server RFEM
        private const string RFEM6_MODEL_SERVER_NAME = "RfemModel";
        private const string RFEM6_MODEL_SERVER_PROXY = "RfemModelProxy";
        internal const string RFEM6_MODEL_SERVER_INTERFACE_NAME = "IRfemModel";
        internal const string RFEM6_MODEL_CLIENT_NAME = "RfemModelClient";

        // Model Server RSTAB
        private const string RSTAB9_MODEL_SERVER_NAME = "RstabModel";
        private const string RSTAB9_MODEL_SERVER_PROXY = "RstabModelProxy";
        internal const string RSTAB9_MODEL_SERVER_INTERFACE_NAME = "IRstabModel";
        internal const string RSTAB9_MODEL_CLIENT_NAME = "RstabModelClient";

        // Common constants
        private const string WSDL_DIRECTORY_PATH = @"WSDL\";
        private const string SERVICE_REFERENCE_DIRECTORY_PATH = @"Service Reference\";
        private const string SERVICE_REFERENCE_FILE_NAME = "Reference.cs";
        private const string USER_TYPE_ARRAY_OF_INT_NAME = "array_of_int";

        private const string OPERATION_SUCCESSFUL = "DONE";
        private const string OPERATION_FAILED = "FAILED";

        // RFEM constants
        private const string RFEM6_SOAP_WS_CLIENT_PROJECT_NAME = "RfemSoapWsClient";
        private const string RFEM6_SOAP_WS_CLIENT_PROJECT_PATH = @"..\..\..\RfemSoapWsClient\";
        private const string RFEM6_APPLICATION_NAMESPACE = "Dlubal.WS.Rfem6.Application";
        private const string RFEM6_MODEL_NAMESPACE = "Dlubal.WS.Rfem6.Model";

        // RSTAB constants
        private const string RSTAB9_SOAP_WS_CLIENT_PROJECT_NAME = "RstabSoapWsClient";
        private const string RSTAB9_SOAP_WS_CLIENT_PROJECT_PATH = @"..\..\..\RstabSoapWsClient\";
        private const string RSTAB9_APPLICATION_NAMESPACE = "Dlubal.WS.Rstab9.Application";
        private const string RSTAB9_MODEL_NAMESPACE = "Dlubal.WS.Rstab9.Model";

        // Visual studio paths
        private const string SVCUTIL_PATH = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.7.2 Tools\SvcUtil.exe";
        private const string MSBUILD_15_PATH = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe";
        private const string MSBUILD_15E_PATH = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\WDExpress\MSBuild\15.0\Bin\msbuild.exe";
        private const string MSBUILD_2019C_PATH = @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\msbuild.exe";
        private const string MSBUILD_2022C_PATH = @"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\msbuild.exe";

        // Common variables
        private static string serverName;
        private static string serverProxy;
        private static string serverNamespace;
        internal static string ServerInterfaceName { get; private set; }
        internal static string ClientName { get; private set; }

        internal static Dictionary<string, List<string>> TypeToFixDictionary { get; private set; } = new Dictionary<string, List<string>>();

        internal static readonly List<string> MethodToFixList = new List<string>();

        public static int Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            // Overeni existence nastroju
            if (!File.Exists(SVCUTIL_PATH))
            {
                Console.Error.WriteLine("Tool SvcUtil.exe not found.\n\"{0}\"", SVCUTIL_PATH);
                return ERROR_FILE_NOT_FOUND;
            }

            if (!IsActiveMsBuild()) // zkontroluje cesty k MSBUILD a nastaví cesty k němu
            {
                return ReportMsBuildNotFound(); //vypíše, že nenašlo cestu k MSBuild
            }

            if (!InitializeSeviceDirectory())
            {
                return ERROR_INITIALIZATION_FAILED;
            }

            activeApplication = GetApplicationType();
            if (activeApplication == EApplicationType.E_APPLICATION_TYPE_UNKNOWN)
            {
                Console.Error.WriteLine("Wsdl files not found");
                return ERROR_INITIALIZATION_FAILED;
            }

            // Application Server
            Console.WriteLine("Generating files for application:");
            InitializePathsForApplicationServer();
            if (!Generate())
            {
                return ERROR_GENERATION_FAILED;
            }

            // Model Server
            Console.WriteLine("Generating files for model:");

            InitializePathsForModelServer();
            if (!Generate())
            {
                return ERROR_GENERATION_FAILED;
            }

            if (!BuildSoapWsClient())
            {
                return ERROR_GENERATION_FAILED;
            }

            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "-rebuild_all":
                        return BuildExternalProjects();
                    default:
                        Console.WriteLine("Unknown parameter \"{0}\"", args[0]);
                        break;
                }
            }

            return ERROR_SUCCESS;
        }

        private static EApplicationType GetApplicationType()
        {
            if (File.Exists(Path.Combine(WSDL_DIRECTORY_PATH + "RfemApplication.wsdl")))
            {
                return EApplicationType.E_APPLICATION_TYPE_RFEM6;
            }
            else if (File.Exists(Path.Combine(WSDL_DIRECTORY_PATH + "RstabApplication.wsdl")))
            {
                return EApplicationType.E_APPLICATION_TYPE_RSTAB9;
            }
            else
            {
                return EApplicationType.E_APPLICATION_TYPE_UNKNOWN;
            }
        }

        private static string GetApplicationDependentBuildConfiguration()
        {
            string buildConfiguration;

            switch (GetApplicationType())
            {
                case EApplicationType.E_APPLICATION_TYPE_RFEM6:
                {
#if DEBUG
                    buildConfiguration = "Debug RFEM";
#else
                    buildConfiguration = "Release RFEM";
#endif
                    break;
                }
                case EApplicationType.E_APPLICATION_TYPE_RSTAB9:
                {
#if DEBUG
                    buildConfiguration = "Debug RSTAB";
#else
                    buildConfiguration = "Release RSTAB";
#endif
                    break;
                }
                default:
                {
                    buildConfiguration = string.Empty;
                    break;
                }
            }

            return buildConfiguration;
        }

        private static void InitializePathsForApplicationServer()
        {
            switch (activeApplication)
            {
                case EApplicationType.E_APPLICATION_TYPE_RFEM6:
                    serverName = RFEM6_APPLICATION_SERVER_NAME;
                    serverProxy = RFEM6_APPLICATION_SERVER_PROXY;
                    ServerInterfaceName = RFEM6_APPLICATION_SERVER_INTERFACE_NAME;
                    serverNamespace = RFEM6_APPLICATION_NAMESPACE;
                    ClientName = RFEM6_APPLICATION_CLIENT_NAME;
                    break;

                case EApplicationType.E_APPLICATION_TYPE_RSTAB9:
                    serverName = RSTAB9_APPLICATION_SERVER_NAME;
                    serverProxy = RSTAB9_APPLICATION_SERVER_PROXY;
                    ServerInterfaceName = RSTAB9_APPLICATION_SERVER_INTERFACE_NAME;
                    serverNamespace = RSTAB9_APPLICATION_NAMESPACE;
                    ClientName = RSTAB9_APPLICATION_CLIENT_NAME;
                    break;
            }
        }

        private static void InitializePathsForModelServer()
        {
            switch (activeApplication)
            {
                case EApplicationType.E_APPLICATION_TYPE_RFEM6:
                    serverName = RFEM6_MODEL_SERVER_NAME;
                    serverProxy = RFEM6_MODEL_SERVER_PROXY;
                    ServerInterfaceName = RFEM6_MODEL_SERVER_INTERFACE_NAME;
                    serverNamespace = RFEM6_MODEL_NAMESPACE;
                    ClientName = RFEM6_MODEL_CLIENT_NAME;
                    break;
                case EApplicationType.E_APPLICATION_TYPE_RSTAB9:
                    serverName = RSTAB9_MODEL_SERVER_NAME;
                    serverProxy = RSTAB9_MODEL_SERVER_PROXY;
                    ServerInterfaceName = RSTAB9_MODEL_SERVER_INTERFACE_NAME;
                    serverNamespace = RSTAB9_MODEL_NAMESPACE;
                    ClientName = RSTAB9_MODEL_CLIENT_NAME;
                    break;
            }
        }

        private static string GetWsClientProjectPath()
        {
            switch (activeApplication)
            {
                case EApplicationType.E_APPLICATION_TYPE_RFEM6:
                    return RFEM6_SOAP_WS_CLIENT_PROJECT_PATH;
                case EApplicationType.E_APPLICATION_TYPE_RSTAB9:
                    return RSTAB9_SOAP_WS_CLIENT_PROJECT_PATH;
            }
            return "";
        }

        private static string GetSoapWsClientProjectName()
        {
            switch (activeApplication)
            {
                case EApplicationType.E_APPLICATION_TYPE_RFEM6:
                    return RFEM6_SOAP_WS_CLIENT_PROJECT_NAME;
                case EApplicationType.E_APPLICATION_TYPE_RSTAB9:
                    return RSTAB9_SOAP_WS_CLIENT_PROJECT_NAME;
            }
            return "";
        }

        /// Metoda vrací true, jestli existuje alespoň jedna z MSBUILD_*_PATH
        private static bool IsActiveMsBuild()
        {
            return !string.IsNullOrEmpty(GetActiveMsBuildPath());
        }

        private static string GetActiveMsBuildPath()
        {
            if (File.Exists(MSBUILD_15E_PATH))
            {
                return MSBUILD_15E_PATH;
            }
            else if (File.Exists(MSBUILD_15_PATH))
            {
                return MSBUILD_15_PATH;
            }
            else if (File.Exists(MSBUILD_2019C_PATH))
            {
                return MSBUILD_2019C_PATH;
            }
            else if (File.Exists(MSBUILD_2022C_PATH))
            {
                return MSBUILD_2022C_PATH;
            }
            return "";
        }

        private static string GetToolsVersion()
        {
            if (File.Exists(MSBUILD_15E_PATH))
            {
                return "15.0";
            }
            else if (File.Exists(MSBUILD_15_PATH))
            {
                return "15.0";
            }
            else if (File.Exists(MSBUILD_2019C_PATH))
            {
                return "Current";
            }
            else if (File.Exists(MSBUILD_2022C_PATH))
            {
                return "Current";
            }
            return "";
        }

        /// Vypíše do konzole, že nenašlo cestu k MSBUILD a vrátí ERROR_FILE_NOT_FOUND
        private static int ReportMsBuildNotFound()
        {
            Console.Error.WriteLine("Tool msbuild.exe not found. One of following applications have to be installed: ");
            Console.Error.WriteLine("Visual Studio 2022: \"{0}\"", MSBUILD_2022C_PATH);
            Console.Error.WriteLine("Visual Studio 2019: \"{0}\"", MSBUILD_2019C_PATH);
            Console.Error.WriteLine("Visual Studio 2017: \"{0}\"", MSBUILD_15_PATH);
            Console.Error.WriteLine("Visual Studio Express 2017: \"{0}\"", MSBUILD_15E_PATH);
            return ERROR_FILE_NOT_FOUND;
        }

        /// Metoda spustí build externích projektů.
        private static int BuildExternalProjects()
        {
            Console.WriteLine("Building external projects:");

            bool succeedded = true;

            // DotNetClientTest
            succeedded &= BuildExternalProject(@"..\..\..\DotNetClientTest\DotNetClientTest.csproj", GetApplicationDependentBuildConfiguration());

            return succeedded ? ERROR_SUCCESS : ERROR_REBUILD_WS_PROJECTS_FAILED;
        }

        private static bool InitializeSeviceDirectory()
        {
            try
            {
                // Vytvori adresar pro vygenerovane soubory, pokud jiz neexistuje.
                if (!Directory.Exists(SERVICE_REFERENCE_DIRECTORY_PATH))
                {
                    Console.Write("Creating directory for service reference files...");
                    Directory.CreateDirectory(SERVICE_REFERENCE_DIRECTORY_PATH);
                    Console.WriteLine(OPERATION_SUCCESSFUL + Environment.NewLine);
                }
                else
                {
                    // Vymaze stare vygenerovane soubory, pokud adresar existuje.
                    Console.Write("Cleaning directory for service reference files...");
                    string[] files = Directory.GetFiles(SERVICE_REFERENCE_DIRECTORY_PATH);

                    foreach (string fileName in files)
                    {
                        File.Delete(fileName);
                    }

                    Console.WriteLine(OPERATION_SUCCESSFUL + Environment.NewLine);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(OPERATION_FAILED);
                Console.Error.WriteLine(exception.Message);

                if (exception.InnerException != null)
                {
                    Console.Error.WriteLine(exception.InnerException.Message);
                }

                return false;
            }
            return true;
        }

        private static bool Generate()
        {
            try
            {
                // Nacte ulozeny WSDL soubor a pripravi seznam trid a jejich datovych clenu,
                // ktere je potreba upravit kvuli spatne serializaci typu array_of_int.
                if (!ReadWSDL())
                {
                    return false;
                }

                // Vygeneruje SOAP WS rozhrani, implementaci WCF klienta a proxy objekty z ulozeneho WSDL souboru.
                if (!GenerateServiceReference())
                {
                    return false;
                }

                // Nacte soubor vygenerovany v predchozim kroku, ktery obsahuje SOAP WS rozhrani, implementaci klienta a proxy objekty.
                // Kvuli lepsi prehlednosti se oddeli SOAP WS rozhrani, implementace klienta a tridy proxy objektu do samostatnych souboru.
                // K prochazeni kodu a jeho upravam se pouziva .NET Compiler Platform ("Roslyn") --> https://github.com/dotnet/roslyn
                // Pri generovani kodu pomoci Roslyn je velmi uzitecny internetovy nastroj RoslynQuoter  --> http://roslynquoter.azurewebsites.net
                Console.Write("Reading service reference file...");
                string sourceText = File.ReadAllText(SERVICE_REFERENCE_DIRECTORY_PATH + serverName + SERVICE_REFERENCE_FILE_NAME);
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceText);
                SyntaxNode syntaxRoot = syntaxTree.GetRoot();
                Console.WriteLine(OPERATION_SUCCESSFUL);

                // Provede upravy SOAP WS rozhrani.
                Console.Write("Injecting WS interface...");
                InterfaceRewriter interfaceRewriter = new InterfaceRewriter();
                SyntaxNode injectedInterface = interfaceRewriter.Visit(syntaxRoot);
                Console.WriteLine(OPERATION_SUCCESSFUL);

                // Setridi obsah SOAP WS rozhrani podle nazvu a ulozi ho do noveho souboru.
                Console.Write("Sorting and writing WS interface...");
                AutoArrangeSyntaxWalker interfaceSyntaxWalker = new AutoArrangeSyntaxWalker();
                interfaceSyntaxWalker.Visit(injectedInterface);
                injectedInterface = new AutoArrangeSyntaxRewriter(interfaceSyntaxWalker).Visit(injectedInterface);
                string wsClientProjectPath = GetWsClientProjectPath();

                using (TextWriter interfaceFileWriter = File.CreateText(wsClientProjectPath + ServerInterfaceName + ".cs"))
                {
                    injectedInterface.WriteTo(interfaceFileWriter);
                }

                Console.WriteLine(OPERATION_SUCCESSFUL);

                // Provede upravy implementace SOAP WS klienta.
                Console.Write("Injecting WS client class...");
                ClientRewriter clientRewriter = new ClientRewriter();
                SyntaxNode injectedClient = clientRewriter.Visit(syntaxRoot);
                Console.WriteLine(OPERATION_SUCCESSFUL);

                // Setridi obsah SOAP WS klienta podle nazvu a ulozi ho do noveho souboru.
                Console.Write("Sorting and writing WS client class...");
                AutoArrangeSyntaxWalker clientSyntaxWalker = new AutoArrangeSyntaxWalker();
                clientSyntaxWalker.Visit(injectedClient);
                injectedClient = new AutoArrangeSyntaxRewriter(clientSyntaxWalker).Visit(injectedClient);

                using (TextWriter clientFileWriter = File.CreateText(wsClientProjectPath + ClientName + ".cs"))
                {
                    injectedClient.WriteTo(clientFileWriter);
                }

                Console.WriteLine(OPERATION_SUCCESSFUL);

                // Provede upravy implementace proxy objektu SOAP WS.
                Console.Write("Injecting WS proxy classes...");
                ProxyRewriter proxyRewriter = new ProxyRewriter();
                SyntaxNode injectedProxy = proxyRewriter.Visit(syntaxRoot);
                Console.WriteLine(OPERATION_SUCCESSFUL);

                // Setridi proxy objekty SOAP WS podle nazvu a ulozi je do noveho souboru.
                Console.Write("Sorting and writing WS proxy classes...");
                AutoArrangeSyntaxWalker proxySyntaxWalker = new AutoArrangeSyntaxWalker();
                proxySyntaxWalker.Visit(injectedProxy);
                injectedProxy = new AutoArrangeSyntaxRewriter(proxySyntaxWalker).Visit(injectedProxy);

                using (TextWriter proxyFileWriter = File.CreateText(wsClientProjectPath + serverProxy + ".cs"))
                {
                    injectedProxy.WriteTo(proxyFileWriter);
                }
                Console.WriteLine(OPERATION_SUCCESSFUL);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.Message);

                if (exception.InnerException != null)
                {
                    Console.Error.WriteLine(exception.InnerException.Message);
                }
                return false;
            }
            finally
            {
                Console.WriteLine();
            }

            return true;
        }

        private static bool BuildSoapWsClient()
        {
            try
            {
                // Prelozi SOAP WS klienta bez XML serializatoru.
                // Pro vygenerovani XML serializatoru potrebujeme assembly obsahujici
                // SOAP WS rozhrani, implementaci klienta a proxy objekty.
                if (!BuildSoapWsClient(false))
                {
                    return false;
                }

                // Vygeneruje XML serializatory pro SOAP WS klienta pomoci nastroje SvcUtil.
                if (!GenerateXmlSerializers())
                {
                    return false;
                }

                // Odmaze AssemblyVersionAttribute ze souboru obsahujiciho XML serializatory,
                // protoze tento atribut koliduje s atributem v souboru AssemblyInfo.cs v projektu.
                // Roslyn z nejakeho duvodu nezpracuje korektne zdrojovy soubor a proto zde nelze pouzit SyntaxRewriter.
                // Atribut je tedy primo nalezen ve zdrojovem kodu a odstranen pomoci instance tridy StringBuilder.
                string soapWsClientProjectPath = Path.Combine(GetWsClientProjectPath(), GetSoapWsClientProjectName());
                Console.Write("Removing AssemblyVersionAttribute from XML serializers...");
                string sourceText = File.ReadAllText(soapWsClientProjectPath + ".XmlSerializers.cs");
                int startIndex = sourceText.IndexOf("[assembly:System.Reflection.AssemblyVersionAttribute(");

                if (startIndex >= 0)
                {
                    string endString = "]" + Environment.NewLine;
                    int endIndex = sourceText.IndexOf(endString, startIndex, 100);
                    if (endIndex >= 0)
                    {
                        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(sourceText);
                        stringBuilder = stringBuilder.Remove(startIndex, endIndex - startIndex + endString.Length);
                        sourceText = stringBuilder.ToString();
                        File.WriteAllText(soapWsClientProjectPath + ".XmlSerializers.cs", sourceText);
                    }
                }
                Console.WriteLine(OPERATION_SUCCESSFUL);

                // Prelozi SOAP WS klienta se XML serializatory.
                if (!BuildSoapWsClient(true))
                {
                    return false;
                }
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.Message);

                if (exception.InnerException != null)
                {
                    Console.Error.WriteLine(exception.InnerException.Message);
                }
                return false;
            }
            finally
            {
                Console.WriteLine();
            }
            return true;
        }

        /// Sestaví Soap klienta, vytvoří cestu k souboru a argument string a zavolá obecnou funkci
        private static bool BuildSoapWsClient(bool withSerializers)
        {
            string projectFilePath = Path.Combine(GetWsClientProjectPath(), GetSoapWsClientProjectName()) + ".csproj";
            string buildConfiguration;

            if (withSerializers)
            {
                buildConfiguration = GetApplicationDependentBuildConfiguration();
            }
            else
            {
#if DEBUG
                buildConfiguration = "Debug no XML serializers";
#else
                buildConfiguration = "Release no XML serializers";
#endif
            }

            return BuildExternalProject(projectFilePath, buildConfiguration);
        }

        private static bool GenerateXmlSerializers()
        {
            string soapWsClientProjectPath = GetWsClientProjectPath();
            string soapWsClientProjectName = GetSoapWsClientProjectName();
#if DEBUG
            string partOfPath = @"bin\Debug_no_xml_serializers\";
#else
            string partOfPath = @"bin\Release_no_xml_serializers\";
#endif
            string AssemblyFilePath = Path.Combine(soapWsClientProjectPath, partOfPath, soapWsClientProjectName + ".dll");
            string OutputFilePath = Path.Combine(soapWsClientProjectPath, soapWsClientProjectName + ".XmlSerializers.cs");

            try
            {
                Console.Write("Generating XML serializers for SOAP WS client...");
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.StandardOutputEncoding = Encoding.GetEncoding("ibm852");
                process.StartInfo.FileName = SVCUTIL_PATH;
                process.StartInfo.Arguments = $"/t:xmlSerializer \"{AssemblyFilePath}\" /out:\"{OutputFilePath}\"";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Console.WriteLine(OPERATION_FAILED);
                    Console.Error.Write(output);
                    return false;
                }

                Console.WriteLine(OPERATION_SUCCESSFUL);
                process.Dispose();
            }
            catch (Exception exception)
            {
                Console.WriteLine(OPERATION_FAILED);
                Console.Error.WriteLine(exception.Message);

                if (exception.InnerException != null)
                {
                    Console.Error.WriteLine(exception.InnerException.Message);
                }
                return false;
            }

            return true;
        }

        private static bool GenerateServiceReference()
        {
            // Vygeneruje SOAP WS rozhrani, implementaci WCF klienta a proxy objekty z ulozeneho WSDL souboru.
            string inputFilePath = WSDL_DIRECTORY_PATH + serverName + ".wsdl";
            string outputFilePath = SERVICE_REFERENCE_DIRECTORY_PATH + serverName + SERVICE_REFERENCE_FILE_NAME;

            try
            {
                Console.Write($"Generating WS interface, WCF client and proxy classes from WSDL file ({inputFilePath})...");
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.StandardErrorEncoding = Encoding.GetEncoding("ibm852");
                process.StartInfo.StandardOutputEncoding = Encoding.GetEncoding("ibm852");
                process.StartInfo.FileName = SVCUTIL_PATH;
                process.StartInfo.Arguments = $"/t:code \"{inputFilePath}\" /out:\"{outputFilePath}\" /serializable /serializer:XmlSerializer /UseSerializerForFaults /syncOnly /language:csharp /edb /noconfig /namespace:*,{serverNamespace}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string outputError = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Console.WriteLine(OPERATION_FAILED);
                    Console.Error.Write(output);
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(outputError))
                {
                    Console.WriteLine(OPERATION_FAILED);
                    Console.Error.Write(outputError);
                    return false;
                }

                Console.WriteLine(OPERATION_SUCCESSFUL);
                process.Dispose();
            }
            catch (Exception exception)
            {
                Console.WriteLine(OPERATION_FAILED);
                Console.Error.WriteLine(exception.Message);

                if (exception.InnerException != null)
                {
                    Console.Error.WriteLine(exception.InnerException.Message);
                }
                return false;
            }

            return true;
        }

        /// Metoda pro sestavení dílčích projektů
        private static bool BuildExternalProject(string projectFilePath, string buildConfiguration = "")
        {
            if (!File.Exists(projectFilePath))
            {
                Console.Error.WriteLine($"Project '{projectFilePath}' not found.");
                return false;
            }

            if (string.IsNullOrEmpty(buildConfiguration))
            {
#if DEBUG
                buildConfiguration = "Debug";
#else
                buildConfiguration = "Release";
#endif
            }

            return BuildProject(projectFilePath, $"/nologo /p:Configuration=\"{buildConfiguration}\";Platform=AnyCPU /t:rebuild /toolsversion:");
        }

        /// Obecná metoda, která je společná pro SoapWs klienta i pro dílčí projekty
        private static bool BuildProject(string projectFilePath, string procesStartInfoArguments)
        {
            try
            {
                Console.Write("Building {0} project...", new FileInfo(projectFilePath).Name);
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.StandardOutputEncoding = Encoding.GetEncoding("ibm852");
                process.StartInfo.FileName = GetActiveMsBuildPath();

                // nastavení argumentů pro start procesu
                process.StartInfo.Arguments = procesStartInfoArguments + GetToolsVersion() + " " + projectFilePath;

                // Konfigurace prekladu projektu se prepina podle parametru metody.
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Console.WriteLine(OPERATION_FAILED);
                    Console.Error.Write(output);
                    return false;
                }

                Console.WriteLine(OPERATION_SUCCESSFUL);
                process.Dispose();
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.Message);

                if (exception.InnerException != null)
                {
                    Console.Error.WriteLine(exception.InnerException.Message);
                }
                return false;
            }

            return true;
        }

        private static bool ReadWSDL()
        {
            try
            {
                // Otevre ulozeny WSDL soubor a nacte ho jako instanci ServiceDescription.
                string wsdlFilePath = WSDL_DIRECTORY_PATH + serverName + ".wsdl";
                Console.Write($"Reading WSDL file ({wsdlFilePath})...");

                XmlTextReader xmlReader = new XmlTextReader(wsdlFilePath);
                ServiceDescription serviceDescription = ServiceDescription.Read(xmlReader);

                if (serviceDescription.Types.Schemas.Count == 0)
                {
                    Console.WriteLine(OPERATION_FAILED);
                    Console.WriteLine("ServiceDescription doesn't contain definitions of proxy classes.");
                    return false;
                }

                Console.WriteLine(OPERATION_SUCCESSFUL);

                Console.Write($"Analyzing WSDL file ({wsdlFilePath})...");

                // Prochazi vsechny definice proxy objektu a hleda vsechny XmlSchemaComplexType elementy,
                // ktere obsahuji XmlSchemaElement s uzivatelsky definovanym typem array_of_int.
                // Vytvori seznam trid a jejich datovych clenu, ktere je potreba upravit,
                // protoze typ array_of_int se mapuje jako string misto int[].
                var typesToFix = from complexType in serviceDescription.Types.Schemas[0].Items.OfType<XmlSchemaComplexType>()
                                 from element in (complexType.Particle as XmlSchemaSequence).Items.OfType<XmlSchemaElement>()
                                 where element.SchemaTypeName.Name == USER_TYPE_ARRAY_OF_INT_NAME
                                 group element.Name by complexType.Name;

                TypeToFixDictionary = typesToFix?.ToDictionary(item => item.Key, item => item.ToList()) ?? new Dictionary<string, List<string>>();

                // Jeste musime pridat typy, ktere maji stejne jmeno, jako jiz pridane typy, ale navic maji postfix Request/Response).
                // Najdeme je podle definice Message.
                MethodToFixList.Clear();
                if (TypeToFixDictionary.Count > 0)
                {
                    foreach (Message message in serviceDescription.Messages)
                    {
                        if ((message.Parts.Count > 0) && TypeToFixDictionary.ContainsKey(message.Parts[0].Element.Name))
                        {
                            string elementName = message.Parts[0].Element.Name.ToLower();

                            if (elementName.EndsWith("request"))
                            {
                                continue;
                            }
                            else if (elementName.EndsWith("response"))
                            {
                                int length = message.Parts[0].Element.Name.Length - "response".Length;
                                MethodToFixList.Add(message.Parts[0].Element.Name.Substring(0, length));

                                continue;
                            }

                            if (message.Name.ToLower().EndsWith("request"))
                            {
                                TypeToFixDictionary.Add(message.Parts[0].Element.Name + "Request", TypeToFixDictionary[message.Parts[0].Element.Name]);
                            }
                            else if (message.Name.ToLower().EndsWith("response"))
                            {
                                TypeToFixDictionary.Add(message.Parts[0].Element.Name + "Response", TypeToFixDictionary[message.Parts[0].Element.Name]);
                            }
                        }
                    }
                }

                Console.WriteLine(OPERATION_SUCCESSFUL);
                Console.WriteLine($"Types to fix: {TypeToFixDictionary.Count}");
                Console.WriteLine($"Methods to fix: {MethodToFixList.Count}");
            }
            catch (Exception exception)
            {
                Console.WriteLine(OPERATION_FAILED);
                Console.Error.WriteLine(exception.Message);

                if (exception.InnerException != null)
                {
                    Console.Error.WriteLine(exception.InnerException.Message);
                }
                return false;
            }

            return true;
        }
    }
}
