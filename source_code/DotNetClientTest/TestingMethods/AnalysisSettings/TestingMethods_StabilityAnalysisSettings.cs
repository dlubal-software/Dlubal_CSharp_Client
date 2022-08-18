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
        public static bool Test_Analysis⁀Settings_Stability_Get()
        {
            DataLogger.AddLogStart("Reading stability analysis settings...");
            try
            {
                ReadStabilityAnalysisSettings();
                DataLogger.AddLogEnd(LogResultType.DONE);
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            return true;
        }

        public static bool Test_Analysis⁀Settings_Stability_Create()
        {
            DataLogger.AddLogStart("Creating stability analysis settings...");
            try
            {
                CreateStabiltiySimulationAnalysisSettings();
                DataLogger.AddLogEnd(LogResultType.DONE);
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            return true;
        }

        public static bool Test_Analysis⁀Settings_Stability_Delete()
        {
            DataLogger.AddLogStart("Deleting stability analysis settings...");
            try
            {
                bool result = DeleteObjects(object_types.E_OBJECT_TYPE_STABILITY_ANALYSIS_SETTINGS, 0, "Stability simulation analysis settings");
                DataLogger.AddLogEnd(LogResultType.DONE);
                return result;
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
        }

        private static void ReadStabilityAnalysisSettings()
        {
            int count = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_STABILITY_ANALYSIS_SETTINGS, 0);
            int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_STABILITY_ANALYSIS_SETTINGS, 0);
            if (count != numbers.Length)
            {
                throw new Exception("Object count does not match object number count.");
            }

            DataLogger.AddText($"{numbers.Length} objects have been read.");
            DataLogger.InitializeProgressBar(0, numbers.Length, 0);

            for (int i = 0; i < count; i++)
            {
                var settings = SoapModelClient.get_stability_analysis_settings(numbers[i]);
                LogStabilitylAnalysisSettings(settings);
                DataLogger.SetProgressBarValue(i);
            }
        }

        private static void LogStabilitylAnalysisSettings(stability_analysis_settings settings)
        {
            DataLogger.AddText($"No: {settings.no}");
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Name: {settings.name}");
            DataLogger.AddText($"Stabiltiy analysis type: {settings.analysis_type}");
            DataLogger.AddText($"Number of lowest eigenvalues: {settings.number_of_lowest_eigenvalues}");
            DataLogger.AddText($"Critical load factor: {settings.critical_load_factor}");
            DataLogger.DecrementOffset();
        }

        private static void CreateStabiltiySimulationAnalysisSettings()
        {
            SoapModelClient.begin_modification(nameof(CreateStabiltiySimulationAnalysisSettings));
            DataLogger.AddText("Generating stability analysis settings...");
            var settings = GetStabilitySimulationAnalysisSettings();
            SoapModelClient.set_stability_analysis_settings(settings);
            DataLogger.AddText("Generated stability analysis settings");
            SoapModelClient.finish_modification();
            DataLogger.SetProgressBarValue(100);
        }

        private static stability_analysis_settings GetStabilitySimulationAnalysisSettings()
        {
            var settings = new stability_analysis_settings()
            {
                no = 1,
                name = "Generated",
                number_of_lowest_eigenvalues = 16,
                number_of_lowest_eigenvaluesSpecified = true,
                find_eigenvectors_beyond_critical_load_factor = true,
                find_eigenvectors_beyond_critical_load_factorSpecified = true,
                critical_load_factor = 1523,
                critical_load_factorSpecified = true,
            };
            return settings;
        }
    }
}
