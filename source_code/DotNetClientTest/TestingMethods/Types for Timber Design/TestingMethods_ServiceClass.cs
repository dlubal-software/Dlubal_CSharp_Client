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
        public static bool Test_Types⁀for⁀Timber⁀Design_Service⁀Class_Get()
        {
            try
            {
                ReadTimberServiceClasses();
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

        private static void ReadTimberServiceClasses()
        {
            DataLogger.AddLogStart("Reading...");
            int count = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_TIMBER_SERVICE_CLASS, 0);
            int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_TIMBER_SERVICE_CLASS, 0);
            if (count != numbers.Length)
            {
                throw new Exception("Object count does not match object number count.");
            }

            DataLogger.AddText($"{numbers.Length} objects have been read.");
            DataLogger.InitializeProgressBar(0, numbers.Length, 0);
            for (int i = 0; i < count; i++)
            {
                timber_service_class objectToRead = SoapModelClient.get_timber_service_class(numbers[i]);
                LogTimberServiceClasses(objectToRead);
                DataLogger.SetProgressBarValue(i);
            }
        }

        private static void LogTimberServiceClasses(timber_service_class condition)
        {
            DataLogger.IncrementOffset();
            DataLogger.AddText($"No.{condition.no}");
            DataLogger.AddText($"Name: {condition.name}");
            DataLogger.AddText($"Service class: {condition.service_class}");
            DataLogger.DecrementOffset();
        }

        public static bool Test_Types⁀for⁀Timber⁀Design_Service⁀Class_Create()
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
                CreateTimberServiceClasses();
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

        private static void CreateTimberServiceClasses()
        {
            SoapModelClient.begin_modification(nameof(CreateTimberServiceClasses));
            DataLogger.SetProgressBarValue(20);
            DataLogger.AddText("Generating...");
            var serviceClass = new timber_service_class
            {
                no = 1,
                name = "Generated 1",
                service_class = timber_service_class_service_class.TIMBER_SERVICE_CLASS_TYPE_1,
                service_classSpecified = true,
            };
            var serviceClass2 = new timber_service_class
            {
                no = 2,
                name = "Generated 2",
                service_class = timber_service_class_service_class.TIMBER_SERVICE_CLASS_TYPE_2,
                service_classSpecified = true,
            };
            var serviceClass3 = new timber_service_class
            {
                no = 3,
                name = "Generated 3",
                service_class = timber_service_class_service_class.TIMBER_SERVICE_CLASS_TYPE_3,
                service_classSpecified = true,
            };
            SoapModelClient.set_timber_service_class(serviceClass);
            SoapModelClient.set_timber_service_class(serviceClass2);
            SoapModelClient.set_timber_service_class(serviceClass3);
            SoapModelClient.finish_modification();
            DataLogger.AddText("Generated");
            DataLogger.SetProgressBarValue(50);
        }

        public static bool Test_Types⁀for⁀Timber⁀Design_Service⁀Class_Delete()
        {
            DataLogger.AddLogStart("Deleting...");
            try
            {
                bool result = DeleteObjects(object_types.E_OBJECT_TYPE_TIMBER_SERVICE_CLASS, 0, "Service class");
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
