using Dlubal.WS.RfemRstabSoapWsCoreLib.Tools;
using System;
using Dlubal.WS.RfemRstabSoapWsCoreLib;
using System.ServiceModel;

#if RFEM
using Dlubal.WS.Rfem6.Application;
using ApplicationClient = Dlubal.WS.Rfem6.Application.RfemApplicationClient;
using Dlubal.WS.Rfem6.Model;
#elif RSTAB
using Dlubal.WS.Rstab9.Application;
using ApplicationClient = Dlubal.WS.Rstab9.Application.RstabApplicationClient;
using Dlubal.WS.Rstab9.Model
#endif

namespace DotNetCoreClientTest.Web2.Code.WSconsumer
{
    public class RunScenario
    {
        public bool RunScenario1(Models.ResultTestModel model)
        {

            Session.Address = new EndpointAddress(MyConfig.DefaultAddress);
            Session.SoapApplicationClient = new ApplicationClient(Session.Binding, Session.Address);
            application_information info = Session.SoapApplicationClient.get_information();

            model.Name = "Application information";
            model.Messages.Add(info.language_name);
            model.Messages.Add(info.type.ToString());
            model.Messages.Add(info.version);

            return true;
        }
        public bool RunScenario2(Models.ResultTestModel model)
        {

            Session.Address = new EndpointAddress(MyConfig.DefaultAddress);
            Session.SoapApplicationClient = new ApplicationClient(Session.Binding, Session.Address);

            //try
            //{
            Session.SoapModelClient.begin_modification("set nodes");

            int nodeNo = 1;
            int lineNo = 1;

            int count = 5;

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
                        Session.SoapModelClient.set_node(node);

                        if (nodeNo > 1)
                        {
                            line line = new line
                            {
                                no = lineNo++,
                                definition_nodes = new int[] { nodeNo - 1, nodeNo }
                            };
                            Session.SoapModelClient.set_line(line);
                        }
                    }
                }
            }
            //}
            //catch (Exception exception)
            //{
            //    ModelWsExceptionHandler(exception);
            //    return false;
            //}
            //finally
            //{
            //    try
            //    {
            //        DataLogger.AddText("Finishing modification...");
            Session.SoapModelClient.finish_modification();
            //        DataLogger.AddText("...done");
            //    }
            //    catch (Exception exception)
            //    {
            //        ModelWsExceptionHandler(exception);
            //        Session.SoapModelClient.reset();
            //    }

            //    DataLogger.ResetProgressBar();
            //}


            model.Name = "1000 points";
            //model.Messages.Add("set nodes");

            model.Messages.Add($"Nodes = {Session.SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_NODE, 0)}");
            model.Messages.Add($"Lines = {Session.SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_LINE, 0)}");

            model.Messages.Add("Uniting nodes and supports...");
            Session.SoapModelClient.unite_nodes_and_supports(0.0005);


            model.Messages.Add($"Nodes = {Session.SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_NODE, 0)}");
            model.Messages.Add($"Lines = {Session.SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_LINE, 0)}");



            return true;
        }
        public bool RunScenario3(Models.ResultTestModel model)
        {
            Session.Address = new EndpointAddress(MyConfig.DefaultAddress);
            Session.SoapApplicationClient = new ApplicationClient(Session.Binding, Session.Address);

            const int MATERIAL_ID = 1;
            const int SECTION_ID = 2;
            const int NODE_1_ID = 3;
            const int NODE_2_ID = 4;
            const int LINE_ID = 5;
            const int MEMBER_ID = 6;
            const int MEMBER_RESULT_INTERMEDIATE_POINT_ID = 7;

            const string MATERIAL_STEEL = "S235"; // (EN 10025-2:2004-11) - zatim bez pouziti normy, aplikace s tim ma problemy
            const string SECTION_I = "IPE 100";
            const string SECTION_RECTANGLE = "R_M1 200/300"; // Rectangle 200/300

            Session.SoapModelClient.begin_modification("Create member result intermediate point");

            // material
            model.Messages.Add($"Set material (IDs = {MATERIAL_ID}).");
            material material = new material
            {
                no = MATERIAL_ID,
                name = MATERIAL_STEEL
            };
            Session.SoapModelClient.set_material(material);


            // cross_section
            model.Messages.Add($"Set cross_section (IDs = {SECTION_ID}).");
            cross_section cross_section = new cross_section
            {
                no = SECTION_ID,
                name = SECTION_I,
                material = MATERIAL_ID,
                materialSpecified = true
            };
            Session.SoapModelClient.set_cross_section(cross_section);

            // nodes
            model.Messages.Add($"Set 2 nodes (IDs = [{NODE_1_ID}, {NODE_2_ID}]).");
            node node1 = new node
            {
                no = NODE_1_ID,
                coordinates = new vector_3d().SetCoordinates(1.0, 1.0, 0.0)
            };
            Session.SoapModelClient.set_node(node1);

            node node2 = new node
            {
                no = NODE_2_ID,
                coordinates = new vector_3d().SetCoordinates(5.0, 1.0, -1.0)
            };
            Session.SoapModelClient.set_node(node2);

#if RFEM
            // line
            model.Messages.Add($"Set line (IDs = {LINE_ID}).");
            line line = new line
            {
                no = LINE_ID,
                definition_nodes = new int[] { NODE_1_ID, NODE_2_ID }
            };
            Session.SoapModelClient.set_line(line);

#endif

            // member
            model.Messages.Add($"Set member (IDs = {MEMBER_ID}).");
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
            Session.SoapModelClient.set_member(member);

            // member result intermediate point
            model.Messages.Add($"Set member result intermediate point (IDs = {MEMBER_RESULT_INTERMEDIATE_POINT_ID}).");
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
            Session.SoapModelClient.set_member_result_intermediate_point(point);

            model.Name = "Adding member result intermediate point";

            return true;
        }
    }
}
