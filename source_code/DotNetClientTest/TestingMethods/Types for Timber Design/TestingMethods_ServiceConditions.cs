#if !RSECTION

using System;
using Dlubal.WS.Clients.DotNetClientTest.Tools;
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
        public static bool Test_Types⁀for⁀Timber⁀Design_Service⁀Conditions_Get()
        {
            try
            {
                ReadTimberServiceConditions();
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

        private static void ReadTimberServiceConditions()
        {
            DataLogger.AddLogStart("Reading...");
            int count = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_TIMBER_SERVICE_CONDITIONS, 0);
            int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_TIMBER_SERVICE_CONDITIONS, 0);
            if (count != numbers.Length)
            {
                throw new Exception("Object count does not match object number count.");
            }

            DataLogger.AddText($"{numbers.Length} objects have been read.");
            DataLogger.InitializeProgressBar(0, numbers.Length, 0);
            for (int i = 0; i < count; i++)
            {
                timber_service_conditions objectToRead = SoapModelClient.get_timber_service_conditions(numbers[i]);
                LogTimberServiceConditions(objectToRead);
                DataLogger.SetProgressBarValue(i);
            }
        }

        private static void LogTimberServiceConditions(timber_service_conditions condition)
        {
            DataLogger.IncrementOffset();
            DataLogger.AddText($"No.{condition.no}");
            DataLogger.AddText($"Name: {condition.name}");
            DataLogger.AddText($"Treatment: {condition.treatment}");
            DataLogger.DecrementOffset();
        }

        public static bool Test_Types⁀for⁀Timber⁀Design_Service⁀Conditions_Create()
        {
            DataLogger.InitializeProgressBar(0, 50, 0);
            bool result = Test_General_Delete⁀All();
            if (!result || (SoapModelClient == null))
            {
                DataLogger.AddLogEnd(LogResultType.FAILED);
                return false;
            }

            DataLogger.SetProgressBarValue(10);
            try
            {
                CreateTimberServiceCondition();
                DataLogger.ResetProgressBar();
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                DataLogger.AddLogEnd(LogResultType.FAILED);
                return false;
            }

            DataLogger.AddLogEnd(LogResultType.DONE);
            return true;
        }

        private static void CreateTimberServiceCondition()
        {
            SoapModelClient.begin_modification(nameof(CreateTimberServiceCondition));
            DataLogger.SetProgressBarValue(20);
            DataLogger.AddText("Generating...");
            var condition = new timber_service_conditions
            {
                no = 1,
                name = "Generated",
                treatment = timber_service_conditions_treatment.TREATMENT_TYPE_PRESERVATIVE,
                treatmentSpecified = true,
            };
            SoapModelClient.set_timber_service_conditions(condition);
            SoapModelClient.finish_modification();
            DataLogger.AddText("Generated");
            DataLogger.SetProgressBarValue(50);
        }

        public static bool Test_Types⁀for⁀Timber⁀Design_Service⁀Conditions_Delete()
        {
            DataLogger.AddLogStart("Deleting...");
            try
            {
                bool result = DeleteObjects(object_types.E_OBJECT_TYPE_TIMBER_SERVICE_CONDITIONS, 0, "Service conditions");
                DataLogger.AddLogEnd(LogResultType.DONE);
                return result;
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                DataLogger.AddLogEnd(LogResultType.FAILED);
                return false;
            }
        }
    }
}

#endif // !RSECTION
