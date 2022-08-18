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
        public static bool Test_Types⁀for⁀Aluminium⁀Design_Effective⁀Lengths_Get()
        {
            try
            {
                ReadAluminiumEffectiveLength();
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

        private static void ReadAluminiumEffectiveLength()
        {
            DataLogger.AddLogStart("Reading...");
            int count = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_ALUMINUM_EFFECTIVE_LENGTHS, 0);
            int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_ALUMINUM_EFFECTIVE_LENGTHS, 0);
            if (count != numbers.Length)
            {
                throw new Exception("Object count does not match object number count.");
            }

            DataLogger.AddText($"{numbers.Length} objects have been read.");
            DataLogger.InitializeProgressBar(0, numbers.Length, 0);
            for (int i = 0; i < count; i++)
            {
                aluminum_effective_lengths objectToRead = SoapModelClient.get_aluminum_effective_lengths(numbers[i]);
                LogAluminiumEffectiveLength(objectToRead);
                DataLogger.SetProgressBarValue(i);
            }
        }

        private static void LogAluminiumEffectiveLength(aluminum_effective_lengths length)
        {
            DataLogger.IncrementOffset();
            DataLogger.AddText($"No.{length.no}");
            DataLogger.AddText($"Name: {length.name}");
            DataLogger.AddText($"Members string: {length.members_string}");
            DataLogger.AddText($"Determination: {length.determination_mcr_europe}");
            DataLogger.AddText($"Considering Flexural buckling about y: {length.flexural_buckling_about_y}");
            DataLogger.AddText($"Considering Flexural buckling about z: {length.flexural_buckling_about_z}");
            DataLogger.AddText($"Considering torsional buckling: {length.torsional_buckling}");
            DataLogger.AddText($"Considering lateral-torsional buckling: {length.lateral_torsional_buckling}");
            if (!string.IsNullOrEmpty(length.comment))
            {
                DataLogger.AddText($"Comment: {length.comment}");
            }

            if (!string.IsNullOrEmpty(length.generating_object_info))
            {
                DataLogger.AddText($"Generating object info: {length.generating_object_info}");
            }

            foreach (var row in length.nodal_supports)
            {
                LogAluminiumEffectiveLengthNodalSupportRow(row);
            }

            foreach(var factor in length.factors)
            {
                LogAluminiumEffectiveLengthFactorRow(factor);
            }

            DataLogger.DecrementOffset();
        }

        private static void LogAluminiumEffectiveLengthFactorRow(aluminum_effective_lengths_factors_row row)
        {
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Factor Row No {row.no}");
            LogAluminiumEffectiveLengthFactor(row.row);
            DataLogger.DecrementOffset();
        }

        private static void LogAluminiumEffectiveLengthFactor(aluminum_effective_lengths_factors factor)
        {
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Torsional buckling {factor.torsional_buckling}");
            DataLogger.AddText($"Flexural buckling z {factor.flexural_buckling_z}");
            DataLogger.AddText($"Flexural buckling y {factor.flexural_buckling_y}");
            DataLogger.AddText($"Flexural buckling v {factor.flexural_buckling_v}");
            DataLogger.AddText($"Ciritical moment {factor.critical_moment}");
            DataLogger.DecrementOffset();
        }

        private static void LogAluminiumEffectiveLengthNodalSupportRow(aluminum_effective_lengths_nodal_supports_row row)
        {
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Support Row No. {row.no}");
            LogAluminiumEffectiveLengthNodalSupport(row.row);
            DataLogger.DecrementOffset();
        }

        private static void LogAluminiumEffectiveLengthNodalSupport(aluminum_effective_lengths_nodal_supports supports)
        {
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Support type: {supports.support_type}");
            DataLogger.AddText($"in z: {supports.support_in_z}");
            DataLogger.AddText($"in y: {supports.support_in_y_type}");
            DataLogger.AddText($"about x: {supports.restraint_about_x_type}");
            DataLogger.AddText($"about z: {supports.restraint_about_z_type}");
            DataLogger.AddText($"Nodes: {supports.restraint_warping_type}");
            DataLogger.DecrementOffset();
        }

        public static bool Test_Types⁀for⁀Aluminium⁀Design_Effective⁀Lengths_Create()
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
                CreateAluminiumEffectiveLengths();
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

        private static void CreateAluminiumEffectiveLengths()
        {
            SoapModelClient.begin_modification(nameof(CreateAluminiumEffectiveLengths));
            DataLogger.SetProgressBarValue(20);
            DataLogger.AddText("Generating...");
            aluminum_effective_lengths_nodal_supports_row[] supportsRows = GetAluminumEffectiveLengthsNodalSupportsRow();
            aluminum_effective_lengths_factors_row[] factorsRows = GetAluminumEffectiveLengthsFactorsRows();
            aluminum_effective_lengths lengths = new aluminum_effective_lengths
            {
                no = 1,
                name = "Generated",
                nodal_supports = supportsRows,
                factors = factorsRows,
                determination_mcr_europe = aluminum_effective_lengths_determination_mcr_europe.DETERMINATION_EUROPE_USER_DEFINED,
                determination_mcr_europeSpecified = true,
            };
            SoapModelClient.set_aluminum_effective_lengths(lengths);
            DataLogger.AddText("Generated");
            SoapModelClient.finish_modification();
            DataLogger.SetProgressBarValue(50);
        }

        private static aluminum_effective_lengths_factors_row[] GetAluminumEffectiveLengthsFactorsRows()
        {
            var factorsRows = new aluminum_effective_lengths_factors_row[]
            {
                new aluminum_effective_lengths_factors_row
                {
                    no = 1,
                    noSpecified = true,
                    row = new aluminum_effective_lengths_factors
                    {
                        torsional_buckling = 10,
                        torsional_bucklingSpecified = true,
                        flexural_buckling_u = 15,
                        flexural_buckling_uSpecified = true,
                        flexural_buckling_v = 42,
                        flexural_buckling_vSpecified = true,
                        critical_moment = 52,
                        critical_momentSpecified=true,
                    }
                },
            };
            return factorsRows;
        }

        private static aluminum_effective_lengths_nodal_supports_row[] GetAluminumEffectiveLengthsNodalSupportsRow()
        {
            aluminum_effective_lengths_nodal_supports_row[] supportsRows = new aluminum_effective_lengths_nodal_supports_row[]
            {
                new aluminum_effective_lengths_nodal_supports_row()
                {
                    no = 1,
                    noSpecified = true,
                    row = new aluminum_effective_lengths_nodal_supports()
                    {
                        support_type = support_type.SUPPORT_TYPE_FIXED_ALL,
                        support_typeSpecified = true,
                    }
                },
                new aluminum_effective_lengths_nodal_supports_row()
                {
                    no = 2,
                    noSpecified = true,
                    row = new aluminum_effective_lengths_nodal_supports()
                    {
                        support_type = support_type.SUPPORT_TYPE_NONE,
                        support_typeSpecified = true,
                    }
                }
            };
            return supportsRows;
        }

        public static bool Test_Types⁀for⁀Aluminium⁀Design_Effective⁀Lengths_Delete()
        {
            DataLogger.AddLogStart("Deleting...");
            try
            {
                bool result = DeleteObjects(object_types.E_OBJECT_TYPE_ALUMINUM_EFFECTIVE_LENGTHS, 0, "Effective Lengths");
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
