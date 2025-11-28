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
        public static bool Test_Types\u2040for\u2040Members_Member\u2040Result\u2040Intermediate\u2040Points_Create()
        {
            const int MATERIAL_ID = 1;
            const int SECTION_ID = 2;
            const int NODE_1_ID = 3;
            const int NODE_2_ID = 4;
            const int LINE_ID = 5;
            const int MEMBER_ID = 6;
            const int MEMBER_RESULT_INTERMEDIATE_POINT_ID = 7;

            DataLogger.AddLogStart("Adding member result intermediate point...");
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
                SoapModelClient.begin_modification("Create member result intermediate point");

                // material
                DataLogger.AddText($"Set material (IDs = {MATERIAL_ID}).");
                material material = new material
                {
                    no = MATERIAL_ID,
                    name = MATERIAL_STEEL
                };
                SoapModelClient.set_material(material);

                DataLogger.SetProgressBarValue(20);

                // cross_section
                DataLogger.AddText($"Set cross_section (IDs = {SECTION_ID}).");
                cross_section cross_section = new cross_section
                {
                    no = SECTION_ID,
                    name = SECTION_I,
                    material = MATERIAL_ID,
                    materialSpecified = true
                };
                SoapModelClient.set_cross_section(cross_section);

                DataLogger.SetProgressBarValue(30);

                // nodes
                DataLogger.AddText($"Set 2 nodes (IDs = [{NODE_1_ID}, {NODE_2_ID}]).");
                node node1 = new node
                {
                    no = NODE_1_ID,
                    coordinates = new vector_3d().SetCoordinates(1.0, 1.0, 0.0)
                };
                SoapModelClient.set_node(node1);

                node node2 = new node
                {
                    no = NODE_2_ID,
                    coordinates = new vector_3d().SetCoordinates(5.0, 1.0, -1.0)
                };
                SoapModelClient.set_node(node2);

                DataLogger.SetProgressBarValue(40);

#if RFEM
                // line
                DataLogger.AddText($"Set line (IDs = {LINE_ID}).");
                line line = new line
                {
                    no = LINE_ID,
                    definition_nodes = new int[] { NODE_1_ID, NODE_2_ID }
                };
                SoapModelClient.set_line(line);

                DataLogger.SetProgressBarValue(50);
#endif

                // member
                DataLogger.AddText($"Set member (IDs = {MEMBER_ID}).");
                member member = new member
                {
                    no = MEMBER_ID,
                    type = member_type.TYPE_BEAM,
                    typeSpecified = true,
#if RFEM
                    line = LINE_ID,
                    lineSpecified = true,
#elif RSTAB
                    nodes = new int[] { NODE_1_ID, NODE_2_ID },
#endif
                    cross_section_start = SECTION_ID,
                    section_startSpecified = true
                };
                SoapModelClient.set_member(member);

                DataLogger.SetProgressBarValue(60);

                // member result intermediate point
                DataLogger.AddText($"Set member result intermediate point (IDs = {MEMBER_RESULT_INTERMEDIATE_POINT_ID}).");
                member_result_intermediate_point point = new member_result_intermediate_point
                {
                    no = MEMBER_RESULT_INTERMEDIATE_POINT_ID,
                    uniform_distribution = false,
                    uniform_distributionSpecified = true,
                    distances = new member_result_intermediate_point_distances[]
                    {
                        new member_result_intermediate_point_distances().SetDistance(0.111, "note1"),
                        new member_result_intermediate_point_distances().SetDistance(0.222, "note2"),
                        new member_result_intermediate_point_distances().SetDistance(0.333, "note3"),
                        new member_result_intermediate_point_distances().SetDistance(0.444, "note4")
                    },
                    members = new int[] { MEMBER_ID }
                };
                SoapModelClient.set_member_result_intermediate_point(point);

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

        public static bool Test_Types\u2040for\u2040Members_Member\u2040Result\u2040Intermediate\u2040Points_Get()
        {
            DataLogger.AddLogStart("Reading member result intermediate point ids...");

            try
            {
                int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_MEMBER_RESULT_INTERMEDIATE_POINT, 0);
                DataLogger.AddText($"{numbers.Length} member result intermediate point ids have been read.");
                DataLogger.InitializeProgressBar(0, numbers.Length, 0);

                int progress = 0;
                foreach (int number in numbers)
                {
                    member_result_intermediate_point line = SoapModelClient.get_member_result_intermediate_point(number);

                    DataLogger.AddText($"Member result intermediate point No.{number}: division ordinate count = {line.distances.Length}");
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

        public static bool Test_Types\u2040for\u2040Members_Member\u2040Result\u2040Intermediate\u2040Points_Modify()
        {
            const int MATERIAL_ID = 1;
            const int SECTION_ID = 2;
            const int NODE_1_ID = 3;
            const int NODE_2_ID = 4;
            const int LINE_ID = 5;
            const int MEMBER_ID = 6;
            const int MEMBER_RESULT_INTERMEDIATE_POINT_ID = 7;

            DataLogger.AddLogStart("Adding member result intermediate point...");
            DataLogger.InitializeProgressBar(0, 90, 0);

            bool result = Test_General_Delete⁀All();

            if (!result || (SoapModelClient == null))
            {
                DataLogger.AddLogEnd(DataLogger.LogResultType.FAILED);
                return false;
            }

            DataLogger.SetProgressBarValue(10);

            try
            {
                SoapModelClient.begin_modification("Create member result intermediate point");

                // material
                DataLogger.AddText($"Set material (IDs = {MATERIAL_ID}).");
                material material = new material
                {
                    no = MATERIAL_ID,
                    name = MATERIAL_STEEL
                };
                SoapModelClient.set_material(material);

                DataLogger.SetProgressBarValue(20);

                // cross_section
                DataLogger.AddText($"Set cross_section (IDs = {SECTION_ID}).");
                cross_section cross_section = new cross_section
                {
                    no = SECTION_ID,
                    name = SECTION_I,
                    material = MATERIAL_ID,
                    materialSpecified = true
                };
                SoapModelClient.set_cross_section(cross_section);

                DataLogger.SetProgressBarValue(30);

                // nodes
                DataLogger.AddText($"Set 2 nodes (IDs = [{NODE_1_ID}, {NODE_2_ID}]).");
                node node1 = new node
                {
                    no = NODE_1_ID,
                    coordinates = new vector_3d().SetCoordinates(1.0, 1.0, 0.0)
                };
                SoapModelClient.set_node(node1);

                node node2 = new node
                {
                    no = NODE_2_ID,
                    coordinates = new vector_3d().SetCoordinates(5.0, 1.0, -1.0)
                };
                SoapModelClient.set_node(node2);

                DataLogger.SetProgressBarValue(40);

#if RFEM
                // line
                DataLogger.AddText($"Set line (IDs = {LINE_ID}).");
                line line = new line
                {
                    no = LINE_ID,
                    definition_nodes = new int[] { NODE_1_ID, NODE_2_ID }
                };
                SoapModelClient.set_line(line);

                DataLogger.SetProgressBarValue(50);
#endif

                // member
                DataLogger.AddText($"Set member (IDs = {MEMBER_ID}).");
                member member = new member
                {
                    no = MEMBER_ID,
                    type = member_type.TYPE_BEAM,
                    typeSpecified = true,
#if RFEM
                    line = LINE_ID,
                    lineSpecified = true,
#elif RSTAB
                    nodes = new int[] { NODE_1_ID, NODE_2_ID },
#endif
                    cross_section_start = SECTION_ID,
                    section_startSpecified = true
                };
                SoapModelClient.set_member(member);

                DataLogger.SetProgressBarValue(60);

                // member result intermediate point
                DataLogger.AddText($"Set member result intermediate point (IDs = {MEMBER_RESULT_INTERMEDIATE_POINT_ID}).");
                member_result_intermediate_point point = new member_result_intermediate_point
                {
                    no = MEMBER_RESULT_INTERMEDIATE_POINT_ID,
                    uniform_distribution = false,
                    uniform_distributionSpecified = true,
                    distances = new member_result_intermediate_point_distances[]
                    {
                        new member_result_intermediate_point_distances().SetDistance(0.111, "note1"),
                        new member_result_intermediate_point_distances().SetDistance(0.222, "note2"),
                        new member_result_intermediate_point_distances().SetDistance(0.333, "note3"),
                        new member_result_intermediate_point_distances().SetDistance(0.444, "note4")
                    },
                    members = new int[] { MEMBER_ID }
                };
                SoapModelClient.set_member_result_intermediate_point(point);

                SoapModelClient.finish_modification();

                DataLogger.SetProgressBarValue(70);

                // increase table
                SoapModelClient.begin_modification("Increase member result intermediate point table");

                DataLogger.AddText($"Increase member result intermediate point table (IDs = {MEMBER_RESULT_INTERMEDIATE_POINT_ID}).");
                point.distances = new member_result_intermediate_point_distances[]
                {
                    new member_result_intermediate_point_distances().SetDistance(0.111, "note1"),
                    new member_result_intermediate_point_distances().SetDistance(0.222, "note2"),
                    new member_result_intermediate_point_distances().SetDistance(0.333, "note3"),
                    new member_result_intermediate_point_distances().SetDistance(0.444, "note4"),
                    new member_result_intermediate_point_distances().SetDistance(0.555, "note5")
                };
                SoapModelClient.set_member_result_intermediate_point(point);

                SoapModelClient.finish_modification();

                if (SoapModelClient.get_member_result_intermediate_point(MEMBER_RESULT_INTERMEDIATE_POINT_ID).distances.Length != 5)
                {
                    DataLogger.AddText("Count of table rows does not match.");
                }

                DataLogger.SetProgressBarValue(80);

                // decrease table
                SoapModelClient.begin_modification("Decrease member result intermediate point table");

                DataLogger.AddText($"Decrease member result intermediate point table (IDs = {MEMBER_RESULT_INTERMEDIATE_POINT_ID}).");
                point.distances = new member_result_intermediate_point_distances[]
                {
                    new member_result_intermediate_point_distances().SetDistance(0.111, "note1"),
                    new member_result_intermediate_point_distances().SetDistance(0.222, "note2"),
                    new member_result_intermediate_point_distances().SetDistance(0.333, "note3")
                };
                SoapModelClient.set_member_result_intermediate_point(point);

                SoapModelClient.finish_modification();

                if (SoapModelClient.get_member_result_intermediate_point(MEMBER_RESULT_INTERMEDIATE_POINT_ID).distances.Length != 3)
                {
                    DataLogger.AddText("Count of table rows does not match.");
                }

                DataLogger.SetProgressBarValue(90);
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
