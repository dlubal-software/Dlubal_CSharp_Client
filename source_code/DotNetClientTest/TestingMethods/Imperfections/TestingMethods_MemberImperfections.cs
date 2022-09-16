#if RFEM || RSTAB

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
        public static bool Test_Imperfections_Member\u2040Imperfections_Create()
        {
            const int COORDINATE_SYSTEM_NO = 1;
            const int IMPERFECTION_CASE_NO = 2;
            const int MEMBER_IMPERFECTION_1_NO = 3;
            const int MEMBER_IMPERFECTION_2_NO = 4;
            const int MEMBER_IMPERFECTION_3_NO = 5;
            const int MEMBER_IMPERFECTION_4_NO = 6;
            const int MEMBER_IMPERFECTION_5_NO = 7;

            DataLogger.AddLogStart("Create member imperfections...");
            DataLogger.InitializeProgressBar(0, 70, 0);

            bool succeeded = InitializeTest();
            if (!succeeded)
            {
                return false;
            }

            DataLogger.SetProgressBarValue(10);

            try
            {
                SoapModelClient.begin_modification("set member imperfections");

                // imperfection case
                DataLogger.AddText($"Set imperfection case No.{IMPERFECTION_CASE_NO}");
                imperfection_case imperfectionCase = new imperfection_case
                {
                    no = IMPERFECTION_CASE_NO,
                    name = "Testing Imperfection Case"
                };
                SoapModelClient.set_imperfection_case(imperfectionCase);

                DataLogger.SetProgressBarValue(20);

                // member imperfection
                SetDefaultMember(MEMBER_IMPERFECTION_1_NO, 1.0, sectionNo: COORDINATE_SYSTEM_NO);
                DataLogger.AddText($"Set member imperfection No.{MEMBER_IMPERFECTION_1_NO}, CS = Principal");
                member_imperfection memberImperfection1 = new member_imperfection
                {
                    no = MEMBER_IMPERFECTION_1_NO,
                    members = new int[] { MEMBER_IMPERFECTION_1_NO },
                    coordinate_system = "Principal",
                    basic_value_absolute = 0.5,
                    basic_value_absoluteSpecified = true,
                    comment = $"IC{imperfectionCase.no} - member imperfection {MEMBER_IMPERFECTION_1_NO}"
                };
                SoapModelClient.set_member_imperfection(imperfectionCase.no, memberImperfection1);

                DataLogger.SetProgressBarValue(30);

                // member imperfection
                SetDefaultMember(MEMBER_IMPERFECTION_2_NO, 2.0, sectionNo: COORDINATE_SYSTEM_NO);
                DataLogger.AddText($"Set member imperfection No.{MEMBER_IMPERFECTION_2_NO}, CS = Local");
                member_imperfection memberImperfection2 = new member_imperfection
                {
                    no = MEMBER_IMPERFECTION_2_NO,
                    members = new int[] { MEMBER_IMPERFECTION_2_NO },
                    coordinate_system = "Local",
                    basic_value_absolute = 0.6,
                    basic_value_absoluteSpecified = true,
                    comment = $"IC{imperfectionCase.no} - member imperfection {MEMBER_IMPERFECTION_2_NO}"
                };
                SoapModelClient.set_member_imperfection(imperfectionCase.no, memberImperfection2);

                DataLogger.SetProgressBarValue(40);

                // member imperfection
                SetDefaultMember(MEMBER_IMPERFECTION_3_NO, 3.0, sectionNo: COORDINATE_SYSTEM_NO);
                DataLogger.AddText($"Set member imperfection No.{MEMBER_IMPERFECTION_3_NO}, Type = INITIAL_BOW...");
                member_imperfection memberImperfection3 = new member_imperfection
                {
                    no = MEMBER_IMPERFECTION_3_NO,
                    members = new int[] { MEMBER_IMPERFECTION_3_NO },
                    imperfection_type = member_imperfection_imperfection_type.IMPERFECTION_TYPE_INITIAL_BOW,
                    imperfection_typeSpecified = true,
                    basic_value_absolute = 0.7,
                    basic_value_absoluteSpecified = true,
                    imperfection_case = imperfectionCase.no,
                    imperfection_caseSpecified = true,
                    comment = $"IC{imperfectionCase.no} - member imperfection {MEMBER_IMPERFECTION_3_NO}"
                };
                SoapModelClient.set_member_imperfection(imperfectionCase.no, memberImperfection3);

                DataLogger.SetProgressBarValue(50);

                // member imperfection
                SetDefaultMember(MEMBER_IMPERFECTION_4_NO, 4.0, sectionNo: COORDINATE_SYSTEM_NO);
                DataLogger.AddText($"Set member imperfection No.{MEMBER_IMPERFECTION_4_NO}, Type = INITIAL_BOW_AND_CRITERION...");
                member_imperfection memberImperfection4 = new member_imperfection
                {
                    no = MEMBER_IMPERFECTION_4_NO,
                    members = new int[] { MEMBER_IMPERFECTION_4_NO },
                    imperfection_type = member_imperfection_imperfection_type.IMPERFECTION_TYPE_INITIAL_BOW_AND_CRITERION,
                    imperfection_typeSpecified = true,
                    basic_value_absolute = 0.8,
                    basic_value_absoluteSpecified = true,
                    active_criterion = member_imperfection_active_criterion.ACTIVITY_CRITERION_EN_1999,
                    active_criterionSpecified = true,
                    comment = $"IC{imperfectionCase.no} - member imperfection {MEMBER_IMPERFECTION_4_NO}"
                };
                SoapModelClient.set_member_imperfection(imperfectionCase.no, memberImperfection4);

                DataLogger.SetProgressBarValue(60);

                // member imperfection
                SetDefaultMember(MEMBER_IMPERFECTION_5_NO, 5.0, sectionNo: COORDINATE_SYSTEM_NO);
                DataLogger.AddText($"Set member imperfection No.{MEMBER_IMPERFECTION_2_NO}, Type = INITIAL_SWAY...");
                member_imperfection memberImperfection5 = new member_imperfection
                {
                    no = MEMBER_IMPERFECTION_5_NO,
                    members = new int[] { MEMBER_IMPERFECTION_5_NO },
                    imperfection_type = member_imperfection_imperfection_type.IMPERFECTION_TYPE_INITIAL_SWAY,
                    imperfection_typeSpecified = true,
                    basic_value_absolute = 0.9,
                    basic_value_absoluteSpecified = true,
                    comment = $"IC{imperfectionCase.no} - member imperfection {MEMBER_IMPERFECTION_5_NO}"
                };
                SoapModelClient.set_member_imperfection(imperfectionCase.no, memberImperfection5);

                DataLogger.SetProgressBarValue(70);
                succeeded = true;
            }
            catch (Exception exception)
            {
                succeeded = false;
                ModelWsExceptionHandler(exception);
            }
            finally
            {
                try
                {
                    SoapModelClient.finish_modification();
                    DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
                }
                catch (Exception exception)
                {
                    succeeded = false;
                    ModelWsExceptionHandler(exception);
                    SoapModelClient.reset();
                }

                DataLogger.ResetProgressBar();
            }

            return succeeded;
        }

        public static bool Test_Imperfections_Member\u2040Imperfections_Get()
        {
            DataLogger.AddLogStart("Reading member imperfections...");

            try
            {
                int[] imperfectionCaseNumbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_IMPERFECTION_CASE, 0);
                foreach (int imperfectionCaseNo in imperfectionCaseNumbers)
                {
                    DataLogger.AddLogStart($"Imperfection case No.{imperfectionCaseNo}");

                    int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_MEMBER_IMPERFECTION, imperfectionCaseNo);

                    DataLogger.InitializeProgressBar(0, numbers.Length, 0);

                    for (int i = 0; i < numbers.Length; i++)
                    {
                        member_imperfection memberImperfection = SoapModelClient.get_member_imperfection(numbers[i], imperfectionCaseNo);

                        DataLogger.AddText($"Member imperfection No.{memberImperfection.no}");
                        DataLogger.AddText($"  - Type = {memberImperfection.imperfection_type}");
                        DataLogger.AddText($"  - CS = {memberImperfection.coordinate_system}");

                        if (memberImperfection.parameters != null)
                        {
                            DataLogger.AddText($"  - Parameters = {memberImperfection.parameters}");
                        }

                        if (!string.IsNullOrEmpty(memberImperfection.parameters_string))
                        {
                            DataLogger.AddText($"  - Parameters (string) = {memberImperfection.parameters_string}");
                        }

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

        public static bool Test_Imperfections_Member\u2040Imperfections_Delete\u2040All()
        {
            return DeleteAllLoads(object_types.E_OBJECT_TYPE_MEMBER_IMPERFECTION, object_types.E_OBJECT_TYPE_IMPERFECTION_CASE, "member imperfections");
        }
    }
}

#endif // RFEM RSTAB