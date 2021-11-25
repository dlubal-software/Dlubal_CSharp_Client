#if RFEM

using Dlubal.WS.RfemRstabSoapWsCoreLib.Tools;
using Dlubal.WS.Rfem6.Model;
using System;
using System.Collections.Generic;

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
        private static void SetDefaultSurface(node[] nodes, int surfaceNo = 1)
        {
            const int MATERIAL_NO = 1;
            const int THICKNESS_NO = 1;

            List<int> surfaceBoundaryLines = new List<int>(nodes.Length);

            // nodes and lines
            for (int i = 0; i <= nodes.Length; i++)
            {
                if (i < nodes.Length)
                {
                    SoapModelClient.set_node(nodes[i]);
                }
                if (i == 0)
                {
                    continue;
                }

                line line = new line { no = nodes[i - 1].no, definition_nodes = new int[] { nodes[i - 1].no, nodes[i == nodes.Length ? 0 : i].no } };
                SoapModelClient.set_line(line);
                surfaceBoundaryLines.Add(line.no);
            }

            // material
            material material = new material
            {
                no = MATERIAL_NO,
                name = MATERIAL_STEEL,
            };
            SoapModelClient.set_material(material);

            // thickness
            thickness thickness = new thickness
            {
                no = THICKNESS_NO,
                material = material.no,
                materialSpecified = true
            };
            SoapModelClient.set_thickness(thickness);

            // surface
            surface surface = new surface
            {
                no = surfaceNo,
                boundary_lines = surfaceBoundaryLines.ToArray(),
                thickness = thickness.no,
                thicknessSpecified = true
            };
            SoapModelClient.set_surface(surface);
        }

        private static bool SetNurbsSurface(bool userCS)
        {
            const int NODE_1_NO = 1;
            const int NODE_2_NO = 2;
            const int NODE_3_NO = 3;
            const int NODE_4_NO = 4;
            const int LINE_1_NO = 5;
            const int LINE_2_NO = 6;
            const int LINE_3_NO = 7;
            const int LINE_4_NO = 8;
            const int MATERIAL_NO = 9;
            const int THICKNESS_NO = 10;
            const int SURFACE_NO = 11;
            int COORDINATE_SYSTEM_ID = userCS ? 12 : 0;
            const int NURBS_ORDER = 3;

            DataLogger.AddLogStart("Creating NURBS surface...");
            DataLogger.InitializeProgressBar(0, 60, 0);

            bool result = Test_General_Delete⁀All();
            if (!result || (SoapModelClient == null))
            {
                DataLogger.AddLogEnd(DataLogger.LogResultType.FAILED);
                return false;
            }

            DataLogger.SetProgressBarValue(5);

            try
            {
                SoapModelClient.begin_modification("set NURBS surface");

                // coordinate system
                if (userCS)
                {
                    DataLogger.AddText($"Set coordinate system No.{COORDINATE_SYSTEM_ID}");

                    coordinate_system coordinateSystem = new coordinate_system
                    {
                        no = COORDINATE_SYSTEM_ID,
                        type = coordinate_system_type.TYPE_2_POINTS_AND_ANGLE,
                        typeSpecified = true,
                        origin_coordinates = new vector_3d().SetCoordinates(0.0, 0.0, 0.0),
                        u_axis_point_coordinates = new vector_3d().SetCoordinates(1.0, 1.0, 0.0),
                        uw_plane_angle = DegToRad(45),
                        uw_plane_angleSpecified = true
                    };
                    SoapModelClient.set_coordinate_system(coordinateSystem);

                    DataLogger.SetProgressBarValue(10);
                }

                DataLogger.SetProgressBarValue(10);

                // nodes
                DataLogger.AddText("Set 4 standard nodes");
                List<node> nodes = new List<node>
                {
                    new node { no = NODE_1_NO, coordinates = new vector_3d().SetCoordinates(0.0, -1.0, -1.0), coordinate_system = COORDINATE_SYSTEM_ID, coordinate_systemSpecified = true },
                    new node { no = NODE_2_NO, coordinates = new vector_3d().SetCoordinates(0.0, -10.0, -1.0), coordinate_system = COORDINATE_SYSTEM_ID, coordinate_systemSpecified = true },
                    new node { no = NODE_3_NO, coordinates = new vector_3d().SetCoordinates(2.0, -1.0, -1.0), coordinate_system = COORDINATE_SYSTEM_ID, coordinate_systemSpecified = true },
                    new node { no = NODE_4_NO, coordinates = new vector_3d().SetCoordinates(2.0, -10.0, -1.0), coordinate_system = COORDINATE_SYSTEM_ID, coordinate_systemSpecified = true }
                };
                nodes.ForEach(node => SoapModelClient.set_node(node));

                DataLogger.SetProgressBarValue(20);

                // lines
                DataLogger.AddText("Set NURBS lines");
                List<line> lines = new List<line>
                {
                    new line
                    {
                        no = LINE_1_NO,
                        type = line_type.TYPE_NURBS,
                        typeSpecified = true,
                        definition_nodes = new int[] { NODE_1_NO, NODE_2_NO },
                        nurbs_control_points = new line_nurbs_control_points[]
                        {
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(0.000, -1.000, -1.000, 1.000),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(0.000, -3.000, -2.000, 1.000),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(0.000, -5.000, -1.000, 1.000),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(0.000, -6.000, -3.000, 1.000),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(0.000, -10.000, -1.000, 1.000),
                        },
                        nurbs_order = NURBS_ORDER,
                        nurbs_orderSpecified = true,
                        nurbs_knots = new double[] { 0.0, 0.0, 0.0, 0.333333, 0.666666, 1.0, 1.0, 1.0 }
                    },
                    new line
                    {
                        no = LINE_2_NO,
                        type = line_type.TYPE_NURBS,
                        typeSpecified = true,
                        definition_nodes = new int[] { NODE_3_NO, NODE_4_NO },
                        nurbs_control_points = new line_nurbs_control_points[]
                        {
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(2.000, -1.000, -1.000, 1.000),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(2.000, -3.000, -2.000, 1.000),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(2.000, -5.000, -1.000, 1.000),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(2.000, -6.000, -3.000, 1.000),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(2.000, -10.000, -1.000, 1.000),
                        },
                        nurbs_order = NURBS_ORDER,
                        nurbs_orderSpecified = true,
                        nurbs_knots = new double[] { 0.0, 0.0, 0.0, 0.333333, 0.666666, 1.0, 1.0, 1.0 }
                    },
                    new line
                    {
                        no = LINE_3_NO,
                        type = line_type.TYPE_NURBS,
                        typeSpecified = true,
                        definition_nodes = new int[] { NODE_1_NO, NODE_3_NO },
                        nurbs_control_points = new line_nurbs_control_points[]
                        {
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(0.000, -1.000, -1.000, 1.000),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(1.000, -1.000, -2.000, 1.000),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(2.000, -1.000, -1.000, 1.000),
                        },
                        nurbs_order = NURBS_ORDER,
                        nurbs_orderSpecified = true,
                        nurbs_knots = new double[] { 0.0, 0.0, 0.0, 1.0, 1.0, 1.0 }
                    },
                    new line
                    {
                        no = LINE_4_NO,
                        type = line_type.TYPE_NURBS,
                        typeSpecified = true,
                        definition_nodes = new int[] { NODE_2_NO, NODE_4_NO },
                        nurbs_control_points = new line_nurbs_control_points[]
                        {
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(0.000, -10.000, -1.000, 1.000),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(1.000, -10.000, -2.000, 1.000),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(2.000, -10.000, -1.000, 1.000),
                        },
                        nurbs_order = NURBS_ORDER,
                        nurbs_orderSpecified = true,
                        nurbs_knots = new double[] { 0.0, 0.0, 0.0, 1.0, 1.0, 1.0 }
                    }
                };
                lines.ForEach(line => SoapModelClient.set_line(line));

                DataLogger.SetProgressBarValue(30);

                // material
                DataLogger.AddText($"Set material No.{MATERIAL_NO}");
                material material = new material
                {
                    no = MATERIAL_NO,
                    name = MATERIAL_STEEL
                };
                SoapModelClient.set_material(material);

                DataLogger.SetProgressBarValue(40);

                // thickness
                DataLogger.AddText($"Set thickness No.{THICKNESS_NO}");
                thickness thickness = new thickness
                {
                    no = THICKNESS_NO,
                    material = MATERIAL_NO,
                    materialSpecified = true
                };
                SoapModelClient.set_thickness(thickness);

                DataLogger.SetProgressBarValue(50);

                // create surface
                DataLogger.AddText($"Set NURBS surface No.{SURFACE_NO}");
                surface surface = new surface
                {
                    no = SURFACE_NO,
                    geometry = surface_geometry.GEOMETRY_NURBS,
                    geometrySpecified = true,
                    boundary_lines = new int[] { LINE_1_NO, LINE_2_NO, LINE_3_NO, LINE_4_NO },
                    thickness = THICKNESS_NO,
                    thicknessSpecified = true,
                    nurbs_order_in_direction_u = NURBS_ORDER,
                    nurbs_order_in_direction_uSpecified = true,
                    nurbs_order_in_direction_v = NURBS_ORDER,
                    nurbs_order_in_direction_vSpecified = true,
                    nurbs_control_points = new surface_nurbs_control_points[][]
                    {
                        new surface_nurbs_control_points[]
                        {
                            new surface_nurbs_control_points().SetNurbsLocalControlPoint(0.000, -10.000, -1.000, 1.11),
                            new surface_nurbs_control_points().SetNurbsLocalControlPoint(0.000, -10.000, -2.000, 1.12),
                            new surface_nurbs_control_points().SetNurbsLocalControlPoint(2.000, -10.000, -1.000, 1.13)
                        },
                        new surface_nurbs_control_points[]
                        {
                            new surface_nurbs_control_points().SetNurbsLocalControlPoint(0.000, -6.000, -3.000, 1.21),
                            new surface_nurbs_control_points().SetNurbsLocalControlPoint(1.000, -5.911, -3.894, 1.22),
                            new surface_nurbs_control_points().SetNurbsLocalControlPoint(2.000, -6.000, -3.000, 1.23)
                        },
                        new surface_nurbs_control_points[]
                        {
                            new surface_nurbs_control_points().SetNurbsLocalControlPoint(0.000, -5.000, -1.000, 1.31),
                            new surface_nurbs_control_points().SetNurbsLocalControlPoint(1.000, -4.802, -1.877, 1.32),
                            new surface_nurbs_control_points().SetNurbsLocalControlPoint(2.000, -5.000, -1.000, 1.33)
                        },
                        new surface_nurbs_control_points[]
                        {
                            new surface_nurbs_control_points().SetNurbsLocalControlPoint(0.000, -3.000, -2.000, 1.41),
                            new surface_nurbs_control_points().SetNurbsLocalControlPoint(1.000, -3.268, -2.894, 1.42),
                            new surface_nurbs_control_points().SetNurbsLocalControlPoint(2.000, -3.000, -2.000, 1.43)
                        },
                        new surface_nurbs_control_points[]
                        {
                            new surface_nurbs_control_points().SetNurbsLocalControlPoint(0.000, -1.000, -1.000, 1.51),
                            new surface_nurbs_control_points().SetNurbsLocalControlPoint(1.000, -1.000, -2.000, 1.52),
                            new surface_nurbs_control_points().SetNurbsLocalControlPoint(2.000, -1.000, -1.000, 1.53)
                        }
                    }
                };
                SoapModelClient.set_surface(surface);

                DataLogger.SetProgressBarValue(60);
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

        public static bool Test_Basic\u2040Objects_Surfaces_Create\u2040Surface()
        {
            const int NODE_1_NO = 1;
            const int NODE_2_NO = 2;
            const int NODE_3_NO = 3;
            const int NODE_4_NO = 4;
            const int LINE_1_NO = 5;
            const int LINE_2_NO = 6;
            const int LINE_3_NO = 7;
            const int LINE_4_NO = 8;
            const int MATERIAL_NO = 9;
            const int THICKNESS_NO = 10;
            const int SURFACE_NO = 11;

            DataLogger.AddLogStart("Creating surface...");
            DataLogger.InitializeProgressBar(0, 60, 0);

            bool result = Test_General_Delete⁀All();
            if (!result || (SoapModelClient == null))
            {
                DataLogger.AddLogEnd(DataLogger.LogResultType.FAILED);
                return false;
            }

            DataLogger.SetProgressBarValue(10);

            try
            {
                SoapModelClient.begin_modification("set surface");

                // nodes
                DataLogger.AddText("Set 4 standard nodes");
                List<node> nodes = new List<node>
                {
                    new node { no = NODE_1_NO, coordinates = new vector_3d().SetCoordinates(1.0, 1.0, 0.0) },
                    new node { no = NODE_2_NO, coordinates = new vector_3d().SetCoordinates(5.0, 1.0, 0.0) },
                    new node { no = NODE_3_NO, coordinates = new vector_3d().SetCoordinates(5.0, 5.0, 0.0) },
                    new node { no = NODE_4_NO, coordinates = new vector_3d().SetCoordinates(1.0, 5.0, 0.0) }
                };
                nodes.ForEach(node => SoapModelClient.set_node(node));

                DataLogger.SetProgressBarValue(20);

                // lines
                DataLogger.AddText("Set 4 lines");
                List<line> lines = new List<line>
                {
                    new line { no = LINE_1_NO, definition_nodes = new int[] { NODE_1_NO, NODE_2_NO } },
                    new line { no = LINE_2_NO, definition_nodes = new int[] { NODE_2_NO, NODE_3_NO } },
                    new line { no = LINE_3_NO, definition_nodes = new int[] { NODE_3_NO, NODE_4_NO } },
                    new line { no = LINE_4_NO, definition_nodes = new int[] { NODE_4_NO, NODE_1_NO } }
                };
                lines.ForEach(line => SoapModelClient.set_line(line));

                DataLogger.SetProgressBarValue(30);

                // material
                DataLogger.AddText($"Set material No.{MATERIAL_NO}");
                material material = new material
                {
                    no = MATERIAL_NO,
                    name = MATERIAL_STEEL
                };
                SoapModelClient.set_material(material);

                DataLogger.SetProgressBarValue(40);

                // thickness
                DataLogger.AddText($"Set thickness No.{THICKNESS_NO}");
                thickness thickness = new thickness
                {
                    no = THICKNESS_NO,
                    material = MATERIAL_NO,
                    materialSpecified = true,
                    type = thickness_type.TYPE_UNIFORM,
                    typeSpecified = true
                };
                SoapModelClient.set_thickness(thickness);

                DataLogger.SetProgressBarValue(50);

                // surface
                DataLogger.AddText($"Set surface No.{SURFACE_NO}");
                surface surface = new surface
                {
                    no = SURFACE_NO,
                    boundary_lines = new int[] { LINE_1_NO, LINE_2_NO, LINE_3_NO, LINE_4_NO },
                    thickness = THICKNESS_NO,
                    thicknessSpecified = true
                };
                SoapModelClient.set_surface(surface);

                DataLogger.SetProgressBarValue(60);
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

        public static bool Test_Basic\u2040Objects_Surfaces_Create\u2040NURBS\u2040Surface\u2040Global\u2040CS()
        {
            return SetNurbsSurface(false);
        }

        public static bool Test_Basic\u2040Objects_Surfaces_Create\u2040NURBS\u2040Surface\u2040User\u2040CS()
        {
            return SetNurbsSurface(true);
        }

        public static bool Test_Basic\u2040Objects_Surfaces_Get\u2040Surfaces()
        {
            DataLogger.AddLogStart("Reading surface ids...");

            try
            {
                int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_SURFACE, 0);
                DataLogger.AddText($"{numbers.Length} surface ids have been read.");
                DataLogger.InitializeProgressBar(0, numbers.Length, 0);

                int progress = 0;
                foreach (int number in numbers)
                {
                    surface surface = SoapModelClient.get_surface(number);
                    DataLogger.AddText($"Surface No.{number}: {surface.geometry}, {surface.type}, boundary lines = {surface.boundary_lines_string}, material = {surface.material}");
                    if (surface.geometry == surface_geometry.GEOMETRY_NURBS)
                    {
                        DataLogger.AddText($"        count u = {surface.nurbs_control_point_count_in_direction_u}; order u = {surface.nurbs_order_in_direction_u}; count v = {surface.nurbs_control_point_count_in_direction_v}; order v = {surface.nurbs_order_in_direction_v}");

                        for (int u = 0; u < surface.nurbs_control_point_count_in_direction_u; u++)
                        {
                            for (int v = 0; v < surface.nurbs_control_point_count_in_direction_v; v++)
                            {
                                surface_nurbs_control_points cp = surface.nurbs_control_points[u][v];
                                DataLogger.AddText($"        u{cp.u}, v{cp.v}; weight = {cp.weight}; global coordinates = {cp.global_coordinates.ToStringEx()}; coordinates = {cp.coordinates.ToStringEx()}");
                            }
                        }
                    }
                    DataLogger.SetProgressBarValue(++progress);
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

        public static bool Test_Basic\u2040Objects_Surfaces_Delete\u2040Surfaces()
        {
            DataLogger.AddLogStart("Deleting all surfaces...");

            try
            {
                SoapModelClient.begin_modification("delete surfaces");

                DataLogger.AddText("Reading surface ids.");
                int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_SURFACE, 0);

                DataLogger.InitializeProgressBar(0, 10 * numbers.Length + 20, 10);

                DataLogger.AddText("Deleting surfaces.");
                int counter = 0;
                foreach (int id in numbers)
                {
                    try
                    {
                        DataLogger.SetProgressBarValue(10 + 10 * counter);
                        SoapModelClient.delete_object(object_types.E_OBJECT_TYPE_SURFACE, id, 0);
                        counter++;
                    }
                    catch (Exception exception)
                    {
                        ModelWsExceptionHandler(exception);
                        return false;
                    }
                }
                DataLogger.AddText($"{counter} surfaces have been deleted.");

                if (SoapModelClient == null)
                {
                    return false;
                }

                SoapModelClient.finish_modification();

                int[] numbersLeft = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_SURFACE, 0);
                DataLogger.AddText($"{numbersLeft.Length} surfaces left.");

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
    }
}

#endif // RFEM
