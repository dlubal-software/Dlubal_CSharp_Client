#if !RSECTION

using Dlubal.WS.Clients.DotNetClientTest.Tools;
using Dlubal.WS.Common.Tools;
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
        public static bool Test_Types⁀for⁀Steel⁀Design_Member⁀Rotational⁀Restraints_Get()
        {
            DataLogger.AddLogStart("Reading member rotational restraints...");
            try
            {
                ReadSteelMemberRotationalRestraints();
            }
            catch(Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            DataLogger.ResetProgressBar();
            DataLogger.AddLogEnd(LogResultType.DONE);
            return true;
        }

        public static bool Test_Types⁀for⁀Steel⁀Design_Member⁀Rotational⁀Restraints_Create()
        {
            DataLogger.AddLogStart("Creating Member rotational constraints conditionals...");
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
                CreateSteelMemberRotationalRestraint();
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

        public static bool Test_Types⁀for⁀Steel⁀Design_Member⁀Rotational⁀Restraints_Delete()
        {
            DataLogger.AddLogStart("Deleting member rotational restraints...");
            try
            {
                bool result = DeleteObjects(object_types.E_OBJECT_TYPE_MEMBER_ROTATIONAL_RESTRAINT, 0, "Member rotational restraint");
                DataLogger.AddLogEnd(LogResultType.DONE);
                return result;
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
        }

        private static void ReadSteelMemberRotationalRestraints()
        {
            int count = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_MEMBER_ROTATIONAL_RESTRAINT, 0);
            int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_MEMBER_ROTATIONAL_RESTRAINT, 0);
            if (count != numbers.Length)
            {
                throw new Exception("Object count does not match object number count.");
            }
            DataLogger.AddText($"{numbers.Length} Member Rotational Restraints have been read.");
            DataLogger.InitializeProgressBar(0, numbers.Length, 0);
            for (int i = 0; i < count; i++)
            {
                member_rotational_restraint memberRotationRestraint = SoapModelClient.get_member_rotational_restraint(numbers[i]);
                DataLogger.IncrementOffset();
                LogSteelMemberRotationalRestraint(memberRotationRestraint);
                DataLogger.DecrementOffset();
                DataLogger.SetProgressBarValue(i);
            }
        }

        private static void LogSteelMemberRotationalRestraint(member_rotational_restraint restraint)
        {
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Member Rotational Restraints No.{restraint.no}");
            DataLogger.AddText($"Restraint type:{restraint.type}");
            if (!string.IsNullOrEmpty(restraint.comment))
            {
                DataLogger.AddText($"Restrain comment: {restraint.comment}");
            }
            if (!string.IsNullOrEmpty(restraint.generating_object_info))
            {
                DataLogger.AddText($"Generating object info: {restraint.generating_object_info}");
            }
            DataLogger.AddText($"Sheeting material name: {restraint.material_name}");
            DataLogger.AddText($"Sheeting name: {restraint.sheeting_name}");
            DataLogger.AddText($"Section name: {restraint.section_name}");
            DataLogger.AddText($"Sheeting position: {restraint.position_of_sheeting}");
            DataLogger.AddText($"Modulus of elasticity: {restraint.modulus_of_elasticity}");
            DataLogger.AddText($"Sheeting thickness: {restraint.sheeting_thickness}");
            DataLogger.AddText($"Sheeting moment of inertia: {restraint.sheeting_moment_of_inertia}");
            DataLogger.AddText($"Sheeting distance of ribs: {restraint.sheeting_distance_of_ribs}");
            DataLogger.AddText($"Width of section flange: {restraint.width_of_section_flange}");
            DataLogger.AddText($"Spring stiffness: {restraint.spring_stiffness}");
            DataLogger.AddText($"Beam spacing: {restraint.beam_spacing}");
            DataLogger.DecrementOffset();
        }

        private static void CreateSteelMemberRotationalRestraint()
        {
            SoapModelClient.begin_modification(nameof(CreateSteelMemberRotationalRestraint));
            coordinate_system coordinateSystem = GetCoordinateSystem();
            DataLogger.AddText($"Set coordinate system No.{coordinateSystem.no}");
            SoapModelClient.set_coordinate_system(coordinateSystem);
            DataLogger.SetProgressBarValue(20);
            DataLogger.AddText("Generating restraint...");
            material material = new material
            {
                no = 1,
                name = "Grade S275",
            };
            SoapModelClient.set_material(material);
            var restraint = new member_rotational_restraint
            {
                no = 1,
                name = "Generated restrain",
                type = member_rotational_restraint_type.TYPE_CONTINUOUS,
                total_rotational_spring_stiffness = 0.45,
                spring_stiffnessSpecified = true,
                spring_stiffness = 0.45,
                material_name = "Grade S275",
                sheeting_name = "GOST C (-) 10-899-0.6",
                beam_spacing = 1.0005,
                beam_spacingSpecified = true,
            };
            SoapModelClient.set_member_rotational_restraint(restraint);
            DataLogger.AddText("Generated restrain");
            SoapModelClient.finish_modification();
            DataLogger.SetProgressBarValue(50);
        }

        private static coordinate_system GetCoordinateSystem()
        {
            const int COORDINATE_SYSTEM_NO = 1;
            coordinate_system coordinateSystem = new coordinate_system
            {
                no = COORDINATE_SYSTEM_NO,
                type = coordinate_system_type.TYPE_3_POINTS,
                typeSpecified = true,
                u_axis_point_coordinates = new vector_3d().SetCoordinates(1.0, 0.0, 1.0),
                uw_plane_point_coordinates = new vector_3d().SetCoordinates(0.0, 0.0, 1.0)
            };
            return coordinateSystem;
        }
    }
}

#endif // !RSECTION
