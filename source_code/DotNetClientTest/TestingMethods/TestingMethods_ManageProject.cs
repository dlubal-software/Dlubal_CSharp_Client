using System;
using static Dlubal.WS.Common.Tools.DataLogger;
using System.IO;

#if RFEM
using Dlubal.WS.Rfem6.Application;
using Dlubal.WS.Rfem6.Model;
#elif RSTAB
using Dlubal.WS.Rstab9.Application;
using Dlubal.WS.Rstab9.Model;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
        public static bool Test_Manage⁀Project_Create⁀New⁀Model⁀As⁀Copy()
        {
#if RFEM
            const string EXTENSION = "rf6";
#else
            const string EXTENSION = "rs9";
#endif
            string newModelname = "Generated Model as copy";
            DataLogger.InitializeProgressBar(0, 100, 0);
            try
            {
                DataLogger.AddText("Fetching files...");
                var info = SoapApplicationClient.get_list_of_existing_projects();
                if(info == null || info.Length < 1)
                {
                    throw new Exception("You are not currently in project so you cant create as copy");
                }

                var folderPath = info[0].folder_path;
                var files = Directory.GetFiles(folderPath,$"*.{EXTENSION}");
                if(files.Length < 1)
                {
                    throw new Exception("No file found to be used as copy");
                }

                DataLogger.AddText("Files loaded");
                DataLogger.AddText("Creating new model");
                var modelPath = files[0];
                SoapApplicationClient.new_model_as_copy(newModelname, modelPath);
                DataLogger.AddText("Created new model");
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                DataLogger.AddLogEnd(LogResultType.FAILED);
                return false;
            }

            DataLogger.ResetProgressBar();
            DataLogger.AddLogEnd(LogResultType.DONE);
            return true;
        }

        public static bool Test_Manage⁀Project_Create⁀New⁀Model⁀From⁀Template()
        {
#if RFEM
            const string EXTENSION = "ft6";
#else
            const string EXTENSION = "st9";
#endif
            string newModelname = "Generated Model from template";
            DataLogger.InitializeProgressBar(0, 100, 0);
            try
            {
                DataLogger.AddText("Fetching files...");
                var info = SoapApplicationClient.get_list_of_existing_templates();
                string folderPath;
                if(info.Length > 0 && Directory.Exists(info[0].folder_path))
                {
                    folderPath = info[0].folder_path;
                }
                else
                {
                    throw new Exception("Template category doesnt exist, create one and then try again.");
                }

                var files = Directory.GetFiles(folderPath, $"*.{EXTENSION}");
                if (files.Length < 1)
                {
                    throw new Exception("No file found to be used as copy");
                }

                DataLogger.AddText("Files loaded");
                DataLogger.AddText("Creating new model");
                var templatePath = files[0];
                SoapApplicationClient.new_model_from_template(newModelname, templatePath);
                DataLogger.AddText("Created new model");
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                DataLogger.AddLogEnd(LogResultType.FAILED);
                return false;
            }

            DataLogger.ResetProgressBar();
            DataLogger.AddLogEnd(LogResultType.DONE);
            return true;
        }

        public static bool Test_Manage⁀Project_Create⁀New⁀Project()
        {
            DataLogger.InitializeProgressBar(0, 100, 0);
            var path = Directory.GetCurrentDirectory() + "\\ProjectFolder";
            try
            {
                project_info info = new project_info
                {
                    description = "Generated project",
                    folder_path = path,
                    name = "Testing",
                    parent_path = "",
                };
                DataLogger.AddText("Creating new project");
                DataLogger.SetProgressBarValue(50);
                SoapApplicationClient.new_project(info);
                DataLogger.AddText("Created new project");
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                DataLogger.AddLogEnd(LogResultType.FAILED);
                return false;
            }

            DataLogger.ResetProgressBar();
            DataLogger.AddLogEnd(LogResultType.DONE);
            return true;
        }

        public static bool Test_Manage⁀Project_Get⁀Project()
        {
            DataLogger.InitializeProgressBar(0, 100, 0);
            try
            {
                DataLogger.AddText("Getting projects...");
                var infos = SoapApplicationClient.get_list_of_existing_projects();
                if (infos == null || infos.Length < 1)
                {
                    throw new Exception("You are not currently in project so you cant create as copy");
                }
                var path = $@"{infos[0].folder_path}\{infos[0].name}";
                DataLogger.SetProgressBarValue(50);
                var info = SoapApplicationClient.get_project(path);
                LogProjectInfo(info);
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                DataLogger.AddLogEnd(LogResultType.FAILED);
                return false;
            }

            DataLogger.ResetProgressBar();
            DataLogger.AddLogEnd(LogResultType.DONE);
            return true;
        }

        public static bool Test_Manage⁀Project_Delete⁀Project()
        {
            DataLogger.InitializeProgressBar(0, 100, 0);
            try
            {
                var infos = SoapApplicationClient.get_list_of_existing_projects();
                if (infos == null || infos.Length < 1)
                {
                    throw new Exception("There are no projects to delete!");
                }
                var path = $@"{infos[0].folder_path}\{infos[0].name}";
                DataLogger.AddText("Deleting project...");
                DataLogger.SetProgressBarValue(50);
                SoapApplicationClient.delete_project(path);
                DataLogger.AddText("Project deleted");
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                DataLogger.AddLogEnd(LogResultType.FAILED);
                return false;
            }

            DataLogger.ResetProgressBar();
            DataLogger.AddLogEnd(LogResultType.DONE);
            return true;
        }

        public static bool Test_Manage⁀Project_Get⁀Current⁀Project()
        {
            DataLogger.InitializeProgressBar(0, 100, 0);
            try
            {
                DataLogger.AddText("Getting project...");
                var info = SoapApplicationClient.get_current_project();
                DataLogger.AddText("Project loaded");
                LogProjectInfo(info);
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                DataLogger.AddLogEnd(LogResultType.FAILED);
                return false;
            }

            DataLogger.ResetProgressBar();
            DataLogger.AddLogEnd(LogResultType.DONE);
            return true;
        }

        public static bool Test_Manage⁀Project_Get⁀List⁀Of⁀Existing⁀Projects()
        {
            DataLogger.InitializeProgressBar(0, 100, 0);
            try
            {
                DataLogger.AddText("Getting project...");
                project_info[] infos = SoapApplicationClient.get_list_of_existing_projects();
                if(infos is null)
                {
                    throw new Exception("Projects info is null");
                }

                DataLogger.AddText($"{infos.Length} projects loaded");
                DataLogger.SetProgressBarValue(50);
                foreach (project_info info in infos)
                {
                    LogProjectInfo(info);
                }
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                DataLogger.AddLogEnd(LogResultType.FAILED);
                return false;
            }
            DataLogger.ResetProgressBar();
            DataLogger.AddLogEnd(LogResultType.DONE);
            return true;
        }

        public static bool Test_Manage⁀Project_Set⁀as⁀Current⁀Project()
        {
            DataLogger.InitializeProgressBar(0, 100, 0);
            try
            {
                DataLogger.AddText("Getting project...");
                var infos = SoapApplicationClient.get_list_of_existing_projects();
                if (infos == null || infos.Length < 1)
                {
                    throw new Exception("No available projects");
                }
                var path = $@"{infos[0].folder_path}\{infos[0].name}";
                DataLogger.SetProgressBarValue(50);
                SoapApplicationClient.set_as_current_project(path);
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                DataLogger.AddLogEnd(LogResultType.FAILED);
                return false;
            }

            DataLogger.ResetProgressBar();
            DataLogger.AddLogEnd(LogResultType.DONE);
            return true;
        }

        private static void LogProjectInfo(project_info info)
        {
            DataLogger.AddText($"Project name: {info.name}");
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Description: {info.description}");
            DataLogger.AddText($"Folder path: {info.folder_path}");
            DataLogger.AddText($"Parent path: {info.parent_path}");
            DataLogger.DecrementOffset();
        }
    }
}