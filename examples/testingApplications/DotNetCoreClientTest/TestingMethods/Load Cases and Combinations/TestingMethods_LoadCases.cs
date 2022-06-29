using Dlubal.WS.RfemRstabSoapWsCoreLib.Tools;
using System;

#if RFEM
using Dlubal.WS.Rfem6.Model;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
        /// <summary>
        /// Sets load case.
        /// </summary>
        /// <param name="loadCaseNo">Load case No.</param>
        /// <param name="imperfectionCaseNo">Imperfection No.</param>
        private static void SetLoadCase(int loadCaseNo, int imperfectionCaseNo = 0, bool noneActionCategory = false)
        {
            DataLogger.AddText($"Set load case No.{loadCaseNo}");

            // static analysis case
            int analysisCaseId = loadCaseNo;
            static_analysis_settings static_analysis_settings = new static_analysis_settings()
            {
                no = analysisCaseId,
            };
            SoapModelClient.set_static_analysis_settings(static_analysis_settings);

            // load case
            load_case loadCase = new load_case
            {
                no = loadCaseNo,

                name = $"load case {loadCaseNo} name",
                comment = $"load case {loadCaseNo} comment",
                to_solve = true,
                to_solveSpecified = true,

                self_weight_active = true,
                self_weight_activeSpecified = true,
                self_weight_factors = new vector_3d().SetCoordinates(0.1, 0.2, 0.3),

                static_analysis_settings = analysisCaseId,
                static_analysis_settingsSpecified = true
            };

            if (noneActionCategory)
            {
                loadCase.action_category = "None | None";
            }

            // imperfection
            if (imperfectionCaseNo > 0)
            {
                imperfection_case imperfection_Case = new imperfection_case
                {
                    no = imperfectionCaseNo,
                    //type = imperfection_case_type.IMPERFECTION_TYPE_LOCAL_IMPERFECTIONS_ONLY,
                    typeSpecified = true
                };
                SoapModelClient.set_imperfection_case(imperfection_Case);

                loadCase.imperfection_case = imperfectionCaseNo;
                loadCase.imperfection_caseSpecified = true;

                loadCase.consider_imperfection = true;
                loadCase.consider_imperfectionSpecified = true;
            }

            SoapModelClient.set_load_case(loadCase);
        }

        public static bool Test_Load\u2040Cases\u2040and\u2040Combinations_Load\u2040Cases_Create()
        {
            const int STATIC_ANALYSIS_SETTINGS_NO = 1;
            const int LOAD_CASE_1_NO = 2;
            const int LOAD_CASE_2_NO = 3;

            DataLogger.AddLogStart("Creating load cases...");
            DataLogger.InitializeProgressBar(0, 40, 0);

            bool result = Test_General_Delete⁀All();
            if (!result || (SoapModelClient == null))
            {
                DataLogger.AddLogEnd(DataLogger.LogResultType.FAILED);
                return false;
            }

            DataLogger.SetProgressBarValue(10);

            try
            {
                SoapModelClient.begin_modification("set load cases");

                // static analysis settings
                DataLogger.AddText($"Set static analysis settings No.{STATIC_ANALYSIS_SETTINGS_NO}");
                static_analysis_settings staticAnalysisSettings = new static_analysis_settings
                {
                    no = STATIC_ANALYSIS_SETTINGS_NO,
                    name = "Testing Load Case No.1"
                };
                SoapModelClient.set_static_analysis_settings(staticAnalysisSettings);

                DataLogger.SetProgressBarValue(20);

                // load cases
                DataLogger.AddText($"Set load case No.{LOAD_CASE_1_NO}");
                load_case loadCase1 = new load_case
                {
                    no = LOAD_CASE_1_NO,
                    name = "Testing Load Case No.1",
                    analysis_type = load_case_analysis_type.ANALYSIS_TYPE_STATIC,
                    analysis_typeSpecified = true,
                    static_analysis_settings = STATIC_ANALYSIS_SETTINGS_NO,
                    static_analysis_settingsSpecified = true,
                };
                SoapModelClient.set_load_case(loadCase1);

                DataLogger.SetProgressBarValue(30);

                DataLogger.AddText($"Set load case No.{LOAD_CASE_2_NO}");
                load_case loadCase2 = new load_case
                {
                    no = LOAD_CASE_2_NO,
                    name = "Testing Load Case No.2",
                    analysis_type = load_case_analysis_type.ANALYSIS_TYPE_STATIC,
                    analysis_typeSpecified = true,
                    static_analysis_settings = STATIC_ANALYSIS_SETTINGS_NO,
                    static_analysis_settingsSpecified = true,
                };
                SoapModelClient.set_load_case(loadCase2);

                DataLogger.SetProgressBarValue(40);
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            finally
            {
                try
                {
                    SoapModelClient.finish_modification();
                }
                catch (Exception exception)
                {
                    ModelWsExceptionHandler(exception);
                    SoapModelClient.reset();
                }

                DataLogger.ResetProgressBar();
            }

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }

        public static bool Test_Load\u2040Cases\u2040and\u2040Combinations_Load\u2040Cases_Get()
        {
            DataLogger.AddLogStart("Reading load cases...");

            try
            {
                int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_LOAD_CASE, 0);

                DataLogger.AddText($"{numbers.Length} load case numbers have been read.");
                DataLogger.InitializeProgressBar(0, numbers.Length, 0);

                for (int i = 0; i < numbers.Length; i++)
                {
                    load_case loadCase = SoapModelClient.get_load_case(numbers[i]);

                    DataLogger.AddText($"Load case No.{numbers[i]}");
                    DataLogger.AddText($"  - Analysis type = {loadCase.analysis_type}");
                    DataLogger.SetProgressBarValue(i);
                }
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            finally
            {
                DataLogger.ResetProgressBar();
            }

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }

        public static bool Test_Load\u2040Cases\u2040and\u2040Combinations_Load\u2040Cases_Delete()
        {
            return DeleteObjects(object_types.E_OBJECT_TYPE_LOAD_CASE, 0, "load cases");
        }
    }
}