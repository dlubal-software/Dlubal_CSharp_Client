#if RFEM

using Dlubal.WS.Clients.DotNetClientTest.Tools;
using Dlubal.WS.Common.Tools;
using Dlubal.WS.Rfem6.Model;
using System;
using System.Collections.Generic;

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
        private static line_nurbs_control_points_row[] From_Pts(in line_nurbs_control_points[] pts)
        {
            var ret = new line_nurbs_control_points_row[pts.Length];
            for (int i = 0; i != pts.Length; ++i)
            {
                ret[i] = new line_nurbs_control_points_row { row = pts[i] };
            }
            return ret;
        }
        private static line_nurbs_knots_row[] From_Doubles(in double[] doubles)
        {
            var ret = new line_nurbs_knots_row[doubles.Length];
            for (int i = 0; i != doubles.Length; ++i)
            {
                ret[i] = new line_nurbs_knots_row { row = doubles[i] };
            }
            return ret;
        }
        public static bool Test_Basic\u2040Objects_Lines_Nurbs_Create()
        {
            const int NODE_1_NO = 3;
            const int NODE_2_NO = 4;
            const int LINE_1_NO = 5;
            const int LINE_2_NO = 6;
            const int LINE_3_NO = 7;

            DataLogger.AddLogStart("Creating NURBS line...");
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
                SoapModelClient.begin_modification("Create NURBS line");

                // nodes
                DataLogger.AddText($"Set 2 nodes...");
                List<node> nodes = new List<node>
                {
                    new node { no = NODE_1_NO, coordinates = new vector_3d().SetCoordinates(10.0, 0.0, 0.0) },
                    new node { no = NODE_2_NO, coordinates = new vector_3d().SetCoordinates(12.0, 0.0, 0.0) }
                };
                nodes.ForEach(node => SoapModelClient.set_node(node));

                DataLogger.SetProgressBarValue(20);

                // NURBS line 1
                DataLogger.AddText($"Set NURBS line No.{LINE_1_NO}");
                line line1 = new line
                {
                    no = LINE_1_NO,
                    type = line_type.TYPE_NURBS,
                    typeSpecified = true,
                    definition_nodes = new int[] { NODE_1_NO, NODE_2_NO },
                    nurbs_control_points = From_Pts
                    (
                        new line_nurbs_control_points[]
                        {
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(3.000, -1.000, 0.000, 1.000),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(1.205, -2.793, 0.000, 1.100),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(1.000, -4.000, 0.000, 1.200),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(3.000, -4.000, 0.000, 1.300),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(2.000, -6.000, 0.000, 1.400),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(1.000, -8.000, 0.000, 1.500),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(3.000, -8.000, 0.000, 1.600),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(5.000, -5.000, 0.000, 1.700),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(4.000, -3.000, 0.000, 1.800),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(6.000, -1.000, 0.000, 1.900),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(4.000, -1.000, 0.000, 1.000)
                        }
                    ),
                    nurbs_order = 3,
                    nurbs_orderSpecified = true,
                    nurbs_knots = From_Doubles(new double[] { 0.0, 0.0, 0.0, 0.111, 0.222, 0.333, 0.444, 0.555, 0.666, 0.777, 0.888, 1.0, 1.0, 1.0 })
                };
                SoapModelClient.set_line(line1);

                DataLogger.AddText($"  control points count = {line1.nurbs_control_points.Length}, order = {line1.nurbs_order}, knots count = {line1.nurbs_knots.Length}");
                DataLogger.SetProgressBarValue(30);

                // NURBS line 2
                DataLogger.AddText($"Set NURBS line No.{LINE_2_NO}");
                line line2 = new line
                {
                    no = LINE_2_NO,
                    type = line_type.TYPE_NURBS,
                    typeSpecified = true,
                    definition_nodes = new int[] { NODE_1_NO, NODE_2_NO },
                    nurbs_control_points = From_Pts
                    (
                        new line_nurbs_control_points[]
                        {
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(3.000, -1.000, 0.000, 1.000),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(1.205, -2.793, 0.000, 1.100),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(1.000, -4.000, 0.000, 1.200),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(3.000, -4.000, 0.000, 1.300),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(2.000, -6.000, 0.000, 1.400),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(1.000, -8.000, 0.000, 1.500),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(3.000, -8.000, 0.000, 1.600),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(5.000, -5.000, 0.000, 1.700),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(4.000, -3.000, 0.000, 1.800),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(6.000, -3.000, 0.000, 1.900),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(6.000, -1.000, 0.000, 2.000),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(4.000, -1.000, 0.000, 1.000)
                        }
                    ),
                    nurbs_order = 4,
                    nurbs_orderSpecified = true,
                    nurbs_knots = From_Doubles(new double[] { 0.0, 0.0, 0.0, 0.0, 0.111, 0.222, 0.333, 0.444, 0.555, 0.666, 0.777, 0.888, 1.0, 1.0, 1.0, 1.0 })
                };
                SoapModelClient.set_line(line2);

                DataLogger.AddText($"  control points count = {line2.nurbs_control_points.Length}, order = {line2.nurbs_order}, knots count = {line2.nurbs_knots.Length}");
                DataLogger.SetProgressBarValue(40);

                // NURBS line 3
                DataLogger.AddText($"Set NURBS line No.{LINE_3_NO}");
                line line3 = new line
                {
                    no = LINE_3_NO,
                    type = line_type.TYPE_NURBS,
                    typeSpecified = true,
                    definition_nodes = new int[] { NODE_1_NO, NODE_2_NO },
                    nurbs_control_points = From_Pts
                    (
                        new line_nurbs_control_points[]
                        {
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(3.000, -1.000, 0.000, 1.000),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(1.205, -2.793, 0.000, 1.100),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(1.000, -4.000, 0.000, 1.200),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(3.000, -4.000, 0.000, 1.300),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(2.000, -6.000, 0.000, 1.400),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(1.000, -8.000, 0.000, 1.500),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(3.000, -8.000, 0.000, 1.600),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(5.000, -5.000, 0.000, 1.700),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(4.000, -3.000, 0.000, 1.800),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(4.000, -1.000, 0.000, 1.000)
                        }
                    ),
                    nurbs_order = 2,
                    nurbs_orderSpecified = true,
                    nurbs_knots = From_Doubles(new double[] { 0.0, 0.0, 0.111, 0.222, 0.333, 0.444, 0.555, 0.666, 0.777, 0.888, 1.0, 1.0 })
                };
                SoapModelClient.set_line(line3);

                DataLogger.AddText($"  control points count = {line3.nurbs_control_points.Length}, order = {line3.nurbs_order}, knots count = {line3.nurbs_knots.Length}");
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

        public static bool Test_Basic\u2040Objects_Lines_Nurbs_Get()
        {
            DataLogger.AddLogStart("Reading lines ids...");

            try
            {
                int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_LINE, 0);
                DataLogger.AddText($"{numbers.Length} line ids have been read.");
                DataLogger.InitializeProgressBar(0, numbers.Length, 0);

                int progress = 0;
                foreach (int number in numbers)
                {
                    line line = SoapModelClient.get_line(number);
                    if (line.type == line_type.TYPE_NURBS)
                    {
                        DataLogger.AddText($"NURBS No.{number}: control points count = {line.nurbs_control_points.Length}, order = {line.nurbs_order}, knots count = {line.nurbs_knots.Length}");
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

        public static bool Test_Basic\u2040Objects_Lines_Nurbs_Modify()
        {
            const int NODE_1_NO = 3;
            const int NODE_2_NO = 4;
            const int LINE_NO = 5;

            DataLogger.AddLogStart("Modify NURBS line...");
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
                SoapModelClient.begin_modification("Set NURBS line");

                // create nodes
                DataLogger.AddText($"Set 2 nodes...");
                List<node> nodes = new List<node>
                {
                    new node { no = NODE_1_NO, coordinates = new vector_3d().SetCoordinates(10.0, 0.0, 0.0) },
                    new node { no = NODE_2_NO, coordinates = new vector_3d().SetCoordinates(12.0, 0.0, 0.0) }
                };
                nodes.ForEach(node => SoapModelClient.set_node(node));

                DataLogger.SetProgressBarValue(20);

                // create line
                DataLogger.AddText($"Set NURBS line No.{LINE_NO}");
                line line = new line
                {
                    no = LINE_NO,
                    type = line_type.TYPE_NURBS,
                    typeSpecified = true,
                    definition_nodes = new int[] { NODE_1_NO, NODE_2_NO },
                    nurbs_control_points = From_Pts
                    (
                        new line_nurbs_control_points[]
                        {
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(3.000, -1.000, 0.000, 1.000),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(1.205, -2.793, 0.000, 1.100),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(1.000, -4.000, 0.000, 1.200),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(3.000, -4.000, 0.000, 1.300),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(2.000, -6.000, 0.000, 1.400),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(1.000, -8.000, 0.000, 1.500),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(3.000, -8.000, 0.000, 1.600),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(5.000, -5.000, 0.000, 1.700),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(4.000, -3.000, 0.000, 1.800),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(6.000, -1.000, 0.000, 1.900),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(4.000, -1.000, 0.000, 1.000)
                        }
                    ),
                    nurbs_order = 3,
                    nurbs_orderSpecified = true,
                    nurbs_knots = From_Doubles(new double[] { 0.0, 0.0, 0.0, 0.111, 0.222, 0.333, 0.444, 0.555, 0.666, 0.777, 0.888, 1.0, 1.0, 1.0 })
                };
                SoapModelClient.set_line(line);

                DataLogger.SetProgressBarValue(30);

                SoapModelClient.finish_modification();

                // increase
                SoapModelClient.begin_modification("Increase NURBS line tables");

                line.nurbs_control_points = From_Pts
                    (
                        new line_nurbs_control_points[]
                        {
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(3.000, -1.000, 0.000, 1.000),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(1.205, -2.793, 0.000, 1.100),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(1.000, -4.000, 0.000, 1.200),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(3.000, -4.000, 0.000, 1.300),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(2.000, -6.000, 0.000, 1.400),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(1.000, -8.000, 0.000, 1.500),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(3.000, -8.000, 0.000, 1.600),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(5.000, -5.000, 0.000, 1.700),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(4.000, -3.000, 0.000, 1.800),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(6.000, -1.000, 0.000, 1.900),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(4.000, -1.000, 0.000, 1.000),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(7.000, -1.000, 0.000, 1.000)
                        }
                    )
                ;
                line.nurbs_order = 4;
                line.nurbs_knots = From_Doubles(new double[] { 0.0, 0.0, 0.0, 0.0, 0.111, 0.222, 0.333, 0.444, 0.555, 0.666, 0.777, 0.888, 1.0, 1.0, 1.0, 1.0 });
                SoapModelClient.set_line(line);

                DataLogger.SetProgressBarValue(40);

                SoapModelClient.finish_modification();

                // decrease
                SoapModelClient.begin_modification("Decrease NURBS line tables");

                line.nurbs_control_points = From_Pts
                    (
                        new line_nurbs_control_points[]
                        {
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(3.000, -1.000, 0.000, 1.000),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(1.205, -2.793, 0.000, 1.100),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(1.000, -4.000, 0.000, 1.200),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(3.000, -4.000, 0.000, 1.300),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(2.000, -6.000, 0.000, 1.400),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(1.000, -8.000, 0.000, 1.500),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(3.000, -8.000, 0.000, 1.600),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(5.000, -5.000, 0.000, 1.700),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(4.000, -3.000, 0.000, 1.800),
                            new line_nurbs_control_points().SetNurbsLocalControlPoint(4.000, -1.000, 0.000, 1.000)
                        }
                    )
                ;
                line.nurbs_order = 2;
                line.nurbs_knots = From_Doubles(new double[] { 0.0, 0.0, 0.111, 0.222, 0.333, 0.444, 0.555, 0.666, 0.777, 0.888, 1.0, 1.0 });
                SoapModelClient.set_line(line);

                DataLogger.SetProgressBarValue(50);

                SoapModelClient.finish_modification();
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
