using System.Reflection;

#if RFEM
using Dlubal.WS.Rfem6.Model;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest.Tools
{
    public static class Extensions
    {
        public static string ConvertToString(this MethodInfo method)
        {
            string methodName = method.Name.Replace('\u2040', ' ');
            methodName = methodName.Replace('_', '/');

            if (methodName.ToLower().StartsWith("test/"))
            {
                methodName = methodName.Substring(5);
            }

            return methodName;
        }

        public static vector_3d SetCoordinates(this vector_3d vector, double x, double y, double z)
        {
            vector.x = x;
            vector.y = y;
            vector.z = z;
            return vector;
        }

        public static string ToStringEx(this vector_3d vector)
        {
            return $"{vector.x,8:F3}; {vector.y,8:F3}; {vector.z,8:F3}";
        }

#if RFEM

        public static line_nurbs_control_points SetNurbsLocalControlPoint(this line_nurbs_control_points controlPoint, double x, double y, double z, double weight)
        {
            controlPoint.coordinates = new vector_3d().SetCoordinates(x, y, z);
            controlPoint.weight = weight;
            return controlPoint;
        }

        public static line_nurbs_control_points SetNurbsGlobalControlPoint(this line_nurbs_control_points controlPoint, double x, double y, double z, double weight)
        {
            controlPoint.global_coordinates = new vector_3d().SetCoordinates(x, y, z);
            controlPoint.weight = weight;
            return controlPoint;
        }

        public static surface_nurbs_control_points SetNurbsLocalControlPoint(this surface_nurbs_control_points controlPoint, double x, double y, double z, double weight)
        {
            controlPoint.coordinates = new vector_3d().SetCoordinates(x, y, z);
            controlPoint.weight = weight;
            return controlPoint;
        }

        public static surface_nurbs_control_points SetNurbsGlobalControlPoint(this surface_nurbs_control_points controlPoint, double x, double y, double z, double weight)
        {
            controlPoint.global_coordinates = new vector_3d().SetCoordinates(x, y, z);
            controlPoint.weight = weight;
            return controlPoint;
        }

#endif // RFEM

        public static member_result_intermediate_point_distances SetDistance(this member_result_intermediate_point_distances distance, double value, string note)
        {
            distance.value = value;
            distance.note = note;
            return distance;
        }

        public static imperfection_case SetDefault(this imperfection_case imperfectionCase, int caseId, imperfection_case_type type, bool setNameAndComment = true)
        {
            imperfectionCase.no = caseId;
            imperfectionCase.type = type;
            imperfectionCase.typeSpecified = true;

            if (setNameAndComment)
            {
                imperfectionCase.name = $"IC{caseId} name";
                imperfectionCase.user_defined_name_enabled = true;
                imperfectionCase.user_defined_name_enabledSpecified = true;
                imperfectionCase.comment = $"IC{caseId} comment";
            }

            return imperfectionCase;
        }
    }
}