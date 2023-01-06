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
        public static bool Test_Types⁀for⁀Timber⁀Design_Member⁀Shear⁀Panel_Get()
        {
            try
            {
                ReadTimberMemberShearPanel();
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

        private static void ReadTimberMemberShearPanel()
        {
            DataLogger.AddLogStart("Reading...");
            int count = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_MEMBER_SHEAR_PANEL, 0);
            int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_MEMBER_SHEAR_PANEL, 0);
            if (count != numbers.Length)
            {
                throw new Exception("Object count does not match object number count.");
            }

            DataLogger.AddText($"{numbers.Length} objects have been read.");
            DataLogger.InitializeProgressBar(0, numbers.Length, 0);
            for (int i = 0; i < count; i++)
            {
                member_shear_panel objectToRead = SoapModelClient.get_member_shear_panel(numbers[i]);
                LogTimberMemberShearPanel(objectToRead);
                DataLogger.SetProgressBarValue(i);
            }
        }

        private static void LogTimberMemberShearPanel(member_shear_panel panel)
        {
            DataLogger.IncrementOffset();
            DataLogger.AddText($"No.{panel.no}");
            DataLogger.AddText($"Name:{panel.name}");
            if (!string.IsNullOrEmpty(panel.comment))
            {
                DataLogger.AddText($"Restrain comment: {panel.comment}");
            }

            if (!string.IsNullOrEmpty(panel.generating_object_info))
            {
                DataLogger.AddText($"Generating object info: {panel.generating_object_info}");
            }

            DataLogger.AddText($"Stiffness:{panel.stiffness}");
            DataLogger.DecrementOffset();
        }

        public static bool Test_Types⁀for⁀Timber⁀Design_Member⁀Shear⁀Panel_Create()
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
                CreateTimberMemberShearPanel();
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

        private static void CreateTimberMemberShearPanel()
        {
            SoapModelClient.begin_modification(nameof(CreateTimberMemberShearPanel));
            DataLogger.SetProgressBarValue(20);
            DataLogger.AddText("Generating...");
            var panel = new member_shear_panel
            {
                no = 1,
                name = "Generated",
                stiffness = 1,
                stiffnessSpecified = true,
            };
            SoapModelClient.set_member_shear_panel(panel);
            DataLogger.AddText("Generated");
            SoapModelClient.finish_modification();
            DataLogger.SetProgressBarValue(50);
        }

        public static bool Test_Types⁀for⁀Timber⁀Design_Member⁀Shear⁀Panel_Delete()
        {
            DataLogger.AddLogStart("Deleting...");
            try
            {
                bool result = DeleteObjects(object_types.E_OBJECT_TYPE_MEMBER_SHEAR_PANEL, 0, "Member shear panel");
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
