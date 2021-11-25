using System;
using System.Collections.Generic;
using System.IO;
using Dlubal.WS.RfemRstabSoapWsCoreLib.Tools;
using static Dlubal.WS.RfemRstabSoapWsCoreLib.Tools.DataLogger;

#if RFEM
using Dlubal.WS.Rfem6.Model;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public partial class TestingMethods
    {
        public static string ScriptDirectoryPath;
        public static List<string> SkipScripts;

        public static bool Test_Automatic\u2040Tests_Compare\u2040Current\u2040Data()
        {
            if (SoapApplicationClient.get_model_list().Length == 0)
            {
                return false;
            }

            bool succeeded = false;

            _ = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_NODE, 0);

            try
            {
                string message = $"Testing current model...";
                Console.WriteLine(message);
                DataLogger.AddLogStart(message);

                succeeded = Test_General_Copy⁀And⁀Verify⁀All⁀Objects();
                if (succeeded)
                {
                    Console.WriteLine("...PASSED");
                    DataLogger.AddLogEnd(LogResultType.DONE);
                }
                else
                {
                    Console.WriteLine("...FAILED");
                    DataLogger.AddLogEnd(LogResultType.FAILED);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                Console.WriteLine("...FAILED");
                _ = DataLogger.ReportError(ex, true);
            }

            return succeeded;
        }

        public static bool Test_Automatic\u2040Tests_Compare\u2040Scripts\u2040Data()
        {
            //if (!Directory.Exists(ScriptDirectoryPath) && !Program.IsConsoleApplication)
            //{
            //    FolderBrowserDialog dialog = new FolderBrowserDialog
            //    {
            //        Description = "Select folder with JS scripts for start automatic tests.",
            //        ShowNewFolderButton = false,
            //        SelectedPath = ScriptDirectoryPath
            //    };

            //    if (dialog.ShowDialog() == DialogResult.OK)
            //    {
            //        ScriptDirectoryPath = dialog.SelectedPath;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}

            if (SoapApplicationClient.get_model_list().Length == 0)
            {
                SoapApplicationClient.new_model("Test");
            }

            SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_NODE, 0);

            int passed = 0;
            int failed = 0;

            void RunScriptTestsInDirectory(string rootDirectory)
            {
                // test all scripts in the root directory
                string[] scripts = Directory.GetFiles(rootDirectory, "*Prepare.js");
                foreach (string script in scripts)
                {
                    string scriptName = Path.GetFileName(script).Replace("Prepare.js", string.Empty);
                    if (SkipScripts != null && SkipScripts.Contains(scriptName))
                    {
                        continue;
                    }

                    try
                    {
                        string message = $"Using script '{script}'...";
                        Console.WriteLine(message);
                        DataLogger.AddLogStart(message);

                        SoapModelClient.delete_all();
                        SoapModelClient.run_script(script);

                        if (Test_General_Copy⁀And⁀Verify⁀All⁀Objects())
                        {
                            passed += 1;
                            Console.WriteLine("...PASSED");
                            DataLogger.AddLogEnd(LogResultType.DONE);
                        }
                        else
                        {
                            failed += 1;
                            Console.WriteLine("...FAILED");
                            DataLogger.AddLogEnd(LogResultType.FAILED);
                        }
                    }
                    catch (Exception ex)
                    {
                        failed += 1;
                        Console.WriteLine(ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                        Console.WriteLine("...FAILED");
                        DataLogger.ReportError(ex, true);
                    }
                }

                // test all scripts in sub - directories
                string[] directories = Directory.GetDirectories(rootDirectory);
                foreach (string directory in directories)
                {
                    RunScriptTestsInDirectory(directory);
                }
            }

            RunScriptTestsInDirectory(ScriptDirectoryPath);

            DataLogger.WriteToLogAndConsole($"Passed tests: {passed}");

            if (failed > 0)
            {
                DataLogger.WriteToLogAndConsole($"Failed tests: {failed}");
            }
            return failed == 0;
        }
    }
}
