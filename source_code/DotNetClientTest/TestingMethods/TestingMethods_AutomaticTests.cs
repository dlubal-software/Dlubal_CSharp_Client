using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using static Dlubal.WS.Common.Tools.DataLogger;

#if RFEM
using Dlubal.WS.Rfem6.Model;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
#elif RSECTION
using Dlubal.WS.RSection1.Model;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public partial class TestingMethods
    {
        internal static List<string> ScriptDirectoryPaths;
        internal static List<string> SkipScripts;

        public static bool Test_Automatic\u2040Tests_Compare\u2040Current\u2040Data()
        {
            if (SoapApplicationClient.get_model_list().Length == 0)
            {
                return false;
            }

            bool succeeded = false;

#if RFEM || RSTAB
            _ = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_NODE, 0);
#elif RSECTION
            _ = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_POINT, 0);
#endif

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
            if (ScriptDirectoryPaths.Count == 0 && !Program.IsConsoleApplication)
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog
                {
                    Description = "Select folder with JS scripts for start automatic tests.",
                    ShowNewFolderButton = false
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ScriptDirectoryPaths.Add(dialog.SelectedPath);
                }
                else
                {
                    return false;
                }
            }

            if (SoapApplicationClient.get_model_list().Length == 0)
            {
                SoapApplicationClient.new_model("Test");
            }

#if RFEM || RSTAB
            SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_NODE, 0);
#elif RSECTION
            SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_POINT, 0);
#endif

            if (SoapModelClient == null)
            {
                return false;
            }

            int passed = 0;
            int failed = 0;

            void RunScriptTestsInDirectory(string rootDirectory)
            {
#if DEBUG
                bool browseSubDirectories = false;
                string[] scripts = new string[]
                {
                    Path.Combine(rootDirectory, @"internal\tests\loadCombinationsPrepare.js")
                };
#else
                // test all scripts in the root directory
                bool browseSubDirectories = false;
                string[] scripts = Directory.GetFiles(rootDirectory, "*Prepare.js");
#endif
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

                // test all scripts in sub-directories
                if (browseSubDirectories)
                {
                    string[] directories = Directory.GetDirectories(rootDirectory);
                    foreach (string directory in directories)
                    {
                        RunScriptTestsInDirectory(directory);
                    }
                }
            }

            foreach (string scriptDirectoryPath in ScriptDirectoryPaths)
            {
                if (Directory.Exists(scriptDirectoryPath))
                {
                    RunScriptTestsInDirectory(scriptDirectoryPath);
                }
            }

            DataLogger.WriteToLogAndConsole($"Passed tests: {passed}");

            if (failed > 0)
            {
                DataLogger.WriteToLogAndConsole($"Failed tests: {failed}");
            }
            return failed == 0;
        }
    }
}
