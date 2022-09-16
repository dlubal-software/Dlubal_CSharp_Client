#if RFEM

using Dlubal.WS.Rfem6.Model;
using System;
using static Dlubal.WS.Common.Tools.DataLogger;

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
        public static bool Test_Main⁀Object⁀To⁀Activate_Get()
        {
            try
            {
                ReadMainObject();
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

        private static void ReadMainObject()
        {
            DataLogger.AddLogStart("Reading...");
            DataLogger.InitializeProgressBar();
            main_objects_to_activate objectToRead = SoapModelClient.get_main_objects_to_activate();

            LogMainObjectToActivate(objectToRead);
        }

        private static void LogMainObjectToActivate(main_objects_to_activate mainObject)
        {
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Activate members: {mainObject.activate_members}");
            DataLogger.AddText($"Activate solids: {mainObject.activate_solids}");
            DataLogger.AddText($"Activate surfaces: {mainObject.activate_surfaces}");
            DataLogger.DecrementOffset();
        }

        public static bool Test_Main⁀Object⁀To⁀Activate_Set()
        {
            DataLogger.AddLogStart("Creating...");
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
                CreateMainObjectToActivate();
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

        private static void CreateMainObjectToActivate()
        {
            SoapModelClient.begin_modification(nameof(CreateMainObjectToActivate));
            DataLogger.SetProgressBarValue(20);

            DataLogger.AddText("Generating...");
            var mainObject = GetMainObjectToActivate();
            SoapModelClient.set_main_objects_to_activate(mainObject);
            DataLogger.AddText("Generated");

            SoapModelClient.finish_modification();
            DataLogger.SetProgressBarValue(50);
        }

        private static main_objects_to_activate GetMainObjectToActivate()
        {
            var mainObject = new main_objects_to_activate()
            {
                activate_members = true,
                activate_membersSpecified = true,
                activate_solids = false,
                activate_solidsSpecified = true,
                activate_surfaces = true,
                activate_surfacesSpecified = true,
            };
            return mainObject;
        }
    }
}

#endif // RFEM
