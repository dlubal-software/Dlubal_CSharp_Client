#if RFEM

using Dlubal.WS.RfemRstabSoapWsCoreLib.Tools;
using Dlubal.WS.Rfem6.Model;
using System;
using System.Collections.Generic;

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
        public static bool Test_Types\u2040for\u2040Lines_Line\u2040Welded\u2040Joints_Create()
        {
            const int MATERIAL_NO = 1;

            const int NODE_1_NO = 2;
            const int NODE_2_NO = 3;
            const int NODE_3_NO = 4;
            const int NODE_4_NO = 5;
            const int NODE_5_NO = 6;
            const int NODE_6_NO = 7;
            const int NODE_7_NO = 8;
            const int NODE_8_NO = 9;

            const int LINE_1_NO = 10;
            const int LINE_2_NO = 11;
            const int LINE_3_NO = 12;
            const int LINE_4_NO = 13;
            const int LINE_5_NO = 14;
            const int LINE_6_NO = 15;
            const int LINE_7_NO = 16;
            const int LINE_8_NO = 17;
            const int LINE_9_NO = 18;
            const int LINE_10_NO = 19;

            const int THICKNESS_NO = 20;

            const int SURFACE_1_NO = 21;
            const int SURFACE_2_NO = 22;
            const int SURFACE_3_NO = 23;

            const int LINE_WELDED_JOINT_NO = 24;

            DataLogger.AddLogStart("Creating line welded joint...");
            DataLogger.InitializeProgressBar(0, 80, 0);

            bool result = Test_General_Delete⁀All();
            if (!result || (SoapModelClient == null))
            {
                DataLogger.AddLogEnd(DataLogger.LogResultType.FAILED);
                return false;
            }

            DataLogger.SetProgressBarValue(10);

            try
            {
                SoapModelClient.begin_modification("set line welded joints");

                // material
                DataLogger.AddText($"Set a material No.{MATERIAL_NO}");
                material material = new material
                {
                    no = MATERIAL_NO,
                    name = MATERIAL_STEEL
                };
                SoapModelClient.set_material(material);

                DataLogger.SetProgressBarValue(20);

                // nodes
                DataLogger.AddText("Set 4 standard nodes...");
                List<node> nodes = new List<node>
                {
                    new node { no = NODE_1_NO, coordinates = new vector_3d().SetCoordinates(0.0, 0.0, 0.0) },
                    new node { no = NODE_2_NO, coordinates = new vector_3d().SetCoordinates(3.0, 0.0, 0.0) },
                    new node { no = NODE_3_NO, coordinates = new vector_3d().SetCoordinates(3.0, 4.0, 0.0) },
                    new node { no = NODE_4_NO, coordinates = new vector_3d().SetCoordinates(0.0, 4.0, 0.0) },
                    new node { no = NODE_5_NO, coordinates = new vector_3d().SetCoordinates(6.0, 0.0, 0.0) },
                    new node { no = NODE_6_NO, coordinates = new vector_3d().SetCoordinates(6.0, 4.0, 0.0) },
                    new node { no = NODE_7_NO, coordinates = new vector_3d().SetCoordinates(3.0, 0.0, -2.0) },
                    new node { no = NODE_8_NO, coordinates = new vector_3d().SetCoordinates(3.0, 4.0, -2.0) }
                };
                nodes.ForEach(node => SoapModelClient.set_node(node));

                DataLogger.SetProgressBarValue(30);

                // lines
                DataLogger.AddText("Set 4 lines...");
                List<line> lines = new List<line>
                {
                    new line { no = LINE_1_NO, definition_nodes = new int[] { NODE_1_NO, NODE_2_NO } },
                    new line { no = LINE_2_NO, definition_nodes = new int[] { NODE_2_NO, NODE_3_NO } },
                    new line { no = LINE_3_NO, definition_nodes = new int[] { NODE_3_NO, NODE_4_NO } },
                    new line { no = LINE_4_NO, definition_nodes = new int[] { NODE_4_NO, NODE_1_NO } },
                    new line { no = LINE_5_NO, definition_nodes = new int[] { NODE_2_NO, NODE_5_NO } },
                    new line { no = LINE_6_NO, definition_nodes = new int[] { NODE_5_NO, NODE_6_NO } },
                    new line { no = LINE_7_NO, definition_nodes = new int[] { NODE_6_NO, NODE_3_NO } },
                    new line { no = LINE_8_NO, definition_nodes = new int[] { NODE_2_NO, NODE_7_NO } },
                    new line { no = LINE_9_NO, definition_nodes = new int[] { NODE_7_NO, NODE_8_NO } },
                    new line { no = LINE_10_NO, definition_nodes = new int[] { NODE_8_NO, NODE_3_NO } }
                };
                lines.ForEach(line => SoapModelClient.set_line(line));

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

                // surfaces
                DataLogger.AddText("Set 3 surfaces...");
                List<surface> surfaces = new List<surface>
                {
                    new surface {no = SURFACE_1_NO, boundary_lines = new int[] { LINE_1_NO, LINE_2_NO, LINE_3_NO, LINE_4_NO }, thickness = THICKNESS_NO, thicknessSpecified = true },
                    new surface {no = SURFACE_2_NO, boundary_lines = new int[] { LINE_2_NO, LINE_5_NO, LINE_6_NO, LINE_7_NO }, thickness = THICKNESS_NO, thicknessSpecified = true },
                    new surface {no = SURFACE_3_NO, boundary_lines = new int[] { LINE_2_NO, LINE_8_NO, LINE_9_NO, LINE_10_NO }, thickness = THICKNESS_NO, thicknessSpecified = true }
                };
                surfaces.ForEach(surface => SoapModelClient.set_surface(surface));

                DataLogger.SetProgressBarValue(60);

                // line welded joint
                DataLogger.AddText($"Set line welded joint No.{LINE_WELDED_JOINT_NO}");
                line_welded_joint joint = new line_welded_joint()
                {
                    no = LINE_WELDED_JOINT_NO,
                    joint_type = line_welded_joint_joint_type.BUTT_JOINT,
                    joint_typeSpecified = true,
                    weld_size_a1 = 0.0041,
                    weld_size_a1Specified = true
                };
                SoapModelClient.set_line_welded_joint(joint);

                DataLogger.SetProgressBarValue(70);

                // assign line welded joint
                DataLogger.AddText("Assigning line welded joint to line.");
                line lineToAssign = SoapModelClient.get_line(LINE_2_NO);
                lineToAssign.line_weld_assignment = new line_line_weld_assignment[]
                {
                    new line_line_weld_assignment
                    {
                        weld = LINE_WELDED_JOINT_NO,
                        surface1 = SURFACE_1_NO,
                        surface2 = SURFACE_2_NO,
                        surface3 = SURFACE_3_NO
                    }
                };
                SoapModelClient.set_line(lineToAssign);

                DataLogger.SetProgressBarValue(80);
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

#endif // RFEM
