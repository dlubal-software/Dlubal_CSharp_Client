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
        public static bool Test_Types⁀for⁀Timber⁀Design_Member⁀Rotational⁀Restraint_Get()
        {
            try
            {
                ReadTimberMemberRotationalRestraint();
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

        private static void ReadTimberMemberRotationalRestraint()
        {
            DataLogger.AddLogStart("Reading...");
            int count = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_MEMBER_ROTATIONAL_RESTRAINT, 0);
            int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_MEMBER_ROTATIONAL_RESTRAINT, 0);
            if (count != numbers.Length)
            {
                throw new Exception("Object count does not match object number count.");
            }

            DataLogger.AddText($"{numbers.Length} objects have been read.");
            DataLogger.InitializeProgressBar(0, numbers.Length, 0);
            for (int i = 0; i < count; i++)
            {
                member_rotational_restraint objectToRead = SoapModelClient.get_member_rotational_restraint(numbers[i]);
                LogTimberMemberRotationalRestraint(objectToRead);
                DataLogger.SetProgressBarValue(i);
            }
        }

        private static void LogTimberMemberRotationalRestraint(member_rotational_restraint restraint)
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

            DataLogger.AddText($"Total rotational spring stiffness: {restraint.total_rotational_spring_stiffness}");
            DataLogger.DecrementOffset();
        }

        public static bool Test_Types⁀for⁀Timber⁀Design_Member⁀Rotational⁀Restraint_Create()
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
                CreateTimberMemberRotationalRestraint();
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

        private static void CreateTimberMemberRotationalRestraint()
        {
            SoapModelClient.begin_modification(nameof(CreateTimberMemberRotationalRestraint));
            DataLogger.SetProgressBarValue(20);
            DataLogger.AddText("Generating...");
            var restraint = new member_rotational_restraint
            {
                no = 1,
                name = "Generated",
                type = member_rotational_restraint_type.TYPE_CONTINUOUS,
                total_rotational_spring_stiffness = 7255,
                total_rotational_spring_stiffnessSpecified = true,
            };
            SoapModelClient.set_member_rotational_restraint(restraint);
            DataLogger.AddText("Generated");
            SoapModelClient.finish_modification();
            DataLogger.SetProgressBarValue(50);
        }

        public static bool Test_Types⁀for⁀Timber⁀Design_Member⁀Rotational⁀Restraint_Delete()
        {
            DataLogger.AddLogStart("Deleting...");
            try
            {
                bool result = DeleteObjects(object_types.E_OBJECT_TYPE_MEMBER_ROTATIONAL_RESTRAINT, 0, "Member rotational restraint");
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
