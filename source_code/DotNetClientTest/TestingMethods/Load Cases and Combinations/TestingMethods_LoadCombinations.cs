#if RFEM || RSTAB

using Dlubal.WS.Common.Tools;
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
        /// Sets load combination.
        /// </summary>
        /// <param name="combinationNo">Load combination number. If zero next free id will be used.</param>
        /// <param name="caseCount">Count of load cases to be set to load combination.</param>
        /// <param name="withImperfectionCase">Create load combination with imperfections?</param>
        /// <returns>User id of the load combination.</returns>
        private static void SetLoadCombination(int combinationNo, int caseCount, bool withImperfectionCase = false)
        {
            const int DESIGN_SITUATION_NO = 1;
            const int IMPERFECTION_CASE_1_NO = 1;

            DataLogger.AddText($"Set load combination No.{combinationNo}");

            // set design situation
            design_situation designSituation = new design_situation
            {
                no = DESIGN_SITUATION_NO
            };
            SoapModelClient.set_design_situation(designSituation);

            // set load combination
            load_combination loadCombination = new load_combination
            {
                no = combinationNo,
                name = $"load combination {combinationNo} name",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                to_solve = true,
                to_solveSpecified = true,
                design_situation = designSituation.no,
                design_situationSpecified = true,
                comment = $"load combination {combinationNo} comment"
            };

            // set load combination items
            loadCombination.items = new load_combination_items_row[caseCount];
            for (int i = 0; i < caseCount; i++)
            {
                int caseId = i + 1;
                SetLoadCase(caseId);

                loadCombination.items[i] = new load_combination_items_row
                {
                    no = i + 1,
                    row = new load_combination_items
                    {
                        factor = 1.0 + (caseId / 10.0),
                        factorSpecified = true,
                        load_case = caseId,
                        load_caseSpecified = true
                    }
                };
            }

            // set imperfection case
            if (withImperfectionCase)
            {
                imperfection_case imperfection_Case = new imperfection_case
                {
                    no = IMPERFECTION_CASE_1_NO,
                    type = imperfection_case_type.IMPERFECTION_TYPE_LOCAL_IMPERFECTIONS,
                    typeSpecified = true
                };
                SoapModelClient.set_imperfection_case(imperfection_Case);

                loadCombination.imperfection_case = IMPERFECTION_CASE_1_NO;
                loadCombination.imperfection_caseSpecified = true;

                loadCombination.consider_imperfection = true;
                loadCombination.consider_imperfectionSpecified = true;
            }

            SoapModelClient.set_load_combination(loadCombination);
        }

        /// <summary>
        /// Creates load combinations.
        /// </summary>
        public static bool Test_Load\u2040Cases\u2040and\u2040Combinations_Load\u2040Combinations_Create()
        {
            const int LOAD_COMBINATION_1_ID = 1;
            const int LOAD_COMBINATION_2_ID = 2;

            DataLogger.AddLogStart("Creating load combinations...");
            DataLogger.InitializeProgressBar(0, 30, 0);

            bool succeeded = InitializeTest();
            if (!succeeded)
            {
                return false;
            }

            DataLogger.SetProgressBarValue(10);

            try
            {
                SoapModelClient.begin_modification("set load combinations");

                SetLoadCombination(LOAD_COMBINATION_1_ID, 3, false);

                DataLogger.SetProgressBarValue(20);

                SetLoadCombination(LOAD_COMBINATION_2_ID, 4, true);

                DataLogger.SetProgressBarValue(30);
                succeeded = true;
            }
            catch (Exception exception)
            {
                succeeded = false;
                ModelWsExceptionHandler(exception);
            }
            finally
            {
                try
                {
                    SoapModelClient.finish_modification();
                    DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
                }
                catch (Exception exception)
                {
                    succeeded = false;
                    ModelWsExceptionHandler(exception);
                    SoapModelClient.reset();
                }

                DataLogger.ResetProgressBar();
            }

            return succeeded;
        }

        /// <summary>
        /// Creates load combination and change count of defined LCs.
        /// </summary>
        public static bool Test_Load\u2040Cases\u2040and\u2040Combinations_Load\u2040Combinations_Modify()
        {
            const int LOAD_COMBINATION_1_ID = 1;

            DataLogger.AddLogStart("Creating load combination...");
            DataLogger.InitializeProgressBar(0, 50, 0);

            bool succeeded = InitializeTest();
            if (!succeeded)
            {
                return false;
            }

            DataLogger.SetProgressBarValue(10);

            try
            {
                bool ValidateItemCount(int coNo, int itemsCount)
                {
                    load_combination co = SoapModelClient.get_load_combination(coNo);

                    if (co.items.Length != itemsCount)
                    {
                        DataLogger.AddError($"Verification failed - count of items does not match {co.items.Length} != {itemsCount}");
                        return false;
                    }

                    DataLogger.AddText("Verification succeeded");
                    return true;
                }

                SoapModelClient.begin_modification("set load combinations");

                SetLoadCombination(LOAD_COMBINATION_1_ID, 1, false);
                succeeded &= ValidateItemCount(LOAD_COMBINATION_1_ID, 1);

                DataLogger.SetProgressBarValue(20);

                SetLoadCombination(LOAD_COMBINATION_1_ID, 2, false);
                succeeded &= ValidateItemCount(LOAD_COMBINATION_1_ID, 2);

                DataLogger.SetProgressBarValue(30);

                SetLoadCombination(LOAD_COMBINATION_1_ID, 5, false);
                succeeded &= ValidateItemCount(LOAD_COMBINATION_1_ID, 5);

                DataLogger.SetProgressBarValue(40);

                SetLoadCombination(LOAD_COMBINATION_1_ID, 3, false);
                succeeded &= ValidateItemCount(LOAD_COMBINATION_1_ID, 3);

                DataLogger.SetProgressBarValue(50);
                succeeded = true;
            }
            catch (Exception exception)
            {
                succeeded = false;
                ModelWsExceptionHandler(exception);
            }
            finally
            {
                try
                {
                    SoapModelClient.finish_modification();
                    DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
                }
                catch (Exception exception)
                {
                    succeeded = false;
                    ModelWsExceptionHandler(exception);
                    SoapModelClient.reset();
                }

                DataLogger.ResetProgressBar();
            }

            return succeeded;
        }
    }
}

#endif // RFEM RSTAB