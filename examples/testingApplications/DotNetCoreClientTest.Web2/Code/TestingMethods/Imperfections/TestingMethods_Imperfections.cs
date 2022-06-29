using Dlubal.WS.RfemRstabSoapWsCoreLib.Tools;
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
        /// <summary>
        /// Sets imperfection case of a specified type.
        /// </summary>
        /// <param name="caseNo">Imperfection case No.</param>
        /// <param name="type">Type of imperfection case.</param>
        /// <param name="tableRowCount">Count of rows in the table.</param>
        private static void SetImperfectionCase(int caseNo, imperfection_case_type type, int tableRowCount = 0)
        {
            const int COORDINATE_SYSTEM_NO = 1;
            const int ASSIGNMENT_TO_LC_NO = 4;
            const int REFERENCE_NODE_NO = 1;

            DataLogger.AddText($"Set imperfection case No.{caseNo}, Type = {type}");

            imperfection_case imperfectionCase;

            switch (type)
            {
                case imperfection_case_type.IMPERFECTION_TYPE_BUCKLING_MODE:
                {
                    imperfectionCase = new imperfection_case
                    {
                        source = imperfection_case_source.SOURCE_TYPE_LOAD_CASE,
                        sourceSpecified = true,

                        assigned_to_load_cases = new int[] { ASSIGNMENT_TO_LC_NO },

                        shape_from_load_case = 1,
                        shape_from_load_caseSpecified = true,

                        buckling_shape = 3,
                        buckling_shapeSpecified = true,

                        magnitude_assignment_type = imperfection_case_magnitude_assignment_type.MAGNITUDE_ASSIGNMENT_SPECIFIC_NODE,
                        magnitude_assignment_typeSpecified = true,

                        reference_node = REFERENCE_NODE_NO,
                        reference_nodeSpecified = true,

                        coordinate_system = COORDINATE_SYSTEM_NO,
                        coordinate_systemSpecified = true,

                        direction = imperfection_case_direction.IMPERFECTION_CASE_DIRECTION_LOCAL_X,
                        directionSpecified = true,

                        direction_for_level_direction = imperfection_case_direction_for_level_direction.DIRECTION_Y,
                        direction_for_level_directionSpecified = true,

                        delta_zero = 1.111,
                        delta_zeroSpecified = true,

                    }.SetDefault(caseNo, type);
                    break;
                }
                case imperfection_case_type.IMPERFECTION_TYPE_DYNAMIC_EIGENMODE:
                {
                    imperfectionCase = new imperfection_case
                    {
                        source = imperfection_case_source.SOURCE_TYPE_LOAD_CASE,
                        sourceSpecified = true,

                        assigned_to_load_cases = new int[] { ASSIGNMENT_TO_LC_NO },

                        shape_from_load_case = 2,
                        shape_from_load_caseSpecified = true,

                        buckling_shape = 1,
                        buckling_shapeSpecified = true,

                        magnitude_assignment_type = imperfection_case_magnitude_assignment_type.MAGNITUDE_ASSIGNMENT_LOCATION_WITH_LARGEST_DISPLACEMENT,
                        magnitude_assignment_typeSpecified = true,

                        coordinate_system = COORDINATE_SYSTEM_NO,
                        coordinate_systemSpecified = true,

                        direction = imperfection_case_direction.IMPERFECTION_CASE_DIRECTION_LOCAL_X,
                        directionSpecified = true,

                        direction_for_level_direction = imperfection_case_direction_for_level_direction.DIRECTION_Y,
                        direction_for_level_directionSpecified = true,

                        delta_zero = 2.222,
                        delta_zeroSpecified = true,

                    }.SetDefault(caseNo, type);
                    break;
                }
                case imperfection_case_type.IMPERFECTION_TYPE_IMPERFECTION_CASES_GROUP:
                {
                    imperfectionCase = new imperfection_case
                    {
                        imperfection_cases_items = new imperfection_case_imperfection_cases_items[tableRowCount]

                    }.SetDefault(caseNo, type);

                    for (int i = 0; i < tableRowCount; i++)
                    {
                        imperfectionCase.imperfection_cases_items[i] = new imperfection_case_imperfection_cases_items
                        {
                            no = i + 1,
                            noSpecified = true,
                            name = i + 1,
                            nameSpecified = true,
                            factor = i + 1.1,
                            factorSpecified = true,
                            operator_type = operator_type.OPERATOR_OR,
                            operator_typeSpecified = true,
                            comment = $"comment {i + 1}",
                        };
                    }

                    break;
                }
                case imperfection_case_type.IMPERFECTION_TYPE_INITIAL_SWAY_VIA_TABLE:
                {
                    imperfectionCase = new imperfection_case
                    {
                        coordinate_system = COORDINATE_SYSTEM_NO,
                        coordinate_systemSpecified = true,

                        direction = imperfection_case_direction.IMPERFECTION_CASE_DIRECTION_LOCAL_X,
                        directionSpecified = true,

                        direction_for_level_direction = imperfection_case_direction_for_level_direction.DIRECTION_Y,
                        direction_for_level_directionSpecified = true,

                        sway_coefficients_reciprocal = false,
                        sway_coefficients_reciprocalSpecified = true,

                    }.SetDefault(caseNo, type);

                    imperfectionCase.level_imperfections = new imperfection_case_level_imperfections[tableRowCount];

                    for (int i = 0; i < tableRowCount; i++)
                    {
                        imperfectionCase.level_imperfections[i] = new imperfection_case_level_imperfections
                        {
                            no = i + 1,
                            level = i / 10.0,
                            e_1 = Math.Round(((i + 1) / 10.0) + 0.01, 3, MidpointRounding.AwayFromZero),
                            theta_1 = Math.Round(((i + 1) / 10.0) + 0.02, 3, MidpointRounding.AwayFromZero),
                            e_2 = Math.Round(((i + 1) / 10.0) + 0.03, 3, MidpointRounding.AwayFromZero),
                            theta_2 = Math.Round(((i + 1) / 10.0) + 0.04, 3, MidpointRounding.AwayFromZero),
                            comment = $"comment row {i + 1}"
                        };
                    }

                    break;
                }
                //case imperfection_case_type.IMPERFECTION_TYPE_LOCAL_IMPERFECTIONS_ONLY:
                //{
                //    imperfectionCase = new imperfection_case().SetDefault(caseNo, type);
                //    break;
                //}
                case imperfection_case_type.IMPERFECTION_TYPE_NOTIONAL_LOADS_FROM_LOAD_CASE:
                {
                    imperfectionCase = new imperfection_case
                    {
                        load_case_for_notional_loads = 3,
                        load_case_for_notional_loadsSpecified = true

                    }.SetDefault(caseNo, type);
                    break;
                }
                case imperfection_case_type.IMPERFECTION_TYPE_STATIC_DEFORMATION:
                {
                    imperfectionCase = new imperfection_case
                    {
                        source = imperfection_case_source.SOURCE_TYPE_LOAD_CASE,
                        sourceSpecified = true,

                        shape_from_load_case = 4,
                        shape_from_load_caseSpecified = true,

                        magnitude_assignment_type = imperfection_case_magnitude_assignment_type.MAGNITUDE_ASSIGNMENT_LOCATION_WITH_LARGEST_DISPLACEMENT,
                        magnitude_assignment_typeSpecified = true,

                        coordinate_system = COORDINATE_SYSTEM_NO,
                        coordinate_systemSpecified = true,

                        direction = imperfection_case_direction.IMPERFECTION_CASE_DIRECTION_LOCAL_X,
                        directionSpecified = true,

                        delta_zero = 0.3,
                        delta_zeroSpecified = true,

                    }.SetDefault(caseNo, type);
                    break;
                }
                default:
                {
                    throw new NotImplementedException();
                }
            }

            SoapModelClient.set_imperfection_case(imperfectionCase);
        }

        public static bool Test_Imperfections_Imperfection\u2040Cases_Create()
        {
            const int NODE_NO = 1;
            const int COORDINATE_SYSTEM_NO = 1;

            const int LOAD_CASE_1_NO = 1;
            const int LOAD_CASE_2_NO = 2;
            const int LOAD_CASE_3_NO = 3;
            const int LOAD_CASE_4_NO = 4;

            const int IMPERFECTION_CASE_1_NO = 1;
            const int IMPERFECTION_CASE_2_NO = 2;
            const int IMPERFECTION_CASE_3_NO = 3;
            const int IMPERFECTION_CASE_4_NO = 4;
            const int IMPERFECTION_CASE_5_NO = 5;
            const int IMPERFECTION_CASE_6_NO = 6;
            const int IMPERFECTION_CASE_7_NO = 7;

            DataLogger.AddLogStart("Creating imperfection cases...");
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
                SoapModelClient.begin_modification("set imperfection cases");

                // set node
                DataLogger.AddText($"Set node No.{NODE_NO}");
                node node = new node
                {
                    no = NODE_NO,
                    coordinates = new vector_3d().SetCoordinates(1.0, 1.0, 0.0)
                };
                SoapModelClient.set_node(node);

                DataLogger.SetProgressBarValue(20);

                // set coordinate system
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

                DataLogger.SetProgressBarValue(30);

                // set load cases
                DataLogger.AddText("Set load cases...");
                SetLoadCase(LOAD_CASE_1_NO);
                SetLoadCase(LOAD_CASE_2_NO);
                SetLoadCase(LOAD_CASE_3_NO, noneActionCategory: true);
                SetLoadCase(LOAD_CASE_4_NO);

                DataLogger.SetProgressBarValue(40);

                // set imperfection cases
                DataLogger.AddText("Set imperfection cases...");
                SetImperfectionCase(IMPERFECTION_CASE_1_NO, imperfection_case_type.IMPERFECTION_TYPE_BUCKLING_MODE);
                SetImperfectionCase(IMPERFECTION_CASE_2_NO, imperfection_case_type.IMPERFECTION_TYPE_DYNAMIC_EIGENMODE);
                SetImperfectionCase(IMPERFECTION_CASE_3_NO, imperfection_case_type.IMPERFECTION_TYPE_IMPERFECTION_CASES_GROUP, 1);
                SetImperfectionCase(IMPERFECTION_CASE_4_NO, imperfection_case_type.IMPERFECTION_TYPE_INITIAL_SWAY_VIA_TABLE, 1);
                //SetImperfectionCase(IMPERFECTION_CASE_5_NO, imperfection_case_type.IMPERFECTION_TYPE_LOCAL_IMPERFECTIONS_ONLY);
                SetImperfectionCase(IMPERFECTION_CASE_6_NO, imperfection_case_type.IMPERFECTION_TYPE_NOTIONAL_LOADS_FROM_LOAD_CASE);
                SetImperfectionCase(IMPERFECTION_CASE_7_NO, imperfection_case_type.IMPERFECTION_TYPE_STATIC_DEFORMATION);

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
    }
}