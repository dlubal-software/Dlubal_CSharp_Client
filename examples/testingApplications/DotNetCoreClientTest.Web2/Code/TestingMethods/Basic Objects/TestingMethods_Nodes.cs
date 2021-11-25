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
        public static bool Test_Basic\u2040Objects_Nodes_Create\u2040Nodes()
        {
            const int NODE_1_NO = 1;
            const int NODE_2_NO = 2;
            const int NODE_3_NO = 3;
            const int NODE_4_NO = 4;
            const int NODE_5_NO = 5;
            const int NODE_6_NO = 6;
            const int LINE_NO = 7;
            const int MATERIAL_NO = 8;
            const int SECTION_NO = 9;
            const int MEMBER_NO = 10;

            DataLogger.AddLogStart("Creating nodes...");
            DataLogger.InitializeProgressBar(0, 100, 0);

            bool result = Test_General_Delete⁀All();
            if (!result || (SoapModelClient == null))
            {
                DataLogger.AddLogEnd(DataLogger.LogResultType.FAILED);
                return false;
            }

            DataLogger.SetProgressBarValue(5);

            try
            {
                SoapModelClient.begin_modification("set nodes");

                // standard nodes
                DataLogger.AddText("Set 2 standard nodes.");

                node node1 = new node
                {
                    no = NODE_1_NO,
                    type = node_type.TYPE_STANDARD,
                    typeSpecified = true,
                    coordinates = new vector_3d().SetCoordinates(1.0, 1.0, 0.0)
                };
                SoapModelClient.set_node(node1);

                DataLogger.SetProgressBarValue(10);

                node node2 = new node
                {
                    no = NODE_2_NO,
                    type = node_type.TYPE_STANDARD,
                    typeSpecified = true,
                    coordinates = new vector_3d().SetCoordinates(5.0, 1.0, -1.0)
                };
                SoapModelClient.set_node(node2);

                DataLogger.SetProgressBarValue(20);

                // node between two points
                DataLogger.AddText("Set node between two points.");
                node node3 = new node
                {
                    no = NODE_3_NO,
                    type = node_type.TYPE_BETWEEN_TWO_POINTS,
                    typeSpecified = true,
                    between_two_points_start_point_coordinates = new vector_3d().SetCoordinates(1.0, -3.0, 0.0),
                    between_two_points_end_point_coordinates = new vector_3d().SetCoordinates(5.0, -3.0, -1.0)
                };
                SoapModelClient.set_node(node3);

                DataLogger.SetProgressBarValue(30);

                // node between two nodes
                DataLogger.AddText("Set node between two nodes.");
                node node4 = new node
                {
                    no = NODE_4_NO,
                    type = node_type.TYPE_BETWEEN_TWO_NODES,
                    typeSpecified = true,
                    between_two_nodes_start_node = NODE_1_NO,
                    between_two_nodes_start_nodeSpecified = true,
                    between_two_nodes_end_node = NODE_2_NO,
                    between_two_nodes_end_nodeSpecified = true
                };
                SoapModelClient.set_node(node4);

                DataLogger.SetProgressBarValue(40);

#if RFEM
                // node on line
                DataLogger.AddText("Set node on line.");
                line line = new line
                {
                    no = LINE_NO,
                    type = line_type.TYPE_POLYLINE,
                    typeSpecified = true,
                    definition_nodes = new int[] { NODE_1_NO, NODE_2_NO }
                };
                SoapModelClient.set_line(line);

                DataLogger.SetProgressBarValue(50);

                node node5 = new node
                {
                    no = NODE_5_NO,
                    type = node_type.TYPE_ON_LINE,
                    typeSpecified = true,
                    on_line_reference_line = LINE_NO,
                    on_line_reference_lineSpecified = true,
                    reference_type = node_reference_type.REFERENCE_TYPE_XY,
                    reference_typeSpecified = true,
                    distance_from_start_relative = 0.3,
                    distance_from_start_relativeSpecified = true
                };
                SoapModelClient.set_node(node5);
#endif

                DataLogger.SetProgressBarValue(60);

                // node on member
                DataLogger.AddText("Set node on member.");
                material material = new material
                {
                    no = MATERIAL_NO,
                    name = MATERIAL_STEEL
                };
                SoapModelClient.set_material(material);

                DataLogger.SetProgressBarValue(70);

                section section = new section
                {
                    no = SECTION_NO,
                    name = SECTION_I,
                    material = MATERIAL_NO,
                    materialSpecified = true
                };
                SoapModelClient.set_section(section);

                DataLogger.SetProgressBarValue(80);

                member member = new member
                {
                    no = MEMBER_NO,
                    section_start = SECTION_NO,
                    section_startSpecified = true,
#if RFEM
                    line = LINE_NO,
                    lineSpecified = true
#elif RSTAB
                    node_start = NODE_1_NO,
                    node_startSpecified = true,
                    node_end = NODE_2_NO,
                    node_endSpecified = true
#endif
                };
                SoapModelClient.set_member(member);

                DataLogger.SetProgressBarValue(90);

                node node6 = new node
                {
                    no = NODE_6_NO,
                    type = node_type.TYPE_ON_MEMBER,
                    typeSpecified = true,
                    on_member_reference_member = MEMBER_NO,
                    on_member_reference_memberSpecified = true,
                    reference_type = node_reference_type.REFERENCE_TYPE_XZ,
                    reference_typeSpecified = true,
                    distance_from_start_relative = 0.4,
                    distance_from_start_relativeSpecified = true
                };
                SoapModelClient.set_node(node6);

                DataLogger.SetProgressBarValue(100);
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

#if RFEM

        public static bool Test_Basic\u2040Objects_Nodes_Create\u20401000\u2040Nodes()
        {
            DataLogger.AddLogStart("Creating nodes and lines...");

            bool result = Test_General_Delete⁀All();
            if (!result || (SoapModelClient == null))
            {
                DataLogger.AddLogEnd(DataLogger.LogResultType.FAILED);
                return false;
            }

            DataLogger.InitializeProgressBar(0, 1125, 0);

            try
            {
                SoapModelClient.begin_modification("set nodes");

                int nodeNo = 1;
                int lineNo = 1;

                DataLogger.AddText("Set objects...");

                void WriteObjects(int count)
                {
                    for (int z = 0; z < count; z++)
                    {
                        for (int y = 0; y < count; y++)
                        {
                            for (int x = 0; x < count; x++)
                            {
                                node node = new node
                                {
                                    no = nodeNo++,
                                    coordinates = new vector_3d().SetCoordinates(x, y, z)
                                };
                                SoapModelClient.set_node(node);

                                if (nodeNo > 1)
                                {
                                    line line = new line
                                    {
                                        no = lineNo++,
                                        definition_nodes = new int[] { nodeNo - 1, nodeNo }
                                    };
                                    SoapModelClient.set_line(line);
                                }
                            }

                            DataLogger.SetProgressBarValue(nodeNo);
                        }
                    }
                }

                // writes nodes and lines
                WriteObjects(10);

                // writes duplicit nodes and lines
                WriteObjects(5);

                DataLogger.AddText("...done");
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
                    DataLogger.AddText("Finishing modification...");
                    SoapModelClient.finish_modification();
                    DataLogger.AddText("...done");
                }
                catch (Exception exception)
                {
                    ModelWsExceptionHandler(exception);
                    SoapModelClient.reset();
                }

                DataLogger.ResetProgressBar();
            }

            try
            {
                DataLogger.AddText($"Nodes = {SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_NODE, 0)}");
                DataLogger.AddText($"Lines = {SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_LINE, 0)}");

                DataLogger.AddText("Uniting nodes and supports...");
                SoapModelClient.unite_nodes_and_supports(0.0005);
                DataLogger.AddText("...done");

                DataLogger.AddText($"Nodes = {SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_NODE, 0)}");
                DataLogger.AddText($"Lines = {SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_LINE, 0)}");
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }

        public static bool Test_Basic\u2040Objects_Nodes_Copy\u2040\u2040Nodes()
        {
            DataLogger.AddLogStart("Copying objects...");

            try
            {
                // initialize soap model client
                SoapModelClient.ToString();

                DataLogger.AddLogStart("Reading objects...");
                DataLogger.InitializeProgressBar(0, 30, 0);

                // reads objects from the model
                GetObject<node> getNode = SoapModelClient.get_node;
                GetObject<line> getLine = SoapModelClient.get_line;
                GetObject<surface> getSurface = SoapModelClient.get_surface;

                node[] nodes = GetAllObjects(ref getNode, object_types.E_OBJECT_TYPE_NODE);
                DataLogger.SetProgressBarValue(10);

                line[] lines = GetAllObjects(ref getLine, object_types.E_OBJECT_TYPE_LINE);
                DataLogger.SetProgressBarValue(20);

                surface[] surfaces = GetAllObjects(ref getSurface, object_types.E_OBJECT_TYPE_SURFACE);
                DataLogger.SetProgressBarValue(30);

                DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);

                // delete the model
                Test_General_Delete⁀All();

                DataLogger.AddLogStart("Writing objects...");
                DataLogger.InitializeProgressBar(0, 30, 0);

                // writes objects back to the model
                SoapModelClient.begin_modification("test");

                SetObject<node> setNode = SoapModelClient.set_node;
                SetObject<line> setLine = SoapModelClient.set_line;
                SetObject<surface> setSurface = SoapModelClient.set_surface;

                SetAllObjects(ref setNode, nodes);
                DataLogger.SetProgressBarValue(10);

                SetAllObjects(ref setLine, lines);
                DataLogger.SetProgressBarValue(20);

                SetAllObjects(ref setSurface, surfaces);
                DataLogger.SetProgressBarValue(30);

                DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
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
                    DataLogger.AddText("Finishing modification...");
                    SoapModelClient.finish_modification();
                    DataLogger.AddText("...done");
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

#endif // RFEM

        public static bool Test_Basic\u2040Objects_Nodes_Get\u2040Nodes()
        {
            DataLogger.AddLogStart("Reading node numbers...");
            DataLogger.InitializeProgressBar(0, 100, 50);

            try
            {
                int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_NODE, 0);
                DataLogger.AddText($"{numbers.Length} node ids have been read.");
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

        public static bool Test_Basic\u2040Objects_Nodes_Delete\u2040Nodes()
        {
            DataLogger.AddLogStart("Deleting all nodes...");

            try
            {
                SoapModelClient.begin_modification("delete nodes");

                DataLogger.AddText("Reading node ids.");
                int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_NODE, 0);

                DataLogger.InitializeProgressBar(0, 10 * numbers.Length + 20, 10);

                DataLogger.AddText("Deleting nodes.");
                int counter = 0;
                foreach (int id in numbers)
                {
                    try
                    {
                        DataLogger.SetProgressBarValue(10 + 10 * counter);
                        SoapModelClient.delete_object(object_types.E_OBJECT_TYPE_NODE, id, 0);
                        counter++;
                    }
                    catch (Exception exception)
                    {
                        ModelWsExceptionHandler(exception);
                        return false;
                    }
                }
                DataLogger.AddText($"{counter} nodes have been deleted.");

                if (SoapModelClient == null)
                {
                    return false;
                }

                SoapModelClient.finish_modification();

                int[] numbersLeft = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_NODE, 0);
                DataLogger.AddText($"{numbersLeft.Length} nodes left.");

                DataLogger.SetProgressBarValue(10 * numbers.Length + 20);

                if ((numbers.Length - counter) > 0)
                {
                    DataLogger.AddLogEnd(DataLogger.LogResultType.FAILED);
                    return false;
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

        public static bool Test_Basic\u2040Objects_Nodes_Unite\u2040Nodes\u2040And\u2040Supports()
        {
            DataLogger.AddLogStart("Uniting nodes and supports...");

            try
            {
                void listNodes()
                {
                    int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_NODE, 0);
                    DataLogger.AddText("Nodes: " + string.Join(",", numbers));
                };

                DataLogger.InitializeProgressBar(0, 30, 0);

                listNodes();

                DataLogger.SetProgressBarValue(10);

                SoapModelClient.begin_modification("Unite nodes and supports");
                SoapModelClient.unite_nodes_and_supports(0.0005);
                SoapModelClient.finish_modification();

                DataLogger.SetProgressBarValue(20);

                listNodes();

                DataLogger.SetProgressBarValue(30);
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
    }
}
