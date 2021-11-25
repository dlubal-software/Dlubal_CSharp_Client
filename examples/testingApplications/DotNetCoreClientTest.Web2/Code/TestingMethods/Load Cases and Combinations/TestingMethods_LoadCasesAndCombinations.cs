using Dlubal.WS.RfemRstabSoapWsCoreLib.Tools;
using System;

#if RFEM
using Dlubal.WS.Rfem6.Model;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public partial class TestingMethods
    {
        public static bool Test_Load\u2040Cases\u2040and\u2040Combinations_Settings_Get()
        {
            DataLogger.AddLogStart("Get load cases and combination settings...");
            DataLogger.InitializeProgressBar(0, 10, 0);

            bool succeeded = InitializeTest();
            if (!succeeded)
            {
                return false;
            }

            try
            {
                load_cases_and_combinations settings = SoapModelClient.get_load_cases_and_combinations();

                DumpObject<load_cases_and_combinations>(settings);

                DataLogger.SetProgressBarValue(10);
            }
            catch (Exception ex)
            {
                succeeded = false;
                ModelWsExceptionHandler(ex);
                return false;
            }

            DataLogger.AddLogEnd(succeeded ? DataLogger.LogResultType.DONE : DataLogger.LogResultType.FAILED);
            DataLogger.ResetProgressBar();
            return succeeded;
        }

        public static bool Test_Load\u2040Cases\u2040and\u2040Combinations_Settings_Set()
        {
            DataLogger.AddLogStart("Get load cases and combination settings...");
            DataLogger.InitializeProgressBar(0, 10, 0);

            bool succeeded = InitializeTest();
            if (!succeeded)
            {
                return false;
            }

            try
            {
                string[] ignorePropertiesList = new string[0];

                load_cases_and_combinations settingsWritten = new load_cases_and_combinations
                {
                    activate_combination_wizard = false,
                    activate_combination_wizardSpecified = true,
                    activate_combination_wizard_and_classification = true,
                    activate_combination_wizard_and_classificationSpecified = true,
                    combination_name_according_to_action_category = false,
                    combination_name_according_to_action_categorySpecified = true,
                    current_standard_for_combination_wizard = 6232,
                    current_standard_for_combination_wizardSpecified = true,
                    result_combinations_active = false,
                    result_combinations_activeSpecified = true,
                    result_combinations_consider_sub_results = false,
                    result_combinations_consider_sub_resultsSpecified = true,
                    result_combinations_parentheses_active = true,
                    result_combinations_parentheses_activeSpecified = true,
                };

                SoapModelClient.set_load_cases_and_combinations(settingsWritten);

                load_cases_and_combinations settingsRead = SoapModelClient.get_load_cases_and_combinations();

                AreObjectsEqual(settingsWritten, settingsRead, nameof(load_cases_and_combinations), ignorePropertiesList);

                DataLogger.SetProgressBarValue(10);

                settingsWritten = settingsRead;
                settingsWritten.activate_combination_wizard = true;
                settingsWritten.activate_combination_wizardSpecified = true;

                SoapModelClient.set_load_cases_and_combinations(settingsWritten);

                settingsRead = SoapModelClient.get_load_cases_and_combinations();

                AreObjectsEqual(settingsWritten, settingsRead, nameof(load_cases_and_combinations), ignorePropertiesList);

                DataLogger.SetProgressBarValue(20);
            }
            catch (Exception ex)
            {
                succeeded = false;
                ModelWsExceptionHandler(ex);
                return false;
            }

            DataLogger.AddLogEnd(succeeded ? DataLogger.LogResultType.DONE : DataLogger.LogResultType.FAILED);
            DataLogger.ResetProgressBar();
            return succeeded;
        }
    }
}
