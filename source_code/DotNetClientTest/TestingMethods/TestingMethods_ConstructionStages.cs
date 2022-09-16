#if RFEM

using Dlubal.WS.Common.Tools;
using Dlubal.WS.Rfem6.Model;
using System;
using static Dlubal.WS.Common.Tools.DataLogger;

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
        public static bool Test_Construction⁀Stage_Create()
        {
            DataLogger.AddText("Creating construction stage...");
            try
            {
                CreateConstructionStage();
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            return true;
        }

        public static bool Test_Construction⁀Stage_Get()
        {
            DataLogger.AddText("Reading construction stage...");
            try
            {
                ReadConstructionStage();
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            return true;
        }

        public static bool Test_Construction⁀Stage_Delete()
        {
            DataLogger.AddText("Deleting construction stage...");
            try
            {
                bool result = DeleteObjects(object_types.E_OBJECT_TYPE_CONSTRUCTION_STAGE, 0, "Construction stage");
                DataLogger.AddLogEnd(LogResultType.DONE);
                return result;
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
        }

        private static void ReadConstructionStage()
        {
            int count = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_CONSTRUCTION_STAGE, 0);
            int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_CONSTRUCTION_STAGE, 0);
            if (count != numbers.Length)
            {
                throw new Exception("Object count does not match object number count.");
            }

            DataLogger.AddText($"{numbers.Length} objects have been read.");
            DataLogger.InitializeProgressBar(0, numbers.Length, 0);

            for (int i = 0; i < count; i++)
            {
                var stage = SoapModelClient.get_construction_stage(numbers[i]);
                LogConstructionStage(stage);
                DataLogger.SetProgressBarValue(i);
            }
        }

        private static void LogConstructionStage(construction_stage stage)
        {
            DataLogger.AddText($"No: {stage.no}");
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Name: {stage.name}");
            DataLogger.AddText($"End time and date: {stage.end_date}");
            DataLogger.AddText($"Stage duration: {stage.duration}");
            foreach (construction_stage_loading_row row in stage.loading)
            {
                LogConstructionStageLoadingRow(row);
            }
            DataLogger.DecrementOffset();
        }

        private static void LogConstructionStageLoadingRow(construction_stage_loading_row row)
        {
            DataLogger.AddText($"No: {row.no}");
            DataLogger.IncrementOffset();
            LogConstructionStageLoading(row.row);
            DataLogger.DecrementOffset();
        }

        private static void LogConstructionStageLoading(construction_stage_loading stageLoading)
        {
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Load case: {stageLoading.load_case}");
            DataLogger.AddText($"Status type: {stageLoading.status_type}");
            DataLogger.AddText($"Permanent: {stageLoading.permanent}");
            DataLogger.AddText($"Factor: {stageLoading.factor}");
            DataLogger.DecrementOffset();
        }

        private static void CreateConstructionStage()
        {
            SoapModelClient.begin_modification(nameof(CreateConstructionStage));
            DataLogger.AddText("Adding static settings...");
            static_analysis_settings staticSettings = new static_analysis_settings()
            {
                no = 1,
            };
            SoapModelClient.set_static_analysis_settings(staticSettings);
            DataLogger.AddText("Added static settings");

            DataLogger.AddText("Adding stability settings...");
            stability_analysis_settings stabilitySettings = new stability_analysis_settings()
            {
                no = 1,
            };
            SoapModelClient.set_stability_analysis_settings(stabilitySettings);
            DataLogger.AddText("Added stability settings");

            DataLogger.AddText("Generating construction stage...");
            construction_stage stage = GetConstructionStage();
            SoapModelClient.set_construction_stage(stage);
            DataLogger.AddText("Generated construction stage");

            SoapModelClient.finish_modification();
            DataLogger.SetProgressBarValue(100);
        }

        private static construction_stage GetConstructionStage()
        {
            construction_stage_loading_row[] rows = new construction_stage_loading_row[]
            {
                new construction_stage_loading_row()
                {
                    no = 1,
                    row = new construction_stage_loading()
                    {
                        load_case = 1,
                        load_caseSpecified = true,
                        status_type = status_type.NONE,
                        status_typeSpecified = true,
                        permanent = true,
                        permanentSpecified = true,
                        factor = 0,
                        factorSpecified = true,
                     }
                },
                new construction_stage_loading_row()
                {
                    no = 2,
                    row = new construction_stage_loading()
                    {
                        load_case = 2,
                        load_caseSpecified = true,
                        status_type = status_type.NONE,
                        status_typeSpecified = true,
                        permanent = true,
                        permanentSpecified = true,
                        factor = 0,
                        factorSpecified = true,
                    }
                }
            };

            construction_stage stage = new construction_stage()
            {
                no = 1,
                name = "Generated",
                static_analysis_settings = 1,
                static_analysis_settingsSpecified = true,
                stability_analysis = 1,
                stability_analysisSpecified = true,
                stability_analysis_enabled = true,
                stability_analysis_enabledSpecified = true,
                loading = rows,
            };
            return stage;
        }
    }
}

#endif // RFEM
