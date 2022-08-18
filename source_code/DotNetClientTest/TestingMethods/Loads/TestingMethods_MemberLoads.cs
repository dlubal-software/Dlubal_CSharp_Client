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
        public static bool Test_Loads_Member\u2040Loads_Create()
        {
            const int GLOBAL_COORDINATE_SYSTEM = 1;
            const int MATERIAL_NO = 1;
            const int SECTION_NO = 2;
            const int COORDINATE_SYSTEM_NO = 3;
            const int MEMBER_NO = 4;
            const int LOAD_CASE_NO = 5;
            const int MEMBER_LOAD_1_NO = 6;
            const int MEMBER_LOAD_2_NO = 7;
            const int MEMBER_LOAD_3_NO = 8;
            const int MEMBER_LOAD_4_NO = 9;

            DataLogger.AddLogStart("Create member loads...");
            DataLogger.InitializeProgressBar(0, 70, 0);

            bool result = Test_General_Delete⁀All();
            if (!result || (SoapModelClient == null))
            {
                DataLogger.AddLogEnd(DataLogger.LogResultType.FAILED);
                return false;
            }

            DataLogger.SetProgressBarValue(10);

            try
            {
                SoapModelClient.begin_modification("create member loads");

                // member
                SetDefaultMember(MEMBER_NO, 0.0, MATERIAL_NO, SECTION_NO, COORDINATE_SYSTEM_NO);

                DataLogger.SetProgressBarValue(20);

                // load case
                SetLoadCase(LOAD_CASE_NO);

                DataLogger.SetProgressBarValue(30);

                // member load
                DataLogger.AddText($"Set member load No.{MEMBER_LOAD_1_NO}, CS = Principal");
                member_load memberLoad1 = new member_load
                {
                    no = MEMBER_LOAD_1_NO,
                    members = new int[] { MEMBER_NO },
                    magnitude = 10000.0,
                    magnitudeSpecified = true,
                    coordinate_system = "Principal"
                };
                SoapModelClient.set_member_load(LOAD_CASE_NO, memberLoad1);

                DataLogger.SetProgressBarValue(40);

                // member load
                DataLogger.AddText($"Set member load No.{MEMBER_LOAD_2_NO}, CS = Local");
                member_load memberLoad2 = new member_load
                {
                    no = MEMBER_LOAD_2_NO,
                    members = new int[] { MEMBER_NO },
                    magnitude = 20000.0,
                    magnitudeSpecified = true,
                    coordinate_system = "Local"
                };
                SoapModelClient.set_member_load(LOAD_CASE_NO, memberLoad2);

                DataLogger.SetProgressBarValue(50);

                // member load
                DataLogger.AddText($"Set member load No.{MEMBER_LOAD_3_NO}, CS = Global");
                member_load memberLoad3 = new member_load
                {
                    no = MEMBER_LOAD_3_NO,
                    members = new int[] { MEMBER_NO },
                    magnitude = 30000.0,
                    magnitudeSpecified = true,
                    coordinate_system = GLOBAL_COORDINATE_SYSTEM.ToString()
                };
                SoapModelClient.set_member_load(LOAD_CASE_NO, memberLoad3);

                DataLogger.SetProgressBarValue(60);

                // member load
                DataLogger.AddText($"Set member load No.{MEMBER_LOAD_4_NO}, CS = User No.{COORDINATE_SYSTEM_NO}");
                member_load memberLoad4 = new member_load
                {
                    no = MEMBER_LOAD_4_NO,
                    members = new int[] { MEMBER_NO },
                    magnitude = 40000.0,
                    magnitudeSpecified = true,
                    coordinate_system = COORDINATE_SYSTEM_NO.ToString()
                };
                SoapModelClient.set_member_load(LOAD_CASE_NO, memberLoad4);

                DataLogger.SetProgressBarValue(70);
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

        public static bool Test_Loads_Member\u2040Loads_Get()
        {
            DataLogger.AddLogStart("Reading member loads...");

            try
            {
                int[] loadCaseNumbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_LOAD_CASE, 0);
                foreach (int loadCaseNo in loadCaseNumbers)
                {
                    DataLogger.AddLogStart($"Load case No.{loadCaseNo}");

                    int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_MEMBER_LOAD, loadCaseNo);

                    DataLogger.InitializeProgressBar(0, numbers.Length, 0);

                    for (int i = 0; i < numbers.Length; i++)
                    {
                        member_load memberLoad = SoapModelClient.get_member_load(numbers[i], loadCaseNo);

                        DataLogger.AddText($"Member load No.{memberLoad.no}");
                        DataLogger.AddText($"  - CS = {memberLoad.coordinate_system}");
                        DataLogger.SetProgressBarValue(i);
                    }

                    DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
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

        public static bool Test_Loads_Member\u2040Loads_Delete()
        {
            return DeleteAllLoads(object_types.E_OBJECT_TYPE_MEMBER_LOAD, object_types.E_OBJECT_TYPE_LOAD_CASE, "member loads");
        }
    }
}
