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
        public static bool Test_Types\u2040for\u2040Members_Member\u2040Eccentricities_Create()
        {
            const int GLOBAL_COORDINATE_SYSTEM = 1;
            const int COORDINATE_SYSTEM_NO = 2;
            const int MEMBER_ECCENTRICITY_1_NO = 3;
            const int MEMBER_ECCENTRICITY_2_NO = 4;
            const int MEMBER_ECCENTRICITY_3_NO = 5;

            DataLogger.AddLogStart("Creating member eccentricities...");
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
                SoapModelClient.begin_modification("Create member eccentricities");

                // coordinate system
                DataLogger.AddText($"Set coordinate system No.{COORDINATE_SYSTEM_NO}");
                coordinate_system coordinateSystem = new coordinate_system
                {
                    no = COORDINATE_SYSTEM_NO,
                    type = coordinate_system_type.TYPE_3_POINTS,
                    typeSpecified = true,
                    u_axis_point_coordinates = new vector_3d().SetCoordinates(1.0, 0.0, 1.0),
                    uw_plane_point_coordinates = new vector_3d().SetCoordinates(0.0, 0.0, 1.0)
                };
                SoapModelClient.set_coordinate_system(coordinateSystem);

                DataLogger.SetProgressBarValue(20);

                // member eccentricity with local CS
                DataLogger.AddText($"Set member eccentricity No.{MEMBER_ECCENTRICITY_1_NO}, CS = Local");
                member_eccentricity memberEccentricity1 = new member_eccentricity
                {
                    no = MEMBER_ECCENTRICITY_1_NO,
                    specification_type = member_eccentricity_specification_type.TYPE_RELATIVE_AND_ABSOLUTE,
                    specification_typeSpecified = true,
                    coordinate_system = "Local"
                };
                SoapModelClient.set_member_eccentricity(memberEccentricity1);

                DataLogger.SetProgressBarValue(30);

                // member eccentricity with global CS
                DataLogger.AddText($"Set member eccentricity No.{MEMBER_ECCENTRICITY_2_NO}, CS = Global");
                member_eccentricity memberEccentricity2 = new member_eccentricity
                {
                    no = MEMBER_ECCENTRICITY_2_NO,
                    specification_type = member_eccentricity_specification_type.TYPE_RELATIVE_AND_ABSOLUTE,
                    specification_typeSpecified = true,
                    coordinate_system = GLOBAL_COORDINATE_SYSTEM.ToString()
                };
                SoapModelClient.set_member_eccentricity(memberEccentricity2);

                DataLogger.SetProgressBarValue(40);

                // member eccentricity with user CS
                DataLogger.AddText($"Set member eccentricity No.{MEMBER_ECCENTRICITY_3_NO}, CS = User No.{COORDINATE_SYSTEM_NO}");
                member_eccentricity memberEccentricity3 = new member_eccentricity
                {
                    no = MEMBER_ECCENTRICITY_3_NO,
                    specification_type = member_eccentricity_specification_type.TYPE_RELATIVE_AND_ABSOLUTE,
                    specification_typeSpecified = true,
                    coordinate_system = COORDINATE_SYSTEM_NO.ToString()
                };
                SoapModelClient.set_member_eccentricity(memberEccentricity3);

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

        public static bool Test_Types\u2040for\u2040Members_Member\u2040Eccentricities_Get()
        {
            DataLogger.AddLogStart("Reading member eccentricities...");

            try
            {
                int count = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_MEMBER_ECCENTRICITY, 0);
                int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_MEMBER_ECCENTRICITY, 0);

                if (count != numbers.Length)
                {
                    throw new Exception("Object count does not match object number count.");
                }

                DataLogger.AddText($"{numbers.Length} member eccentricity numbers have been read.");
                DataLogger.InitializeProgressBar(0, numbers.Length, 0);

                for (int i = 0; i < count; i++)
                {
                    member_eccentricity eccentricity = SoapModelClient.get_member_eccentricity(numbers[i]);

                    DataLogger.AddText($"Member eccentricity No.{numbers[i]}");
                    DataLogger.AddText($"  - CS = {eccentricity.coordinate_system}");
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

        public static bool Test_Types\u2040for\u2040Members_Member\u2040Eccentricities_Delete()
        {
            return DeleteObjects(object_types.E_OBJECT_TYPE_MEMBER_ECCENTRICITY, 0, "member eccentricities");
        }
    }
}
