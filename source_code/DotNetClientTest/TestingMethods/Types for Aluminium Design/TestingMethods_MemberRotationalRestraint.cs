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
        public static bool Test_Types⁀for⁀Aluminium⁀Design_Member⁀Rotational⁀Restraint_Get()
        {
            try
            {
                ReadAluminiumMemberRotationalRestraint();
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

        private static void ReadAluminiumMemberRotationalRestraint()
        {
            DataLogger.AddLogStart("Reading...");
            int count = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_ALUMINUM_MEMBER_ROTATIONAL_RESTRAINT, 0);
            int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_ALUMINUM_MEMBER_ROTATIONAL_RESTRAINT, 0);
            if (count != numbers.Length)
            {
                throw new Exception("Object count does not match object number count.");
            }

            DataLogger.AddText($"{numbers.Length} Member Rotational Restraints have been read.");
            DataLogger.InitializeProgressBar(0, numbers.Length, 0);
            for (int i = 0; i < count; i++)
            {
                aluminum_member_rotational_restraint objectToRead = SoapModelClient.get_aluminum_member_rotational_restraint(numbers[i]);
                LogAluminiumMemberRotationalRestraint(objectToRead);
                DataLogger.SetProgressBarValue(i);
            }
        }

        private static void LogAluminiumMemberRotationalRestraint(aluminum_member_rotational_restraint restraint)
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
            DataLogger.AddText($"Width of sheeting flange: {restraint.width_of_section_flange}");
            DataLogger.AddText($"Spring stiffness: {restraint.spring_stiffness}");
            DataLogger.AddText($"Beam spacing: {restraint.beam_spacing}");
            DataLogger.DecrementOffset();
        }

        public static bool Test_Types⁀for⁀Aluminium⁀Design_Member⁀Rotational⁀Restraint_Create()
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
                CreateAluminiumMemberRotationalRestraint();
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

        private static void CreateAluminiumMemberRotationalRestraint()
        {
            SoapModelClient.begin_modification(nameof(CreateAluminiumMemberRotationalRestraint));
            DataLogger.SetProgressBarValue(20);
            DataLogger.AddText("Generating...");
            var restraint = new aluminum_member_rotational_restraint
            {
                no = 1,
                name = "Generated restrain",
                type = aluminum_member_rotational_restraint_type.TYPE_CONTINUOUS,
                total_rotational_spring_stiffness = 0.45,
                spring_stiffnessSpecified = true,
                spring_stiffness = 0.45,
                material_name = "Grade S275",
                sheeting_name = "GOST C (-) 10-899-0.6",
                beam_spacing = 1.0005,
                beam_spacingSpecified = true,
            };
            SoapModelClient.set_aluminum_member_rotational_restraint(restraint);
            DataLogger.AddText("Generated");
            SoapModelClient.finish_modification();
            DataLogger.SetProgressBarValue(50);
        }

        public static bool Test_Types⁀for⁀Aluminium⁀Design_Member⁀Rotational⁀Restraint_Delete()
        {
            DataLogger.AddLogStart("Deleting...");
            try
            {
                bool result = DeleteObjects(object_types.E_OBJECT_TYPE_ALUMINUM_MEMBER_ROTATIONAL_RESTRAINT, 0, "Member rotational restraint");
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
