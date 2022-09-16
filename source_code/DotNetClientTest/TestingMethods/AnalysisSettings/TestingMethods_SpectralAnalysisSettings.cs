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
        public static bool Test_Analysis⁀Settings_Spectral_Create()
        {
            DataLogger.AddLogStart("Creating spectral analysis settings...");
            try
            {
                CreateSpectralSimulationAnalysisSettings();
                DataLogger.AddLogEnd(LogResultType.DONE);
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            return true;
        }

        public static bool Test_Analysis⁀Settings_Spectral_Get()
        {
            DataLogger.AddLogStart("Reading spectral analysis settings...");
            try
            {
                ReadSpectralAnalysisSettings();
                DataLogger.AddLogEnd(LogResultType.DONE);
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            return true;
        }

        public static bool Test_Analysis⁀Settings_Spectral_Delete()
        {
            DataLogger.AddLogStart("Deleting spectral analysis settings...");
            try
            {
                bool result = DeleteObjects(object_types.E_OBJECT_TYPE_SPECTRAL_ANALYSIS_SETTINGS, 0, "Spectral simulation analysis settings");
                DataLogger.AddLogEnd(LogResultType.DONE);
                return result;
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
        }

        private static void ReadSpectralAnalysisSettings()
        {
            int count = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_SPECTRAL_ANALYSIS_SETTINGS, 0);
            int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_SPECTRAL_ANALYSIS_SETTINGS, 0);
            if (count != numbers.Length)
            {
                throw new Exception("Object count does not match object number count.");
            }

            DataLogger.AddText($"{numbers.Length} objects have been read.");
            DataLogger.InitializeProgressBar(0, numbers.Length, 0);

            for (int i = 0; i < count; i++)
            {
                var settings = SoapModelClient.get_spectral_analysis_settings(numbers[i]);
                LogSpectralAnalysisSettings(settings);
                DataLogger.SetProgressBarValue(i);
            }
        }

        private static void LogSpectralAnalysisSettings(spectral_analysis_settings settings)
        {
            DataLogger.AddText($"No: {settings.no}");
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Name: {settings.name}");
            DataLogger.AddText($"Combination rule: {settings.combination_rule_for_periodic_responses}");
            DataLogger.AddText($"Use equivalent linear combination: {settings.use_equivalent_linear_combination}");
            DataLogger.DecrementOffset();
        }

        private static void CreateSpectralSimulationAnalysisSettings()
        {
            addon_list_type addon = new addon_list_type
            {
                dynamic_analysis_settings = new addon_list_dynamic_analysis_settings_list_type
                {
                    spectral_active = true,
                    spectral_activeSpecified = true
                }
            };

            SoapModelClient.set_addon_statuses(addon);

            SoapModelClient.begin_modification(nameof(CreateSpectralSimulationAnalysisSettings));
            DataLogger.AddText("Generating spectral analysis settings...");
            var settings = GetSpectralSimulationAnalysisSettings();
            SoapModelClient.set_spectral_analysis_settings(settings);
            DataLogger.AddText("Generated spectral analysis settings");
            SoapModelClient.finish_modification();
            DataLogger.SetProgressBarValue(100);
        }

        private static spectral_analysis_settings GetSpectralSimulationAnalysisSettings()
        {
            var settings = new spectral_analysis_settings()
            {
                no = 1,
                name = "Generated",
                combination_rule_for_periodic_responses = spectral_analysis_settings_combination_rule_for_periodic_responses.SRSS,
                combination_rule_for_periodic_responsesSpecified = true,
                use_equivalent_linear_combination = true,
                use_equivalent_linear_combinationSpecified = true,
            };
            return settings;
        }
    }
}

#endif // !RSECTION
