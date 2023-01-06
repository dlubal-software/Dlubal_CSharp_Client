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
        public static bool Test_Types⁀for⁀Steel⁀Design_Member⁀Shear⁀Panel_Get()
        {
            DataLogger.AddLogStart("Reading member shear panels...");
            try
            {
                ReadMemberShearPanels();
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

        public static bool Test_Types⁀for⁀Steel⁀Design_Member⁀Shear⁀Panel_Create()
        {
            DataLogger.AddLogStart("Creating Member shear panel...");
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
                CreateSteelMemberShearPanel();
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

        public static bool Test_Types⁀for⁀Steel⁀Design_Member⁀Shear⁀Panel_Delete()
        {
            DataLogger.AddLogStart("Deleting steel member shear panel...");
            try
            {
                bool result = DeleteObjects(object_types.E_OBJECT_TYPE_MEMBER_SHEAR_PANEL, 0, "Steel member shear panel");
                DataLogger.AddLogEnd(LogResultType.DONE);
                return result;
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
        }

        private static void ReadMemberShearPanels()
        {
            int count = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_MEMBER_SHEAR_PANEL, 0);
            int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_MEMBER_SHEAR_PANEL, 0);

            if (count != numbers.Length)
            {
                throw new Exception("Object count does not match object number count.");
            }

            DataLogger.AddText($"{numbers.Length} Member shear panel have been read.");
            DataLogger.InitializeProgressBar(0, numbers.Length, 0);

            for (int i = 0; i < count; i++)
            {
                member_shear_panel steel_member_shear_panel = SoapModelClient.get_member_shear_panel(numbers[i]);
                DataLogger.IncrementOffset();
                DataLogger.AddText($"Shear panel No.{numbers[i]}");
                LogSteelShearPanelInfo(steel_member_shear_panel);
                DataLogger.DecrementOffset();
                DataLogger.SetProgressBarValue(i);
            }
        }

        private static void LogSteelShearPanelInfo(member_shear_panel panel)
        {
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Name: {panel.name}");
            DataLogger.AddText($"Type: {panel.definition_type}");
            DataLogger.AddText($"Sheeting name: {panel.sheeting_name}");
            DataLogger.AddText($"Coefficent k1: {panel.coefficient_k1}");
            DataLogger.AddText($"Coefficent k2: {panel.coefficient_k2}");
            DataLogger.AddText($"Beam spacing: {panel.beam_spacing}");
            DataLogger.DecrementOffset();
        }

        private static void CreateSteelMemberShearPanel()
        {
            SoapModelClient.begin_modification(nameof(CreateSteelMemberShearPanel));
            coordinate_system coordinateSystem = GetCoordinateSystem();
            DataLogger.AddText($"Set coordinate system No.{coordinateSystem.no}");
            SoapModelClient.set_coordinate_system(coordinateSystem);
            DataLogger.SetProgressBarValue(20);
            DataLogger.AddText("Generating steel member shear panel...");
            var panel = new member_shear_panel
            {
                no = 1,
                name = "Generated shear panel",
                sheeting_name = "GOST C (-) 10-899-0.6",
                panel_length = 1,
                panel_lengthSpecified = true,
                beam_spacing = 1.0005,
                beam_spacingSpecified = true,
                coefficient_k1 = 1,
                coefficient_k2 = 1,
                coefficient_k1Specified = true,
                coefficient_k2Specified = true,
            };
            SoapModelClient.set_member_shear_panel(panel);
            DataLogger.AddText("Generated steel member shear panel");
            SoapModelClient.finish_modification();
            DataLogger.SetProgressBarValue(50);
        }
    }
}

#endif // !RSECTION
