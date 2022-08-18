using Dlubal.WS.Clients.DotNetClientTest.Tools;
using Dlubal.WS.Common.Tools;
using System;

#if RFEM
using Dlubal.WS.Rfem6.Model;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
        public static bool Test_Types\u2040for\u2040Steel\u2040Design_Member\u2040Shear\u2040Panels_Create()
        {
            const int MEMBER_SHEAR_PANEL_1_NO = 1;
            const int MEMBER_SHEAR_PANEL_2_NO = 2;
            const int MEMBER_SHEAR_PANEL_3_NO = 3;
            const int MEMBER_SHEAR_PANEL_4_NO = 4;

            DataLogger.AddLogStart("Creating member shear panels...");
            DataLogger.InitializeProgressBar(0, 50, 0);

            bool result = Test_General_Delete⁀All();

            if (!result || (SoapModelClient == null))
            {
                DataLogger.AddLogEnd(DataLogger.LogResultType.FAILED);
                return false;
            }

            DataLogger.SetProgressBarValue(10);

            try
            {
                SoapModelClient.begin_modification("Create member shear panel");

                // member shear panel trapezoidal sheeting
                DataLogger.AddText($"Set member shear panel No.{MEMBER_SHEAR_PANEL_1_NO} - trapezoidal sheeting");
                steel_member_shear_panel memberShearPanel1 = new steel_member_shear_panel
                {
                    no = MEMBER_SHEAR_PANEL_1_NO,
                    definition_type = steel_member_shear_panel_definition_type.DEFINITION_TYPE_TRAPEZOIDAL_SHEETING,
                    definition_typeSpecified = true,
                    position_on_section = steel_member_shear_panel_position_on_section.POSITION_ON_UPPER_FLANGE,
                    position_on_sectionSpecified = true,
                    panel_length = 4,
                    panel_lengthSpecified = true,
                    beam_spacing = 1.5,
                    beam_spacingSpecified = true,
                    fastening_arrangement = steel_member_shear_panel_fastening_arrangement.FASTENING_ARRANGEMENT_EVERY_RIB,
                    fastening_arrangementSpecified = true,
                    coefficient_k1 = 0.000247,
                    coefficient_k1Specified = true,
                    coefficient_k2 = 0.01043,
                    coefficient_k2Specified = true,
                    sheeting_name = "FI (+) 35/207 - 0.63 (b: 1) | DIN 18807 | Fischer Profil"
                };
                SoapModelClient.set_steel_member_shear_panel(memberShearPanel1);

                DataLogger.SetProgressBarValue(30);

                // member shear panel branching
                DataLogger.AddText($"Set member shear panel No.{MEMBER_SHEAR_PANEL_2_NO} - bracing");
                steel_member_shear_panel memberShearPanel2 = new steel_member_shear_panel
                {
                    no = MEMBER_SHEAR_PANEL_2_NO,
                    definition_type = steel_member_shear_panel_definition_type.DEFINITION_TYPE_BRACING,
                    definition_typeSpecified = true,
                    position_on_section = steel_member_shear_panel_position_on_section.POSITION_ON_UPPER_FLANGE,
                    position_on_sectionSpecified = true,
                    panel_length = 3,
                    panel_lengthSpecified = true,
                    beam_spacing = 1.4,
                    beam_spacingSpecified = true,
                    post_spacing = 1,
                    post_spacingSpecified = true,
                    number_of_bracings = 2,
                    number_of_bracingsSpecified = true,
                    diagonals_section_area = 0.00951,
                    diagonals_section_areaSpecified = true,
                    posts_section_area = 0.062,
                    posts_section_areaSpecified = true,
                    diagonals_section_name = "ASB 280 - 74 | BS EN 10365:2017 | British Steel",
                    posts_section_name = "UKB 1016x305x487 | BS 4-1:2005 | Tata Steel"
                };
                SoapModelClient.set_steel_member_shear_panel(memberShearPanel2);
                DataLogger.SetProgressBarValue(40);

                // member shear panel trapezoidal sheeting and bracing
                DataLogger.AddText($"Set member shear panel No.{MEMBER_SHEAR_PANEL_3_NO} - trapezoidal sheeting and bracing");
                steel_member_shear_panel memberShearPanel3 = new steel_member_shear_panel
                {
                    no = MEMBER_SHEAR_PANEL_3_NO,
                    definition_type = steel_member_shear_panel_definition_type.DEFINITION_TYPE_TRAPEZOIDAL_SHEETING_AND_BRACING,
                    definition_typeSpecified = true,
                    position_on_section = steel_member_shear_panel_position_on_section.POSITION_IN_CENTROID,
                    position_on_sectionSpecified = true,
                    panel_length = 4,
                    panel_lengthSpecified = true,
                    beam_spacing = 1.5,
                    beam_spacingSpecified = true,
                    fastening_arrangement = steel_member_shear_panel_fastening_arrangement.FASTENING_ARRANGEMENT_EVERY_SECOND_RIB,
                    fastening_arrangementSpecified = true,
                    coefficient_k1 = 0.5,
                    coefficient_k1Specified = true,
                    coefficient_k2 = 6.5,
                    coefficient_k2Specified = true,
                    sheeting_name = "FI (+) 35/207 - 0.63 (b: 1) | DIN 18807 | Fischer Profil",
                    post_spacing = 1,
                    post_spacingSpecified = true,
                    number_of_bracings = 2,
                    number_of_bracingsSpecified = true,
                    diagonals_section_area = 3.5,
                    diagonals_section_areaSpecified = true,
                    posts_section_area = 3,
                    posts_section_areaSpecified = true,
                    diagonals_section_name = "L 100x65x7 | EN 10056-1:1998; ... | ArcelorMittal",
                    posts_section_name = "IPE 100 | Euronorm 19-57 | ArcelorMittal (2011)"
                };
                SoapModelClient.set_steel_member_shear_panel(memberShearPanel3);
                DataLogger.SetProgressBarValue(50);

                // member shear panel define s prov
                DataLogger.AddText($"Set member shear panel No.{MEMBER_SHEAR_PANEL_4_NO} - define s prov");
                steel_member_shear_panel memberShearPanel4 = new steel_member_shear_panel
                {
                    no = MEMBER_SHEAR_PANEL_4_NO,
                    definition_type = steel_member_shear_panel_definition_type.DEFINITION_TYPE_DEFINE_S_PROV,
                    definition_typeSpecified = true,
                    position_on_section = steel_member_shear_panel_position_on_section.POSITION_DEFINE,
                    position_on_sectionSpecified = true,
                    stiffness = 2.5,
                    stiffnessSpecified = true
                };
                SoapModelClient.set_steel_member_shear_panel(memberShearPanel4);
                DataLogger.SetProgressBarValue(50);
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            finally
            {
                try
                {
                    SoapModelClient.finish_modification();
                }
                catch (Exception exception)
                {
                    ModelWsExceptionHandler(exception);
                    SoapModelClient.reset();
                }

                DataLogger.ResetProgressBar();
            }

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }

        public static bool Test_Types\u2040for\u2040Steel\u2040Design_Member\u2040Shear\u2040Panels_Get()
        {
            DataLogger.AddLogStart("Reading member shear ...");

            try
            {
                int count = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_STEEL_MEMBER_SHEAR_PANEL, 0);
                int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_STEEL_MEMBER_SHEAR_PANEL, 0);

                if (count != numbers.Length)
                {
                    throw new Exception("Object count does not match object number count.");
                }

                DataLogger.AddText($"{numbers.Length} member shear panels numbers have been read.");
                DataLogger.InitializeProgressBar(0, numbers.Length, 0);

                for (int i = 0; i < count; i++)
                {
                    steel_member_shear_panel shearPanel = SoapModelClient.get_steel_member_shear_panel(numbers[i]);

                    DataLogger.AddText($"Member shear panel No.{numbers[i]}");
                    DataLogger.AddText($"Type = {shearPanel.definition_type.ToString()}");
                    DataLogger.SetProgressBarValue(i);
                }
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            finally
            {
                DataLogger.ResetProgressBar();
            }

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }

        public static bool Test_Types\u2040for\u2040Steel\u2040Design_Member\u2040Shear\u2040Panels_Delete()
        {
            return DeleteObjects(object_types.E_OBJECT_TYPE_STEEL_MEMBER_SHEAR_PANEL, 0, "member shear panel");
        }
    }
}
