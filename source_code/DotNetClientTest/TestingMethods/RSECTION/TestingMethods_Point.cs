#if RSECTION

using System;
using Dlubal.WS.Clients.DotNetClientTest.Tools;
using Dlubal.WS.Common.Tools;

using Dlubal.WS.RSection1.Model;

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
        public static bool Test_RSection_Points_Create\u2040Points()
        {
            const int POINT_1_NO = 1;
            const int POINT_2_NO = 2;
            const int POINT_3_NO = 3;
            const int POINT_4_NO = 4;
            const int POINT_5_NO = 5;

            const int LINE_NO = 6;

            DataLogger.AddLogStart("Creating points...");
            DataLogger.InitializeProgressBar(0, 100, 0);

            bool result = Test_General_Delete⁀All();
            if (!result || (SoapModelClient == null))
            {
                DataLogger.AddLogEnd(DataLogger.LogResultType.FAILED);
                return false;
            }

            DataLogger.SetProgressBarValue(4);

            try
            {
                SoapModelClient.begin_modification("set points");

                // standard point
                DataLogger.AddText("Set 2 standard points.");

                point point1 = new point
                {
                    no = POINT_1_NO,
                    type = point_type.TYPE_STANDARD,
                    typeSpecified = true,
                    coordinates = new vector_3d().SetCoordinates(1.0, 1.0, 0.0)
                };
                SoapModelClient.set_point(point1);

                DataLogger.SetProgressBarValue(10);

                point point2 = new point
                {
                    no = POINT_2_NO,
                    type = point_type.TYPE_STANDARD,
                    typeSpecified = true,
                    coordinates = new vector_3d().SetCoordinates(2.0, 2.0, 0.0)
                };
                SoapModelClient.set_point(point2);

                DataLogger.SetProgressBarValue(20);

                // point on line
                DataLogger.AddText("Set a point on line.");

                line line1 = new line
                {
                    no = LINE_NO
                    , type = line_type.TYPE_POLYLINE
                    , typeSpecified = true
                    , definition_points = new int[] { POINT_1_NO, POINT_2_NO }
                };
                SoapModelClient.set_line(line1);

                point point3 = new point
                {
                    no = POINT_3_NO,
                    type = point_type.TYPE_ON_LINE,
                    typeSpecified = true,
                    distance_from_start_relative = 0.40,
                    distance_from_start_relativeSpecified = true,
                    on_line_reference_line = LINE_NO,
                    on_line_reference_lineSpecified = true,
                };
                SoapModelClient.set_point(point3);

                DataLogger.SetProgressBarValue(30);

                // point between two points
                DataLogger.AddText("Set point between two points.");
                point point4 = new point
                {
                    no = POINT_4_NO,
                    type = point_type.TYPE_BETWEEN_TWO_POINTS,
                    typeSpecified = true,
                    distance_from_start_is_defined_as_relative = true,
                    distance_from_start_is_defined_as_relativeSpecified = true,
                    distance_from_start_relative = 0.70,
                    distance_from_start_relativeSpecified = true,
                    between_two_points_start_point = POINT_1_NO,
                    between_two_points_start_pointSpecified = true,
                    between_two_points_end_point = POINT_2_NO,
                    between_two_points_end_pointSpecified = true,
                };
                SoapModelClient.set_point(point4);

                DataLogger.SetProgressBarValue(40);

                // point between two locations
                DataLogger.AddText($"Set point between two locations.");
                point point5 = new point
                {
                    no = POINT_5_NO,
                    type = point_type.TYPE_BETWEEN_TWO_LOCATIONS,
                    typeSpecified = true,
                    distance_from_start_is_defined_as_relative = true,
                    distance_from_start_is_defined_as_relativeSpecified = true,
                    distance_from_start_relative = 0.45,
                    distance_from_start_relativeSpecified = true,
                    between_two_locations_start_point_coordinate_1 = 0.6,
                    between_two_locations_start_point_coordinate_1Specified = true,
                    between_two_locations_start_point_coordinate_2 = -1.5,
                    between_two_locations_start_point_coordinate_2Specified = true,
                    between_two_locations_end_point_coordinate_1 = -1.0,
                    between_two_locations_end_point_coordinate_1Specified = true,
                    between_two_locations_end_point_coordinate_2 = 0,
                    between_two_locations_end_point_coordinate_2Specified = true,
                };
                SoapModelClient.set_point(point5);

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
                DataLogger.AddText($"Points = {SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_POINT, 0)}");
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }

        public static bool Test_RSection_Points_Get\u2040Points()
        {
            DataLogger.AddLogStart("Reading point numbers...");
            DataLogger.InitializeProgressBar(0, 100, 50);

            try
            {
                int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_POINT, 0);
                DataLogger.AddText($"{numbers.Length} point ids have been read.");
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
#endif //RSECTION