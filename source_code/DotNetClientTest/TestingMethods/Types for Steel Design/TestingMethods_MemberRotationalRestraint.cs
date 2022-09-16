#if !RSECTION

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
        public static bool Test_Types\u2040for\u2040Steel\u2040Design_Member\u2040Rotational\u2040Restraint_Create()
        {
            const int MEMBER_ROTATIONAL_RESTRAINT_1_NO = 1;
            const int MEMBER_ROTATIONAL_RESTRAINT_2_NO = 2;
            const int MEMBER_ROTATIONAL_RESTRAINT_3_NO = 3;

            DataLogger.AddLogStart("Creating member rotational restraints...");
            DataLogger.InitializeProgressBar(0, 40, 0);

            bool result = Test_General_Delete⁀All();

            if (!result || (SoapModelClient == null))
            {
                DataLogger.AddLogEnd(DataLogger.LogResultType.FAILED);
                return false;
            }

            DataLogger.SetProgressBarValue(10);

            try
            {
                SoapModelClient.begin_modification("Create member rotational restraint");

                // continuous member rotational restraint
                DataLogger.AddText($"Set member rotational restraint No.{MEMBER_ROTATIONAL_RESTRAINT_1_NO}, type continuous");
                steel_member_rotational_restraint memberRotationalRestraint1 = new steel_member_rotational_restraint
                {
                    no = MEMBER_ROTATIONAL_RESTRAINT_1_NO,
                    type = steel_member_rotational_restraint_type.TYPE_CONTINUOUS,
                    typeSpecified = true,
                    material_name = "S355",
                    modulus_of_elasticity = 2.10005e+11,
                    modulus_of_elasticitySpecified = true,
                    continuous_beam_effect = steel_member_rotational_restraint_continuous_beam_effect.CONTINUOUS_BEAM_EFFECT_END_PANEL,
                    continuous_beam_effectSpecified = true,
                    section_deformation_cdb = true,
                    section_deformation_cdbSpecified = true,
                    beam_spacing = 8,
                    beam_spacingSpecified = true,
                    sheeting_name = "Arval (-) 35/207 - 0.63 (b: 1) | DIN 18807 | Arval",
                    position_of_sheeting = steel_member_rotational_restraint_position_of_sheeting.SHEETING_POSITION_POSITIVE,
                    position_of_sheetingSpecified = true,
                    sheeting_thickness = 1.5,
                    sheeting_thicknessSpecified = true,
                    sheeting_moment_of_inertia = 1960000,
                    sheeting_moment_of_inertiaSpecified = true,
                    sheeting_distance_of_ribs = 0.28,
                    sheeting_distance_of_ribsSpecified = true,
                    width_of_section_flange = 40,
                    width_of_section_flangeSpecified = true,
                    spring_stiffness = 5.2,
                    spring_stiffnessSpecified = true,
                    different_spring_stiffness = false,
                    different_spring_stiffnessSpecified = true,
                    method_of_determining_cda = steel_member_rotational_restraint_method_of_determining_cda.EN_1993_1_3_TABLE_10_3,
                    method_of_determining_cdaSpecified = true
                };
                SoapModelClient.set_steel_member_rotational_restraint(memberRotationalRestraint1);

                DataLogger.SetProgressBarValue(20);

                // discrete member rotational restraint
                DataLogger.AddText($"Set member rotational restraint No.{MEMBER_ROTATIONAL_RESTRAINT_2_NO}, type discrete");
                steel_member_rotational_restraint memberRotationalRestraint2 = new steel_member_rotational_restraint
                {
                    no = MEMBER_ROTATIONAL_RESTRAINT_2_NO,
                    type = steel_member_rotational_restraint_type.TYPE_DISCRETE,
                    typeSpecified = true,
                    material_name = "S355",
                    modulus_of_elasticity = 2.1e+11,
                    modulus_of_elasticitySpecified = true,
                    continuous_beam_effect = steel_member_rotational_restraint_continuous_beam_effect.CONTINUOUS_BEAM_EFFECT_END_PANEL,
                    continuous_beam_effectSpecified = true,
                    section_deformation_cdb = true,
                    section_deformation_cdbSpecified = true,
                    beam_spacing = 8,
                    beam_spacingSpecified = true,
                    section_name = "IPE 80 | Euronorm 19-57 | ArcelorMittal (2011)",
                    rotational_stiffness = steel_member_rotational_restraint_rotational_stiffness.ROTATIONAL_STIFFNESS_INFINITELY,
                    section_moment_of_inertia = 193000,
                    section_moment_of_inertiaSpecified = true,
                    purlin_spacing = 1,
                    purlin_spacingSpecified = true
                };
                SoapModelClient.set_steel_member_rotational_restraint(memberRotationalRestraint2);

                DataLogger.SetProgressBarValue(30);

                // manually member rotational restraint
                DataLogger.AddText($"Set member rotational restraint No.{MEMBER_ROTATIONAL_RESTRAINT_3_NO}, type manually");
                steel_member_rotational_restraint memberRotationalRestraint3 = new steel_member_rotational_restraint
                {
                    no = MEMBER_ROTATIONAL_RESTRAINT_3_NO,
                    type = steel_member_rotational_restraint_type.TYPE_MANUALLY,
                    typeSpecified = true,
                    total_rotational_spring_stiffness = 56,
                    total_rotational_spring_stiffnessSpecified = true
                };
                SoapModelClient.set_steel_member_rotational_restraint(memberRotationalRestraint3);

                DataLogger.SetProgressBarValue(40);
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

        public static bool Test_Types\u2040for\u2040Steel\u2040Design_Member\u2040Rotational\u2040Restraint_Get()
        {
            DataLogger.AddLogStart("Reading member rotational restraints...");

            try
            {
                int count = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_STEEL_MEMBER_ROTATIONAL_RESTRAINT, 0);
                int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_STEEL_MEMBER_ROTATIONAL_RESTRAINT, 0);

                if (count != numbers.Length)
                {
                    throw new Exception("Object count does not match object number count.");
                }

                DataLogger.AddText($"{numbers.Length} member rotational restraint numbers have been read.");
                DataLogger.InitializeProgressBar(0, numbers.Length, 0);

                for (int i = 0; i < count; i++)
                {
                    steel_member_rotational_restraint memberRotationalRestraints = SoapModelClient.get_steel_member_rotational_restraint(numbers[i]);

                    DataLogger.AddText($"Member rotational restraint No.{numbers[i]}");
                    DataLogger.AddText($"Type = {memberRotationalRestraints.type.ToString()}");
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

        public static bool Test_Types\u2040for\u2040Steel\u2040Design_Member\u2040Rotational\u2040Restraint_Delete()
        {
            return DeleteObjects(object_types.E_OBJECT_TYPE_STEEL_MEMBER_ROTATIONAL_RESTRAINT, 0, "member rotational restraints");
        }
    }
}

#endif // !RSECTION
