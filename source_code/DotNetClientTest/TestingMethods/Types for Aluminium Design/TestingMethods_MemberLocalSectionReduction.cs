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
        public static bool Test_Types⁀for⁀Aluminium⁀Design_Member⁀Local⁀Section⁀Reduction_Get()
        {
            try
            {
                ReadAluminiumMemberLocalSectionReduction();
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

        private static void ReadAluminiumMemberLocalSectionReduction()
        {
            DataLogger.AddLogStart("Reading...");
            int count = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_ALUMINUM_MEMBER_LOCAL_SECTION_REDUCTION, 0);
            int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_ALUMINUM_MEMBER_LOCAL_SECTION_REDUCTION, 0);
            if (count != numbers.Length)
            {
                throw new Exception("Object count does not match object number count.");
            }

            DataLogger.AddText($"{numbers.Length} objects have been read.");
            DataLogger.InitializeProgressBar(0, numbers.Length, 0);
            for (int i = 0; i < count; i++)
            {
                aluminum_member_local_section_reduction reduction = SoapModelClient.get_aluminum_member_local_section_reduction(numbers[i]);
                DataLogger.IncrementOffset();
                LogAluminiumMemberLocalSectionReduction(reduction);
                DataLogger.DecrementOffset();
                DataLogger.SetProgressBarValue(i);
            }
        }

        private static void LogAluminiumMemberLocalSectionReduction(aluminum_member_local_section_reduction reduction)
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

        private static void LogAluminumMemberSectionReductionComponentsRow(aluminum_member_local_section_reduction_components_row row)
        {
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Row number: {row.no}");
            LogAluminumMemberSectionReductionComponents(row.row);
            DataLogger.DecrementOffset();
        }

        private static void LogAluminumMemberSectionReductionComponents(aluminum_member_local_section_reduction_components components)
        {
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Position: {components.position}");
            DataLogger.AddText($"Reduction area: {components.reduction_area}");
            DataLogger.AddText($"Reduction type: {components.reduction_type}");
            DataLogger.AddText($"Note: {components.note}");
            DataLogger.DecrementOffset();
        }

        public static bool Test_Types⁀for⁀Aluminium⁀Design_Member⁀Local⁀Section⁀Reduction_Create()
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
                CreateAluminiumMemberLocalSectionReduction();
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

        private static void CreateAluminiumMemberLocalSectionReduction()
        {
            SoapModelClient.begin_modification(nameof(CreateAluminiumMemberLocalSectionReduction));
            DataLogger.SetProgressBarValue(20);
            DataLogger.AddText("Generating...");
            aluminum_member_local_section_reduction_components_row[] components = new aluminum_member_local_section_reduction_components_row[]
            {
                new aluminum_member_local_section_reduction_components_row
                {
                    no = 1,
                    noSpecified = true,
                    row = new aluminum_member_local_section_reduction_components
                    {
                        reduction_area = 54,
                        reduction_areaSpecified = true,
                        position = 1,
                        positionSpecified = true,
                        note = "Position should be 1",
                    }
                },
                new aluminum_member_local_section_reduction_components_row
                {
                    no = 2,
                    noSpecified = true,
                    row = new aluminum_member_local_section_reduction_components
                    {
                        reduction_area = 1.5,
                        reduction_areaSpecified = true,
                        position = 2,
                        positionSpecified = true,
                        note = "Position should be 2"
                    }
                },
                new aluminum_member_local_section_reduction_components_row
                {
                    no = 3,
                    noSpecified = true,
                    row = new aluminum_member_local_section_reduction_components
                    {
                        reduction_area = 1.5,
                        reduction_areaSpecified = true,
                        position = 3,
                        positionSpecified = true,
                        // ToDo: Aluminium Design: Enable REDUCTION_COMPONENT_TYPE_SECTION_VALUES after publish from pre-release mode.
                        //reduction_type = reduction_type.REDUCTION_COMPONENT_TYPE_SECTION_VALUES, //< pre-release
                        reduction_type = reduction_type.REDUCTION_COMPONENT_TYPE_DESIGN_PARAMETERS,
                        reduction_typeSpecified = true,
                        note = "Position should be 3",
                    }
                }
            };
            var reduction = new aluminum_member_local_section_reduction
            {
                no = 1,
                name = "Generated",
                components = components
            };
            SoapModelClient.set_aluminum_member_local_section_reduction(reduction);
            DataLogger.AddText("Generated");
            SoapModelClient.finish_modification();
            DataLogger.SetProgressBarValue(50);
        }

        public static bool Test_Types⁀for⁀Aluminium⁀Design_Member⁀Local⁀Section⁀Reduction_Delete()
        {
            DataLogger.AddLogStart("Deleting...");
            try
            {
                bool result = DeleteObjects(object_types.E_OBJECT_TYPE_ALUMINUM_MEMBER_LOCAL_SECTION_REDUCTION, 0, "Local section reduction");
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
