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
        public static bool Test_Analysis⁀Settings_Static_Create()
        {
            DataLogger.AddLogStart("Creating static analysis settings...");
            try
            {
                CreateStaticSimulationAnalysisSettings();
                DataLogger.AddLogEnd(LogResultType.DONE);
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            return true;
        }

        public static bool Test_Analysis⁀Settings_Static_Get()
        {
            DataLogger.AddLogStart("Reading static analysis settings...");
            try
            {
                ReadStaticAnalysisSettings();
                DataLogger.AddLogEnd(LogResultType.DONE);
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            return true;
        }

        public static bool Test_Analysis⁀Settings_Static_Delete()
        {
            DataLogger.AddLogStart("Deleting static analysis settings...");
            try
            {
                bool result = DeleteObjects(object_types.E_OBJECT_TYPE_STATIC_ANALYSIS_SETTINGS, 0, "Static simulation analysis settings");
                DataLogger.AddLogEnd(LogResultType.DONE);
                return result;
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
        }

        private static void ReadStaticAnalysisSettings()
        {
            int count = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_STATIC_ANALYSIS_SETTINGS, 0);
            int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_STATIC_ANALYSIS_SETTINGS, 0);
            if (count != numbers.Length)
            {
                throw new Exception("Object count does not match object number count.");
            }

            DataLogger.AddText($"{numbers.Length} objects have been read.");
            DataLogger.InitializeProgressBar(0, numbers.Length, 0);

            for (int i = 0; i < count; i++)
            {
                var settings = SoapModelClient.get_static_analysis_settings(numbers[i]);
                LogStaticAnalysisSettings(settings);
                DataLogger.SetProgressBarValue(i);
            }
        }

        private static void LogStaticAnalysisSettings(static_analysis_settings settings)
        {
            DataLogger.AddText($"No: {settings.no}");
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Name: {settings.name}");
            DataLogger.AddText($"Analysis type: {settings.analysis_type}");
            DataLogger.AddText($"Precision and tolerance enabled: {settings.standard_precision_and_tolerance_settings_enabled}");
#if RFEM
            DataLogger.AddText($"Nonlinear method: {settings.iterative_method_for_nonlinear_analysisSpecified}");
            DataLogger.AddText($"Plate bending theory: {settings.plate_bending_theory}");
#endif
            DataLogger.DecrementOffset();
        }

        private static void CreateStaticSimulationAnalysisSettings()
        {
            SoapModelClient.begin_modification(nameof(CreateStaticSimulationAnalysisSettings));
            DataLogger.AddText("Generating static analysis settings...");
            var settings = GetStaticSimulationAnalysisSettings();
            SoapModelClient.set_static_analysis_settings(settings);
            DataLogger.AddText("Generated static analysis settings");
            SoapModelClient.finish_modification();
            DataLogger.SetProgressBarValue(100);
        }

        private static static_analysis_settings GetStaticSimulationAnalysisSettings()
        {
            var settings = new static_analysis_settings()
            {
                no = 1,
                name = "Generated",
                analysis_type = static_analysis_settings_analysis_type.LARGE_DEFORMATIONS,
                analysis_typeSpecified = true,
                standard_precision_and_tolerance_settings_enabled = true,
                standard_precision_and_tolerance_settings_enabledSpecified = true,
#if RFEM
                iterative_method_for_nonlinear_analysis = static_analysis_settings_iterative_method_for_nonlinear_analysis.DYNAMIC_RELAXATION,
                iterative_method_for_nonlinear_analysisSpecified = true,
                plate_bending_theory = static_analysis_settings_plate_bending_theory.PLATE_BENDING_THEORY_KIRCHHOFF,
                plate_bending_theorySpecified = true,
#endif
            };
            return settings;
        }
    }
}

#endif // !RSECTION
