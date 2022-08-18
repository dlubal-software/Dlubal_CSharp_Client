using Dlubal.WS.Clients.DotNetClientTest.Tools;
using Dlubal.WS.Common.Tools;
using System;
using System.Collections.Generic;

#if RFEM
using Dlubal.WS.Rfem6.Model;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
        public static bool Test_Loads_Nodal\u2040Loads_Create()
        {
            const int NODE_1_NO = 1;
            const int NODE_2_NO = 2;
            const int NODE_3_NO = 3;
            const int NODE_4_NO = 4;
            const int LOAD_CASE_NO = 5;
            const int NODAL_LOAD_1_NO = 6;
            const int NODAL_LOAD_2_NO = 7;
            const int NODAL_LOAD_3_NO = 8;
            const int NODAL_LOAD_4_NO = 9;

            DataLogger.AddLogStart("Creating nodal loads...");
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
                SoapModelClient.begin_modification("set nodal loads");

                // nodes
                DataLogger.AddText("Set nodes...");
                List<node> nodes = new List<node>
                {
                    new node { no = NODE_1_NO, coordinates = new vector_3d().SetCoordinates(1.0, -1.0, 0.0) },
                    new node { no = NODE_2_NO, coordinates = new vector_3d().SetCoordinates(4.0, -1.0, 0.0) },
                    new node { no = NODE_3_NO, coordinates = new vector_3d().SetCoordinates(8.0, -1.0, 0.0) },
                    new node { no = NODE_4_NO, coordinates = new vector_3d().SetCoordinates(12.0, -1.0, 0.0) }
                };
                nodes.ForEach(node => SoapModelClient.set_node(node));

                DataLogger.SetProgressBarValue(20);

                // load case
                SetLoadCase(LOAD_CASE_NO);

                DataLogger.SetProgressBarValue(30);

                // nodal loads
                DataLogger.AddText("Set nodal loads...");
                nodal_load nodalLoad1 = new nodal_load
                {
                    no = NODAL_LOAD_1_NO,
                    nodes = new int[] { NODE_1_NO },
                    load_type = nodal_load_load_type.LOAD_TYPE_FORCE,
                    load_typeSpecified = true,
                    load_direction = nodal_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W,
                    load_directionSpecified = true,
                    force_magnitude = 10000.0,
                    force_magnitudeSpecified = true
                };
                SoapModelClient.set_nodal_load(LOAD_CASE_NO, nodalLoad1);

                DataLogger.SetProgressBarValue(40);

                nodal_load nodalLoad2 = new nodal_load
                {
                    no = NODAL_LOAD_2_NO,
                    nodes = new int[] { NODE_2_NO },
                    load_type = nodal_load_load_type.LOAD_TYPE_MOMENT,
                    load_typeSpecified = true,
                    load_direction = nodal_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V,
                    load_directionSpecified = true,
                    moment_magnitude = 20000.0,
                    moment_magnitudeSpecified = true
                };
                SoapModelClient.set_nodal_load(LOAD_CASE_NO, nodalLoad2);

                DataLogger.SetProgressBarValue(50);

                nodal_load nodalLoad3 = new nodal_load
                {
                    no = NODAL_LOAD_3_NO,
                    nodes = new int[] { NODE_3_NO },
                    load_type = nodal_load_load_type.LOAD_TYPE_MASS,
                    load_typeSpecified = true,
                    load_direction = nodal_load_load_direction.LOAD_DIRECTION_LOCAL_Z,
                    load_directionSpecified = true,
                    mass_global = 5000.0,
                    mass_globalSpecified = true
                };
                SoapModelClient.set_nodal_load(LOAD_CASE_NO, nodalLoad3);

                DataLogger.SetProgressBarValue(60);

                nodal_load nodalLoad4 = new nodal_load
                {
                    no = NODAL_LOAD_4_NO,
                    nodes = new int[] { NODE_4_NO },
                    load_type = nodal_load_load_type.LOAD_TYPE_COMPONENTS,
                    load_typeSpecified = true,
                    components_force = new vector_3d().SetCoordinates(10000.0, 20000.0, 30000.0),
                    components_moment = new vector_3d().SetCoordinates(11000.0, 21000.0, 31000.0)
                };
                SoapModelClient.set_nodal_load(LOAD_CASE_NO, nodalLoad4);

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

        public static bool Test_Loads_Nodal\u2040Loads_Get()
        {
            DataLogger.AddLogStart("Reading nodal loads...");

            try
            {
                int[] loadCaseNumbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_LOAD_CASE, 0);
                foreach (int loadCaseNo in loadCaseNumbers)
                {
                    DataLogger.AddLogStart($"Load case No.{loadCaseNo}");

                    int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_NODAL_LOAD, loadCaseNo);

                    DataLogger.InitializeProgressBar(0, numbers.Length, 0);

                    for (int i = 0; i < numbers.Length; i++)
                    {
                        nodal_load nodalLoad = SoapModelClient.get_nodal_load(numbers[i], loadCaseNo);

                        DataLogger.AddText($"Nodal load No.{nodalLoad.no}");
                        DataLogger.AddText($"  - Type = {nodalLoad.load_type}");
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

        public static bool Test_Loads_Nodal\u2040Loads_Delete()
        {
            return DeleteAllLoads(object_types.E_OBJECT_TYPE_NODAL_LOAD, object_types.E_OBJECT_TYPE_LOAD_CASE, "nodal loads");
        }
    }
}
