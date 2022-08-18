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
        /// <summary>
        /// Sets member with default parameters.
        /// </summary>
        /// <param name="memberNo">Member No.</param>
        /// <param name="nodeOffsetX">Offset for coordinate in axis x.</param>
        /// <param name="materialNo">Material No. If not set then default material number is used.</param>
        /// <param name="sectionNo">Section No. If not set then default section number is used.</param>
        /// <param name="coordinateSystemNo">Coordinate system No. If not sed then global coordinate system is used.</param>
        private static void SetDefaultMember(int memberNo, double nodeOffsetX = 0.0, int materialNo = 0, int sectionNo = 0, int coordinateSystemNo = 0)
        {
            const int MATERIAL_NO = 1;
            const int SECTION_NO = 2;
            int lineNo = memberNo;

            DataLogger.AddText($"Set member No.{memberNo}");

            // material
            material material = new material
            {
                no = materialNo > 0 ? materialNo : MATERIAL_NO,
                name = MATERIAL_STEEL,
            };
            SoapModelClient.set_material(material);

            // section
            section section = new section
            {
                no = sectionNo > 0 ? sectionNo : SECTION_NO,
                name = SECTION_I,
                material = material.no,
                materialSpecified = true
            };
            SoapModelClient.set_section(section);

            // coordinate system
            if (coordinateSystemNo > 0)
            {
                coordinate_system coordinateSystem = new coordinate_system
                {
                    no = coordinateSystemNo,
                    type = coordinate_system_type.TYPE_3_POINTS,
                    typeSpecified = true,
                    u_axis_point_coordinates = new vector_3d().SetCoordinates(1.0, 0.0, 1.0),
                    uw_plane_point_coordinates = new vector_3d().SetCoordinates(0.0, 0.0, 1.0)
                };
                SoapModelClient.set_coordinate_system(coordinateSystem);
            }

            // nodes
            List<node> nodes = new List<node>
            {
                new node { no = (memberNo - 1) * 2 + 1, coordinates = new vector_3d().SetCoordinates(nodeOffsetX, 1.0, 0.0) },
                new node { no = (memberNo - 1) * 2 + 2, coordinates = new vector_3d().SetCoordinates(nodeOffsetX, 4.0, 0.0) }
            };
            nodes.ForEach(node => SoapModelClient.set_node(node));

#if RFEM
            // lines
            line line = new line
            {
                no = lineNo,
                definition_nodes = new int[] { nodes[0].no, nodes[1].no },
            };
            SoapModelClient.set_line(line);
#endif

            // member
            member member = new member
            {
                no = memberNo,
#if RFEM
                line = line.no,
                lineSpecified = true,
#elif RSTAB
                nodes = new int[] { nodes[0].no, nodes[1].no },
#endif
                section_start = section.no,
                section_startSpecified = true
            };
            SoapModelClient.set_member(member);
        }

        public static bool Test_Basic\u2040Objects_Members_Create\u2040Member()
        {
            const int MATERIAL_ID = 1;
            const int SECTION_ID = 2;
            const int NODE_1_ID = 3;
            const int NODE_2_ID = 4;
#if RFEM
            const int LINE_ID = 5;
#endif
            const int MEMBER_ID = 6;

            DataLogger.AddLogStart("Adding member...");

            if (!InitializeTest())
            {
                return false;
            }

            DataLogger.InitializeProgressBar(0, 50, 0);

            try
            {
                SoapModelClient.begin_modification("Create member");

                // create material
                DataLogger.AddText($"Creating material (ID = {MATERIAL_ID}, Name = {MATERIAL_STEEL}).");
                material material = new material
                {
                    no = MATERIAL_ID,
                    name = MATERIAL_STEEL
                };
                SoapModelClient.set_material(material);

                DataLogger.SetProgressBarValue(10);

                // create section
                DataLogger.AddText($"Creating section (ID = {SECTION_ID}, Name = {SECTION_I}).");
                section section = new section
                {
                    no = SECTION_ID,
                    name = SECTION_I,
                    material = MATERIAL_ID,
                    materialSpecified = true
                };
                SoapModelClient.set_section(section);

                DataLogger.SetProgressBarValue(20);

                // create nodes
                DataLogger.AddText($"Creating 2 nodes (IDs = [{NODE_1_ID}, {NODE_2_ID}]).");
                node node1 = new node
                {
                    no = NODE_1_ID,
                    coordinates = new vector_3d().SetCoordinates(10.0, 0.0, 0.0)
                };
                SoapModelClient.set_node(node1);

                node node2 = new node
                {
                    no = NODE_2_ID,
                    coordinates = new vector_3d().SetCoordinates(12.0, 0.0, 0.0)
                };
                SoapModelClient.set_node(node2);

                DataLogger.SetProgressBarValue(30);

#if RFEM
                // create line
                DataLogger.AddText($"Creating line (ID = {LINE_ID}).");
                line line = new line
                {
                    no = LINE_ID,
                    definition_nodes = new int[] { NODE_1_ID, NODE_2_ID }
                };
                SoapModelClient.set_line(line);

                DataLogger.SetProgressBarValue(40);

#endif

                // create member
                DataLogger.AddText($"Creating member (IDs = {MEMBER_ID}).");
                member member = new member
                {
                    no = MEMBER_ID,
#if RFEM
                    line = LINE_ID,
                    lineSpecified = true,
#elif RSTAB
                    nodes = new int[] { NODE_1_ID, NODE_2_ID },
#endif
                    section_start = SECTION_ID,
                    section_startSpecified = true
                };
                SoapModelClient.set_member(member);

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

        public static bool Test_Basic\u2040Objects_Members_Delete\u2040All\u2040Members\u2040One\u2040by\u2040One()
        {
            if (SoapModelClient == null)
            {
                DataLogger.AddLogEnd(DataLogger.LogResultType.FAILED);
                return false;
            }

            DataLogger.AddLogStart("Deleting all members one by one...");

            try
            {
                SoapModelClient.begin_modification("Deleting all members");

                int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_MEMBER, 0);

                DataLogger.InitializeProgressBar(0, numbers.Length, 0);

                for (int i = 0; i < numbers.Length; i++)
                {
                    SoapModelClient.delete_object(object_types.E_OBJECT_TYPE_MEMBER, numbers[i], 0);

                    DataLogger.SetProgressBarValue(i + 1);
                }
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

        public static bool Test_Basic\u2040Objects_Members_Delete\u2040All\u2040Members\u2040at\u2040Once()
        {
            if (SoapModelClient == null)
            {
                DataLogger.AddLogEnd(DataLogger.LogResultType.FAILED);
                return false;
            }

            DataLogger.AddLogStart("Deleting all members at once...");
            DataLogger.InitializeProgressBar(0, 10, 0);

            try
            {
                SoapModelClient.begin_modification("Deleting all members");

                // 0 means all objects of specified type
                SoapModelClient.delete_object(object_types.E_OBJECT_TYPE_MEMBER, 0, 0);

                DataLogger.SetProgressBarValue(10);
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
