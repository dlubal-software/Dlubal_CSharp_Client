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
        public static bool Test_Types⁀for⁀Aluminium⁀Design_Member⁀Transverse⁀Weld_Get()
        {
            try
            {
                ReadAluminiumMemberTransverseWeld();
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

        private static void ReadAluminiumMemberTransverseWeld()
        {
            DataLogger.AddLogStart("Reading...");
            int count = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_ALUMINUM_MEMBER_TRANSVERSE_WELD, 0);
            int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_ALUMINUM_MEMBER_TRANSVERSE_WELD, 0);
            if (count != numbers.Length)
            {
                throw new Exception("Object count does not match object number count.");
            }

            DataLogger.AddText($"{numbers.Length} objects have been read.");
            DataLogger.InitializeProgressBar(0, numbers.Length, 0);
            for (int i = 0; i < count; i++)
            {
                aluminum_member_transverse_weld objectToRead = SoapModelClient.get_aluminum_member_transverse_weld(numbers[i]);
                LogAluminiumMemberTransverseWeld(objectToRead);
                DataLogger.SetProgressBarValue(i);
            }
        }

        private static void LogAluminiumMemberTransverseWeld(aluminum_member_transverse_weld reduction)
        {
            DataLogger.IncrementOffset();
            DataLogger.AddText($"No.{reduction.no}");
            DataLogger.AddText($"Name: {reduction.name}");
            DataLogger.AddText($"Members string: {reduction.members_string}");
            if (!string.IsNullOrEmpty(reduction.comment))
            {
                DataLogger.AddText($"Comment: {reduction.comment}");
            }

            if (!string.IsNullOrEmpty(reduction.generating_object_info))
            {
                DataLogger.AddText($"Generating object info: {reduction.generating_object_info}");
            }

            foreach (var row in reduction.components)
            {
                LogAluminumMemberSectionReductionComponentsRow(row);
            }

            DataLogger.DecrementOffset();
        }

        private static void LogAluminumMemberSectionReductionComponentsRow(aluminum_member_transverse_weld_components_row row)
        {
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Row number: {row.no}");
            LogSteelMemberSectionReductionComponents(row.row);
            DataLogger.DecrementOffset();
        }

        private static void LogSteelMemberSectionReductionComponents(aluminum_member_transverse_weld_components components)
        {
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Position: {components.position}");
            DataLogger.AddText($"Weld type: {components.weld_type}");
            DataLogger.AddText($"Note: {components.note}");
            DataLogger.DecrementOffset();
        }

        public static bool Test_Types⁀for⁀Aluminium⁀Design_Member⁀Transverse⁀Weld_Create()
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
                CreateAluminiumMemberTransverseWeld();
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

        private static void CreateAluminiumMemberTransverseWeld()
        {
            SoapModelClient.begin_modification(nameof(CreateAluminiumMemberTransverseWeld));
            DataLogger.SetProgressBarValue(20);
            DataLogger.AddText("Generating...");
            aluminum_member_transverse_weld_components_row[] components =
            {
                new aluminum_member_transverse_weld_components_row
                {
                    no = 1,
                    noSpecified = true,
                    row = new aluminum_member_transverse_weld_components
                    {
                        weld_type = weld_type.WELD_COMPONENT_TYPE_BUTT,
                        weld_typeSpecified  = true,
                        position = 5,
                        positionSpecified = true,
                    }
                },
            };
            var weld = new aluminum_member_transverse_weld
            {
                no = 1,
                name = "Generated",
                components = components,
            };
            SoapModelClient.set_aluminum_member_transverse_weld(weld);
            DataLogger.AddText("Generated");
            SoapModelClient.finish_modification();
            DataLogger.SetProgressBarValue(50);
        }

        public static bool Test_Types⁀for⁀Aluminium⁀Design_Member⁀Transverse⁀Weld_Delete()
        {
            DataLogger.AddLogStart("Deleting...");
            try
            {
                bool result = DeleteObjects(object_types.E_OBJECT_TYPE_ALUMINUM_MEMBER_TRANSVERSE_WELD, 0, "Transverse Weld");
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
