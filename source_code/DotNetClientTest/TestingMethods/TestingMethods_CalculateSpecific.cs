#if !RSECTION

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
        public static bool Test_Calculate_Specific()
        {
            try
            {
                CalculateSpecific();
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }

            DataLogger.ResetProgressBar();
            DataLogger.AddLogEnd(LogResultType.DONE);
            return true;
        }

        private static void CalculateSpecific()
        {
            InitializeTest();
            DataLogger.AddLogStart("Calculating...");
            DataLogger.InitializeProgressBar();
            var calculationLoadings = GetCalculateSpecificLoadings();
            SetLoadCase();
            SetLoadCombination();

            calculation_message[] calculationMessages = SoapModelClient.calculate_specific(calculationLoadings, true);
            DataLogger.SetProgressBarValue(50);

            DataLogger.IncrementOffset();
            if(calculationMessages?.Length == 0)
            {
                DataLogger.AddText("All calculations was succesfull");
            }

            for (int i = 0; i < calculationMessages?.Length; i++)
            {
                LogCalculationMessages(calculationMessages[i]);
            }
            DataLogger.DecrementOffset();
            DataLogger.SetProgressBarValue(100);
        }

        private static void LogCalculationMessages(calculation_message calculationMessage)
        {
            DataLogger.AddText($"Calculation failed");
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Message type: {calculationMessage.message_type}");
            DataLogger.AddText($"Message: {calculationMessage.message}");
            DataLogger.AddText($"Current value: {calculationMessage.current_value}");
            DataLogger.AddText($"Object: {calculationMessage.@object}");
            DataLogger.AddText($"Input field: {calculationMessage.input_field}");
            DataLogger.AddText($"Result: {calculationMessage.result}");
            DataLogger.DecrementOffset();
        }

        private static calculate_specific_loading[] GetCalculateSpecificLoadings()
        {
            var calculationLoadings = new calculate_specific_loading[]
            {
                new calculate_specific_loading()
                {
                    no = 1,
                    type = case_object_types.E_OBJECT_TYPE_LOAD_COMBINATION,
                },
                new calculate_specific_loading()
                {
                    no = 1,
                    type = case_object_types.E_OBJECT_TYPE_LOAD_CASE,
                }
            };
            return calculationLoadings;
        }

        private static void SetLoadCombination()
        {
            load_combination combination = new load_combination()
            {
                no = 1,
            };
            SoapModelClient.set_load_combination(combination);
    }

        private static void SetLoadCase()
        {
            load_case combination = new load_case()
            {
                no = 1,
            };
            SoapModelClient.set_load_case(combination);
        }
    }
}

#endif // !RSECTION