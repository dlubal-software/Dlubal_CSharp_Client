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
        public static bool Test_Types\u2040for\u2040Steel\u2040Design_Boundary\u2040Conditionals_Create()
        {
            const int COORDINATE_SYSTEM_NO = 1;
            const int NODAL_SUPPORTS_FIXED_IN_Y_AND_TORSION_1_NO = 2;
            const int NODAL_SUPPORTS_FIXED_IN_Y_AND_TORSION_2_NO = 3;
            const int NODAL_SUPPORTS_FIXED_IN_Y_1_NO = 4;
            const int NODAL_SUPPORTS_INDIVIDUALLY_1_NO = 5;
            const int MEMBER_HINGES_1_NO = 6;
            const int MEMBER_HINGES_2_NO = 7;
            const int BOUNDARY_CONDITIONALS_1_NO = 8;
            const int BOUNDARY_CONDITIONALS_2_NO = 9;

            DataLogger.AddLogStart("Creating boundary conditionals...");
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
                SoapModelClient.begin_modification("Create Boundary Conditionals");

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

                //nodal supports fixed in y and torsion
                steel_boundary_conditions_nodal_supports_row steelBCNodalSupports1 = new steel_boundary_conditions_nodal_supports_row()
                {
                    no = NODAL_SUPPORTS_FIXED_IN_Y_AND_TORSION_1_NO,
                    noSpecified = true,
                    row = new steel_boundary_conditions_nodal_supports
                    {
                        nodes = new int[] { 2, 3, 20 },
                        support_type = steel_boundary_conditions_nodal_supports_support_type.SUPPORT_TYPE_FIXED_IN_Y_AND_TORSION,
                        support_typeSpecified = true,
                        support_in_y = true,
                        support_in_ySpecified = true,
                        restraint_about_x = true,
                        restraint_about_xSpecified = true,
                        restraint_about_z = false,
                        restraint_about_zSpecified = true,
                        restraint_warping = false,
                        restraint_warpingSpecified = true,
                        eccentricity_type_z_type = eccentricity_type_z_type.ECCENTRICITY_TYPE_USER_VALUE,
                        eccentricity_type_z_typeSpecified = true,
                        eccentricity_x = 0.0,
                        eccentricity_xSpecified = true,
                        eccentricity_y = 0.0,
                        eccentricity_ySpecified = true,
                        rotation = 0.0,
                        rotationSpecified = true,
                    }
                };
                steel_boundary_conditions_nodal_supports_row steelBCNodalSupports2 = new steel_boundary_conditions_nodal_supports_row()
                {
                    no = NODAL_SUPPORTS_FIXED_IN_Y_AND_TORSION_2_NO,
                    noSpecified = true,
                    row = new steel_boundary_conditions_nodal_supports
                    {
                        nodes = new int[] { 3, 21 },
                        support_type = steel_boundary_conditions_nodal_supports_support_type.SUPPORT_TYPE_FIXED_IN_Y_AND_TORSION,
                        support_typeSpecified = true,
                        support_in_y = true,
                        support_in_ySpecified = true,
                        restraint_about_x = true,
                        restraint_about_xSpecified = true,
                        restraint_about_z = false,
                        restraint_about_zSpecified = true,
                        restraint_warping = false,
                        restraint_warpingSpecified = true,
                        eccentricity_type_z_type = eccentricity_type_z_type.ECCENTRICITY_TYPE_USER_VALUE,
                        eccentricity_type_z_typeSpecified = true,
                        eccentricity_x = 0.0,
                        eccentricity_xSpecified = true,
                        eccentricity_y = 0.0,
                        eccentricity_ySpecified = true,
                        rotation = 0.0,
                        rotationSpecified = true,
                    }
                };

                //nodal supports fixed in y
                steel_boundary_conditions_nodal_supports_row steelBCNodalSupports3 = new steel_boundary_conditions_nodal_supports_row()
                {
                    no = NODAL_SUPPORTS_FIXED_IN_Y_1_NO,
                    noSpecified = true,
                    row = new steel_boundary_conditions_nodal_supports
                    {
                        support_type = steel_boundary_conditions_nodal_supports_support_type.SUPPORT_TYPE_FIXED_IN_Y,
                        support_typeSpecified = true,
                        support_in_x = false,
                        support_in_xSpecified = true,
                        support_in_y = true,
                        support_in_ySpecified = true,
                        support_in_z = false,
                        support_in_zSpecified = true,
                        restraint_about_x = false,
                        restraint_about_xSpecified = true,
                        restraint_about_y = false,
                        restraint_about_ySpecified = true,
                        restraint_about_z = false,
                        restraint_about_zSpecified = true,
                        restraint_warping = false,
                        restraint_warpingSpecified = true,
                        eccentricity_type_z_type = eccentricity_type_z_type.ECCENTRICITY_TYPE_USER_VALUE,
                        eccentricity_type_z_typeSpecified = true,
                        eccentricity_x = 0.0,
                        eccentricity_xSpecified = true,
                        eccentricity_y = 0.0,
                        eccentricity_ySpecified = true,
                        rotation = 0.0,
                        rotationSpecified = true,
                    }
                };

                //nodal supports individually
                steel_boundary_conditions_nodal_supports_row steelBCNodalSupports4 = new steel_boundary_conditions_nodal_supports_row()
                {
                    no = NODAL_SUPPORTS_INDIVIDUALLY_1_NO,
                    noSpecified = true,
                    row = new steel_boundary_conditions_nodal_supports
                    {
                        support_type = steel_boundary_conditions_nodal_supports_support_type.SUPPORT_TYPE_INDIVIDUALLY,
                        support_typeSpecified = true,
                        support_in_x = false,
                        support_in_xSpecified = true,
                        support_in_y = false,
                        support_in_ySpecified = true,
                        support_in_z = false,
                        support_in_zSpecified = true,
                        support_spring_in_y = 2222.22,
                        support_spring_in_ySpecified = true,
                        restraint_about_x = true,
                        restraint_about_xSpecified = true,
                        restraint_about_y = false,
                        restraint_about_ySpecified = true,
                        restraint_about_z = true,
                        restraint_about_zSpecified = true,
                        restraint_warping = true,
                        restraint_warpingSpecified = true,
                        eccentricity_type_z_type = eccentricity_type_z_type.ECCENTRICITY_TYPE_USER_VALUE,
                        eccentricity_type_z_typeSpecified = true,
                        eccentricity_x = 0.2,
                        eccentricity_xSpecified = true,
                        eccentricity_y = 0.0,
                        eccentricity_ySpecified = true,
                        eccentricity_z = 0.07,
                        eccentricity_zSpecified = true,
                        rotation = 0.6,
                        rotationSpecified = true,
                    }
                };

                //member_hinges
                steel_boundary_conditions_member_hinges_row startSteelBCMemberHinges = new steel_boundary_conditions_member_hinges_row()
                {
                    no = MEMBER_HINGES_1_NO,
                    noSpecified = true,
                    row = new steel_boundary_conditions_member_hinges
                    {
                        node_seq_no = "Start",
                        nodes = new int[] { 2, 3, 20 },
                        release_in_y = false,
                        release_in_ySpecified = true,
                        release_about_x = true,
                        release_about_xSpecified = true,
                        release_about_z = false,
                        release_about_zSpecified = true,
                        release_warping = false,
                        release_warpingSpecified = true,
                    }
                };
                steel_boundary_conditions_member_hinges_row endSteelBCMemberHinges = new steel_boundary_conditions_member_hinges_row()
                {
                    no = MEMBER_HINGES_2_NO,
                    noSpecified = true,
                    row = new steel_boundary_conditions_member_hinges
                    {
                        node_seq_no = "End",
                        nodes = new int[] { 3, 21 },
                        release_in_y = false,
                        release_in_ySpecified = true,
                        release_about_x = true,
                        release_about_xSpecified = true,
                        release_about_z = false,
                        release_about_zSpecified = true,
                        release_warping = false,
                        release_warpingSpecified = true,
                    }
                };

                DataLogger.AddText($"Set Boundary Conditionals No.{BOUNDARY_CONDITIONALS_1_NO}");
                steel_boundary_conditions boundaryConditionals1 = new steel_boundary_conditions
                {
                    no = BOUNDARY_CONDITIONALS_1_NO,
                    definition_type = steel_boundary_conditions_definition_type.TYPE_2D,
                    definition_typeSpecified = true,
                    coordinate_system = "Local",
                    intermediate_nodes = false,
                    intermediate_nodesSpecified = true,
                    members = new int[] { 2, 14, 26 },
                    nodal_supports = new steel_boundary_conditions_nodal_supports_row[] { steelBCNodalSupports1, steelBCNodalSupports2 },
                    member_hinges = new steel_boundary_conditions_member_hinges_row[] { startSteelBCMemberHinges, endSteelBCMemberHinges }
                };
                SoapModelClient.set_steel_boundary_conditions(boundaryConditionals1);

                DataLogger.SetProgressBarValue(30);

                DataLogger.AddText($"Set Boundary Conditionals No.{BOUNDARY_CONDITIONALS_2_NO}");
                steel_boundary_conditions boundaryConditionals2 = new steel_boundary_conditions
                {
                    no = BOUNDARY_CONDITIONALS_2_NO,
                    definition_type = steel_boundary_conditions_definition_type.TYPE_2D,
                    definition_typeSpecified = true,
                    coordinate_system = "Local",
                    intermediate_nodes = true,
                    intermediate_nodesSpecified = true,
                    members = new int[] { 2, 14, 26 },
                    nodal_supports = new steel_boundary_conditions_nodal_supports_row[] { steelBCNodalSupports3, steelBCNodalSupports4 },
                    member_hinges = new steel_boundary_conditions_member_hinges_row[] { startSteelBCMemberHinges, endSteelBCMemberHinges }
                };
                SoapModelClient.set_steel_boundary_conditions(boundaryConditionals2);

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

        public static bool Test_Types\u2040for\u2040Steel\u2040Design_Boundary\u2040Conditionals_Get()
        {
            DataLogger.AddLogStart("Reading Boundary Conditionals...");

            try
            {
                int count = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_STEEL_BOUNDARY_CONDITIONS, 0);
                int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_STEEL_BOUNDARY_CONDITIONS, 0);

                if (count != numbers.Length)
                {
                    throw new Exception("Object count does not match object number count.");
                }

                DataLogger.AddText($"{numbers.Length} Boundary Conditionals numbers have been read.");
                DataLogger.InitializeProgressBar(0, numbers.Length, 0);

                for (int i = 0; i < count; i++)
                {
                    steel_boundary_conditions boundaryConditions = SoapModelClient.get_steel_boundary_conditions(numbers[i]);

                    DataLogger.AddText($"Boundary Conditionals No.{numbers[i]}");
                    DataLogger.AddText($"  - CS = {boundaryConditions.coordinate_system}");
                    DataLogger.AddText($"Intermediate nodes = {boundaryConditions.intermediate_nodes}");

                    steel_boundary_conditions_nodal_supports_row[] nodalSupports = boundaryConditions.nodal_supports;
                    for (int j = 0; j < nodalSupports.GetLength(0); j++)
                    {
                        DataLogger.AddText($"Nodal support No.{nodalSupports[j].no}");
                        DataLogger.AddText($"Type = {nodalSupports[j].row.support_type.ToString()}");
                    }

                    steel_boundary_conditions_member_hinges_row[] memberHinges = boundaryConditions.member_hinges;
                    for (int j = 0; j < memberHinges.GetLength(0); j++)
                    {
                        DataLogger.AddText($"Member hinges {memberHinges[j].row.node_seq_no}");
                    }
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

        public static bool Test_Types\u2040for\u2040Steel\u2040Design_Boundary\u2040Conditionals_Delete()
        {
            return DeleteObjects(object_types.E_OBJECT_TYPE_STEEL_BOUNDARY_CONDITIONS, 0, "boundary conditions");
        }
    }
}
