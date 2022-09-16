#if !RSECTION

using Dlubal.WS.Common.Tools;
using System;
using static Dlubal.WS.Common.Tools.DataLogger;

#if RFEM
using Dlubal.WS.Rfem6.Model;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
        public static bool Test_Analysis⁀Settings_Modal_Create()
        {
            DataLogger.AddLogStart("Creating modal analysis settings...");
            try
            {
                CreateModalSimulationAnalysisSettings();
                DataLogger.AddLogEnd(LogResultType.DONE);
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            return true;
        }

        public static bool Test_Analysis⁀Settings_Modal_Get()
        {
            DataLogger.AddLogStart("Reading modal analysis settings...");
            try
            {
                ReadModalAnalysisSettings();
                DataLogger.AddLogEnd(LogResultType.DONE);
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            return true;
        }

        public static bool Test_Analysis⁀Settings_Modal_Delete()
        {
            DataLogger.AddLogStart("Deleting modal analysis settings...");
            try
            {
                bool result = DeleteObjects(object_types.E_OBJECT_TYPE_MODAL_ANALYSIS_SETTINGS, 0, "Modal simulation analysis settings");
                DataLogger.AddLogEnd(LogResultType.DONE);
                return result;
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
        }

        private static void ReadModalAnalysisSettings()
        {
            int count = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_MODAL_ANALYSIS_SETTINGS, 0);
            int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_MODAL_ANALYSIS_SETTINGS, 0);
            if (count != numbers.Length)
            {
                throw new Exception("Object count does not match object number count.");
            }

            DataLogger.AddText($"{numbers.Length} objects have been read.");
            DataLogger.InitializeProgressBar(0, numbers.Length, 0);

            for (int i = 0; i < count; i++)
            {
                var settings = SoapModelClient.get_modal_analysis_settings(numbers[i]);
                LogModalAnalysisSettings(settings);
                DataLogger.SetProgressBarValue(i);
            }
        }

        private static void LogModalAnalysisSettings(modal_analysis_settings settings)
        {
            DataLogger.AddText($"No: {settings.no}");
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Name: {settings.name}");
            DataLogger.AddText($"Number of modes: {settings.number_of_modes}");
            DataLogger.AddText($"Method for determining the number of modes: {settings.number_of_modes_method}");
            DataLogger.AddText($"Solution method: {settings.solution_method}");
            DataLogger.AddText($"Mass matrix type: {settings.mass_matrix_type}");
            DataLogger.AddText($"Mass conversion type: {settings.mass_conversion_type}");
            DataLogger.AddText($"Activate minimum initial prestress: {settings.activate_minimum_initial_prestress}");
            DataLogger.DecrementOffset();
        }

        private static void CreateModalSimulationAnalysisSettings()
        {
            addon_list_type addon = new addon_list_type
            {
                dynamic_analysis_settings = new addon_list_dynamic_analysis_settings_list_type
                {
                    modal_active = true,
                    modal_activeSpecified = true
                }
            };

            SoapModelClient.set_addon_statuses(addon);

            SoapModelClient.begin_modification(nameof(CreateModalSimulationAnalysisSettings));
            DataLogger.AddText("Generating modal analysis settings...");
            var settings = GetModalSimulationAnalysisSettings();
            SoapModelClient.set_modal_analysis_settings(settings);
            DataLogger.AddText("Generated modal simulation analysis settings");
            SoapModelClient.finish_modification();
            DataLogger.SetProgressBarValue(100);
        }

        private static modal_analysis_settings GetModalSimulationAnalysisSettings()
        {
            var settings = new modal_analysis_settings()
            {
                no = 1,
                name = "Generated",
                number_of_modes = 15,
                number_of_modesSpecified = true,
                number_of_modes_method = modal_analysis_settings_number_of_modes_method.NUMBER_OF_MODES_METHOD_MAXIMUM_FREQUENCY,
                number_of_modes_methodSpecified = true,
                maxmimum_natural_frequency = 3200,
                maxmimum_natural_frequencySpecified = true,
#if RFEM
                solution_method = modal_analysis_settings_solution_method.METHOD_LANCZOS,
                solution_methodSpecified = true,
#elif RSTAB
                solution_method = modal_analysis_settings_solution_method.METHOD_SUBSPACE_ITERATION,
                solution_methodSpecified = true,
#endif

            };
            return settings;
        }
    }
}

#endif // !RSECTION
