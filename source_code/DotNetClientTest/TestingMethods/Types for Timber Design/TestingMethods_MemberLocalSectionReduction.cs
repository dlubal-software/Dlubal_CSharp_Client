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
        public static bool Test_Types⁀for⁀Timber⁀Design_Member⁀Local⁀Section⁀Reduction_Get()
        {
            try
            {
                ReadTimberMemberLocalSectionReduction();
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

        private static void ReadTimberMemberLocalSectionReduction()
        {
            DataLogger.AddLogStart("Reading...");
            int count = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_TIMBER_MEMBER_LOCAL_SECTION_REDUCTION, 0);
            int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_TIMBER_MEMBER_LOCAL_SECTION_REDUCTION, 0);
            if (count != numbers.Length)
            {
                throw new Exception("Object count does not match object number count.");
            }

            DataLogger.AddText($"{numbers.Length} objects have been read.");
            DataLogger.InitializeProgressBar(0, numbers.Length, 0);
            for (int i = 0; i < count; i++)
            {
                timber_member_local_section_reduction objectToRead = SoapModelClient.get_timber_member_local_section_reduction(numbers[i]);
                LogTimberMemberLocalSectionReduction(objectToRead);
                DataLogger.SetProgressBarValue(i);
            }
        }

        private static void LogTimberMemberLocalSectionReduction(timber_member_local_section_reduction reduction)
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
                LogTimberMemberLocalSectionReductionComponentsRow(row);
            }

            DataLogger.DecrementOffset();
        }

        private static void LogTimberMemberLocalSectionReductionComponentsRow(timber_member_local_section_reduction_components_row row)
        {
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Row number: {row.no}");
            LogTimberMemberLocalSectionReductionComponents(row.row);
            DataLogger.DecrementOffset();
        }

        private static void LogTimberMemberLocalSectionReductionComponents(timber_member_local_section_reduction_components components)
        {
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Position: {components.position}");
            DataLogger.AddText($"Reduction type: {components.reduction_type}");
            DataLogger.AddText($"Note: {components.note}");
            DataLogger.DecrementOffset();
        }

        public static bool Test_Types⁀for⁀Timber⁀Design_Member⁀Local⁀Section⁀Reduction_Create()
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
                CreateTimberMemberLocalSectionReduction();
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

        private static void CreateTimberMemberLocalSectionReduction()
        {
            SoapModelClient.begin_modification(nameof(CreateTimberMemberLocalSectionReduction));
            DataLogger.SetProgressBarValue(20);
            DataLogger.AddText("Generating...");
            var components = GetTimberMemberLocalSectionReductionComponentsRows();
            var reduction = new timber_member_local_section_reduction
            {
                no = 1,
                name = "Generated",
                components = components,
            };
            SoapModelClient.set_timber_member_local_section_reduction(reduction);
            SoapModelClient.finish_modification();
            DataLogger.AddText("Generated");
            DataLogger.SetProgressBarValue(50);
        }

        private static timber_member_local_section_reduction_components_row[] GetTimberMemberLocalSectionReductionComponentsRows()
        {
            var rows = new timber_member_local_section_reduction_components_row[]
            {
                new timber_member_local_section_reduction_components_row
                {
                    no = 1,
                    noSpecified = true,
                    row = new timber_member_local_section_reduction_components
                    {
                        // ToDo: Timber Design: Enable REDUCTION_COMPONENT_TYPE_RECTANGLE_OPENING after publish from pre-release mode.
                        //reduction_type = timber_member_local_section_reduction_components_reduction_type.REDUCTION_COMPONENT_TYPE_RECTANGLE_OPENING, //< pre-release
                        reduction_type = timber_member_local_section_reduction_components_reduction_type.REDUCTION_COMPONENT_TYPE_START_NOTCH,
                        reduction_typeSpecified = true,
                        position = 5,
                        positionSpecified = true,
                        length = 5,
                        lengthSpecified = true,
                        depth = 5,
                        depthSpecified = true,
                    }
                },
                new timber_member_local_section_reduction_components_row
                {
                    no = 2,
                    noSpecified = true,
                    row = new timber_member_local_section_reduction_components
                    {
                        // ToDo: Timber Design: Enable REDUCTION_COMPONENT_TYPE_CIRCLE_OPENING after publish from pre-release mode.
                        //reduction_type = timber_member_local_section_reduction_components_reduction_type.REDUCTION_COMPONENT_TYPE_CIRCLE_OPENING, //< pre-release
                        reduction_type = timber_member_local_section_reduction_components_reduction_type.REDUCTION_COMPONENT_TYPE_END_NOTCH,
                        reduction_typeSpecified = true,
                        position = 10,
                        positionSpecified = true,
                        length = 5,
                        lengthSpecified = true,
                        depth = 5,
                        depthSpecified = true,
                    }
                }
            };
            return rows;
        }

        public static bool Test_Types⁀for⁀Timber⁀Design_Member⁀Local⁀Section⁀Reduction_Delete()
        {
            DataLogger.AddLogStart("Deleting...");
            try
            {
                bool result = DeleteObjects(object_types.E_OBJECT_TYPE_TIMBER_MEMBER_LOCAL_SECTION_REDUCTION, 0, "Local section reduction");
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
