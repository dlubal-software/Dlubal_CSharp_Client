#if !RSECTION

using Dlubal.WS.Common.Tools;
using System;
using static Dlubal.WS.Common.Tools.DataLogger;
using System.Collections.Generic;
using Dlubal.WS.Clients.DotNetClientTest.Tools;


#if RFEM
using Dlubal.WS.Rfem6.Model;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
#if RFEM
        public static bool Test_Divide⁀Object_Line()
        {
            InitializeTest();
            DataLogger.AddLogStart("Dividing line objects...");
            try
            {
                DivideObjectLine();
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            DataLogger.AddText("Line divided");
            return true;
        }
#endif
        public static bool Test_Divide⁀Object_Members()
        {
            InitializeTest();
            DataLogger.AddLogStart("Dividing member objects...");
            try
            {
                DivideObjectMembers();
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            DataLogger.AddText("Members divided");
            return true;
        }
#if RFEM
        public static bool Test_Divide⁀Object_Surfaces_Line()
        {
            InitializeTest();
            DataLogger.AddLogStart("Dividing line by surface...");
            try
            {
                DivideObjectSurfacesWithLine();
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            DataLogger.AddText("Line divided");
            return true;
        }

        public static bool Test_Divide⁀Object_Surfaces_Member()
        {
            InitializeTest();
            DataLogger.AddLogStart("Dividing member by surface...");
            try
            {
                DivideObjectSurfacesWithMember();
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            DataLogger.AddText("Members divided");
            return true;
        }

        private static void DivideObjectLine()
        {
            DataLogger.InitializeProgressBar();
            bool result = Test_General_Delete⁀All();
            if (!result || (SoapModelClient == null))
            {
                DataLogger.AddLogEnd(LogResultType.FAILED);
                return;
            }

            CreateNewLines();
            DataLogger.SetProgressBarValue(50);
            DivideByIntersectionLines();
            DataLogger.AddLogEnd(LogResultType.DONE);
        }
#endif
        private static void DivideObjectMembers()
        {
            DataLogger.InitializeProgressBar();
            bool result = Test_General_Delete⁀All();
            if (!result || (SoapModelClient == null))
            {
                DataLogger.AddLogEnd(LogResultType.FAILED);
                return;
            }

            CreateNewMembers();
            DataLogger.SetProgressBarValue(50);
            DivideByIntersectionMembers();
            DataLogger.AddLogEnd(LogResultType.DONE);
        }
#if RFEM
        private static void DivideObjectSurfacesWithLine()
        {
            DataLogger.InitializeProgressBar();
            bool result = Test_General_Delete⁀All();
            if (!result || (SoapModelClient == null))
            {
                DataLogger.AddLogEnd(LogResultType.FAILED);
                return;
            }

            CreateNewSurfaceAndLine();
            DataLogger.SetProgressBarValue(50);
            DivideByIntersectionSurfaceAndLine();
            DataLogger.AddLogEnd(LogResultType.DONE);
        }

        private static void DivideObjectSurfacesWithMember()
        {
            DataLogger.InitializeProgressBar();
            bool result = Test_General_Delete⁀All();
            if (!result || (SoapModelClient == null))
            {
                DataLogger.AddLogEnd(LogResultType.FAILED);
                return;
            }

            CreateNewSurfaceAndMember();
            DataLogger.SetProgressBarValue(50);
            DivideByIntersectionSurfaceAndMember();
            DataLogger.AddLogEnd(LogResultType.DONE);
        }

        private static void DivideByIntersectionLines()
        {
            ObjectsHaveSpecificCountOfItems(object_types.E_OBJECT_TYPE_LINE, 2);
            int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_LINE, 0);
            SoapModelClient.divide_by_intersections(new int[0], numbers, new int[0]);
            ObjectsHaveSpecificCountOfItems(object_types.E_OBJECT_TYPE_NODE, 5);
            numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_NODE, 0);

            node nodeCreatedByIntersection = SoapModelClient.get_node(numbers[4]);
            vector_3d nodeCoordinates = nodeCreatedByIntersection.coordinates;
            vector_3d whereShouldNewNodeBe = new vector_3d().SetCoordinates(0, 0, 0);
            if(!TwoVectorsAreSame(nodeCoordinates,whereShouldNewNodeBe))
            {
                throw new Exception($"Intersection node created on wrong place!");
            }

            ObjectsHaveSpecificCountOfItems(object_types.E_OBJECT_TYPE_LINE, 4);
        }
#endif
        private static void DivideByIntersectionMembers()
        {
            ObjectsHaveSpecificCountOfItems(object_types.E_OBJECT_TYPE_MEMBER, 2);
            int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_MEMBER, 0);
#if RFEM
            SoapModelClient.divide_by_intersections(numbers, new int[0], new int[0]);
#else
            SoapModelClient.divide_by_intersections(numbers);
#endif
            ObjectsHaveSpecificCountOfItems(object_types.E_OBJECT_TYPE_NODE, 5);
            numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_NODE, 0);

            node nodeCreatedByIntersection = SoapModelClient.get_node(numbers[4]);
            vector_3d nodeCoordinates = nodeCreatedByIntersection.coordinates;
            vector_3d whereShouldNewNodeBe = new vector_3d().SetCoordinates(0, 0, 0);
            if (!TwoVectorsAreSame(nodeCoordinates, whereShouldNewNodeBe))
            {
                throw new Exception($"Intersection node created on wrong place!");
            }

            ObjectsHaveSpecificCountOfItems(object_types.E_OBJECT_TYPE_MEMBER, 4);
        }
#if RFEM
        private static void DivideByIntersectionSurfaceAndLine()
        {
            ObjectsHaveSpecificCountOfItems(object_types.E_OBJECT_TYPE_SURFACE, 1);
            ObjectsHaveSpecificCountOfItems(object_types.E_OBJECT_TYPE_LINE, 5);
            int[] surfaceNumbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_SURFACE, 0);
            int[] lineNumbers = new int[] { 5 };

            SoapModelClient.divide_by_intersections(new int[0], lineNumbers, surfaceNumbers);
            ObjectsHaveSpecificCountOfItems(object_types.E_OBJECT_TYPE_NODE, 7);
            int[] nodeNumbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_NODE, 0);

            node nodeCreatedByIntersection = SoapModelClient.get_node(nodeNumbers[6]);
            vector_3d nodeCoordinates = nodeCreatedByIntersection.coordinates;
            vector_3d whereShouldNewNodeBe = new vector_3d().SetCoordinates(0, 0, 0);
            if (!TwoVectorsAreSame(nodeCoordinates, whereShouldNewNodeBe))
            {
                throw new Exception($"Intersection node created on wrong place!");
            }
        }

        private static void DivideByIntersectionSurfaceAndMember()
        {
            ObjectsHaveSpecificCountOfItems(object_types.E_OBJECT_TYPE_SURFACE, 1);
            ObjectsHaveSpecificCountOfItems(object_types.E_OBJECT_TYPE_MEMBER, 1);
            int[] surfaceNumbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_SURFACE, 0);
            int[] memberNumbers = new int[] { 1 };

            SoapModelClient.divide_by_intersections(memberNumbers, new int[0],  surfaceNumbers);
            ObjectsHaveSpecificCountOfItems(object_types.E_OBJECT_TYPE_NODE, 7);
            int[] nodeNumbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_NODE, 0);

            node nodeCreatedByIntersection = SoapModelClient.get_node(nodeNumbers[6]);
            vector_3d nodeCoordinates = nodeCreatedByIntersection.coordinates;
            vector_3d whereShouldNewNodeBe = new vector_3d().SetCoordinates(0, 0, 0);
            if (!TwoVectorsAreSame(nodeCoordinates, whereShouldNewNodeBe))
            {
                throw new Exception($"Intersection node created on wrong place!");
            }
        }

        private static void CreateNewLines()
        {
            List<node> nodes = new List<node>
            {
                new node { no = 1, coordinates = new vector_3d().SetCoordinates(-3, 0, 0) },
                new node { no = 2, coordinates = new vector_3d().SetCoordinates(3, 0, 0) },
                new node { no = 3, coordinates = new vector_3d().SetCoordinates(0, 3, 0) },
                new node { no = 4, coordinates = new vector_3d().SetCoordinates(0, -3, 0) },
            };
            nodes.ForEach(node => SoapModelClient.set_node(node));
            List<line> lines = new List<line>
            {
                new line { no = 1, definition_nodes = new int[] { 1, 2 } },
                new line { no = 2, definition_nodes = new int[] { 3, 4 } },
            };
            lines.ForEach(line => SoapModelClient.set_line(line));
        }
#endif
        private static void CreateNewMembers()
        {
            List<node> nodes = new List<node>
            {
                new node { no = 1, coordinates = new vector_3d().SetCoordinates(-3, 0, 0) },
                new node { no = 2, coordinates = new vector_3d().SetCoordinates(3, 0, 0) },
                new node { no = 3, coordinates = new vector_3d().SetCoordinates(0, 3, 0) },
                new node { no = 4, coordinates = new vector_3d().SetCoordinates(0, -3, 0) },
            };
            nodes.ForEach(node => SoapModelClient.set_node(node));
#if RFEM
            List<line> lines = new List<line>
            {
                new line { no = 1, definition_nodes = new int[] { 1, 2 } },
                new line { no = 2, definition_nodes = new int[] { 3, 4 } },
            };
            lines.ForEach(line => SoapModelClient.set_line(line));
#endif
            material material = new material()
            {
                no = 1,
                name = "S235"
            };
            SoapModelClient.set_material(material);

            section section = new section()
            {
                no = 1,
                material = 1,
                materialSpecified = true,
            };
            SoapModelClient.set_section(section);

            List<member> members = new List<member>
            {
                new member {
                    no = 1,
#if RFEM
                    line = 1,
                    lineSpecified = true,
#else
                    node_start = 1,
                    node_startSpecified = true,
                    node_end = 2,
                    node_endSpecified = true,
#endif
                    section_start = 1,
                    section_startSpecified = true,
                    section_end = 1,
                    section_endSpecified = true,
                },
                new member {
                    no = 2,
#if RFEM
                    line = 2,
                    lineSpecified = true,
#else
                    node_start = 3,
                    node_startSpecified = true,
                    node_end = 4,
                    node_endSpecified = true,
#endif
                    section_start = 1,
                    section_startSpecified = true,
                    section_end = 1,
                    section_endSpecified = true,
                },
            };
            members.ForEach(member => SoapModelClient.set_member(member));
        }

#if RFEM
        private static void CreateNewSurfaceAndLine()
        {
            List<node> nodes = new List<node>
            {
                new node { no = 1, coordinates = new vector_3d().SetCoordinates(-3, -3, 0) },
                new node { no = 2, coordinates = new vector_3d().SetCoordinates(3, -3, 0) },
                new node { no = 3, coordinates = new vector_3d().SetCoordinates(3, 3, 0) },
                new node { no = 4, coordinates = new vector_3d().SetCoordinates(-3, 3, 0) },

                new node { no = 5, coordinates = new vector_3d().SetCoordinates(0, 0, 3) },
                new node { no = 6, coordinates = new vector_3d().SetCoordinates(0, 0, -3) },
            };
            nodes.ForEach(node => SoapModelClient.set_node(node));

            List<line> lines = new List<line>
            {
                new line { no = 1, definition_nodes = new int[] { 1, 2 } },
                new line { no = 2, definition_nodes = new int[] { 2, 3 } },
                new line { no = 3, definition_nodes = new int[] { 3, 4 } },
                new line { no = 4, definition_nodes = new int[] { 4, 1 } },

                new line { no = 5, definition_nodes = new int[] { 5, 6 } },
            };
            lines.ForEach(line => SoapModelClient.set_line(line));

            material material = new material()
            {
                no = 1,
                name = "S235"
            };
            SoapModelClient.set_material(material);

            thickness thickness = new thickness()
            {
                no = 1,
                material = 1,
                materialSpecified = true,
            };
            SoapModelClient.set_thickness(thickness);

            List<surface> surfaces = new List<surface>()
            {
                new surface
                {
                    no = 1,
                    boundary_lines = new int[]{1, 2, 3, 4 },
                    thickness = 1,
                    thicknessSpecified = true,
                }
            };
            surfaces.ForEach(surface => SoapModelClient.set_surface(surface));
        }

        private static void CreateNewSurfaceAndMember()
        {
            List<node> nodes = new List<node>
            {
                new node { no = 1, coordinates = new vector_3d().SetCoordinates(-3, -3, 0) },
                new node { no = 2, coordinates = new vector_3d().SetCoordinates(3, -3, 0) },
                new node { no = 3, coordinates = new vector_3d().SetCoordinates(3, 3, 0) },
                new node { no = 4, coordinates = new vector_3d().SetCoordinates(-3, 3, 0) },

                new node { no = 5, coordinates = new vector_3d().SetCoordinates(0, 0, 3) },
                new node { no = 6, coordinates = new vector_3d().SetCoordinates(0, 0, -3) },
            };
            nodes.ForEach(node => SoapModelClient.set_node(node));

            List<line> lines = new List<line>
            {
                new line { no = 1, definition_nodes = new int[] { 1, 2 } },
                new line { no = 2, definition_nodes = new int[] { 2, 3 } },
                new line { no = 3, definition_nodes = new int[] { 3, 4 } },
                new line { no = 4, definition_nodes = new int[] { 4, 1 } },

                new line { no = 5, definition_nodes = new int[] { 5, 6 } },
            };
            lines.ForEach(line => SoapModelClient.set_line(line));

            material material = new material()
            {
                no = 1,
                name = "S235"
            };
            SoapModelClient.set_material(material);

            thickness thickness = new thickness()
            {
                no = 1,
                material = 1,
                materialSpecified = true,
            };
            SoapModelClient.set_thickness(thickness);

            section section = new section()
            {
                no = 1,
                material = 1,
                materialSpecified = true,
            };
            SoapModelClient.set_section(section);

            List<member> members = new List<member>
            {
                new member {
                    no = 1,
                    line = 5,
                    lineSpecified = true,
                    section_start = 1,
                    section_startSpecified = true,
                    section_end = 1,
                    section_endSpecified = true,
                }
            };
            members.ForEach(member => SoapModelClient.set_member(member));

            List<surface> surfaces = new List<surface>()
            {
                new surface
                {
                    no = 1,
                    boundary_lines = new int[]{1,2,3,4},
                    thickness = 1,
                    thicknessSpecified = true,
                }
            };
            surfaces.ForEach(surface => SoapModelClient.set_surface(surface));
        }
#endif
        private static void ObjectsHaveSpecificCountOfItems(object_types objectType,int requiredCount)
        {
            int count = SoapModelClient.get_object_count(objectType, 0);
            int[] numbers = SoapModelClient.get_all_object_numbers(objectType, 0);
            if (count != numbers.Length || count != requiredCount)
            {
                throw new Exception($"Object count does not match object number count or lines count is not {requiredCount}! (count: {count})");
            }
        }

        private static bool TwoVectorsAreSame(vector_3d first, vector_3d second)
        {
            bool sameVectors = (first.x == second.x) && (first.y == second.y) && (first.z == second.z);
            return sameVectors;
        }
    }
}

#endif // !RSESCTION