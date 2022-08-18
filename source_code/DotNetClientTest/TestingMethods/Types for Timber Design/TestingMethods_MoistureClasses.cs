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
        public static bool Test_Types⁀for⁀Timber⁀Design_Moisture⁀Classes_Get()
        {
            try
            {
                ReadTimberMoistureClasses();
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

        private static void ReadTimberMoistureClasses()
        {
            DataLogger.AddLogStart("Reading...");
            int count = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_TIMBER_MOISTURE_CLASS, 0);
            int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_TIMBER_MOISTURE_CLASS, 0);
            if (count != numbers.Length)
            {
                throw new Exception("Object count does not match object number count.");
            }

            DataLogger.AddText($"{numbers.Length} objects have been read.");
            DataLogger.InitializeProgressBar(0, numbers.Length, 0);
            for (int i = 0; i < count; i++)
            {
                timber_moisture_class objectToRead = SoapModelClient.get_timber_moisture_class(numbers[i]);
                LogTimberMoistureClasses(objectToRead);
                DataLogger.SetProgressBarValue(i);
            }
        }

        private static void LogTimberMoistureClasses(timber_moisture_class condition)
        {
            DataLogger.IncrementOffset();
            DataLogger.AddText($"No.{condition.no}");
            DataLogger.AddText($"Name: {condition.name}");
            DataLogger.DecrementOffset();
        }

        public static bool Test_Types⁀for⁀Timber⁀Design_Moisture⁀Classes_Create()
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
                CreateTimberMoistureClasses();
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

        private static void CreateTimberMoistureClasses()
        {
            SoapModelClient.begin_modification(nameof(CreateTimberMoistureClasses));
            DataLogger.SetProgressBarValue(20);
            DataLogger.AddText("Generating...");
            var moistureClass = new timber_moisture_class
            {
                no = 1,
                name = "Generated",
            };
            SoapModelClient.set_timber_moisture_class(moistureClass);
            SoapModelClient.finish_modification();
            DataLogger.AddText("Generated");
            DataLogger.SetProgressBarValue(50);
        }

        public static bool Test_Types⁀for⁀Timber⁀Design_Moisture⁀Classes_Delete()
        {
            DataLogger.AddLogStart("Deleting...");
            try
            {
                bool result = DeleteObjects(object_types.E_OBJECT_TYPE_TIMBER_MOISTURE_CLASS, 0, "Moisture class");
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
