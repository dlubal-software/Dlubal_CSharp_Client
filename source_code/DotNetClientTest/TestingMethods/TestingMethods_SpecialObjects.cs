#if !RSECTION

using Dlubal.WS.Common.Tools;
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
        public static bool Test_Special\u2040Objects_Terrain()
        {
            return RunSetterGetterTest(
                setterTest1Helper: new PropertiesHelper<terrain>{
                    Properties = new (string, object)[]{
                        ("no", 1)
                        , ("type", terrain_type.HORIZONTAL_PLANE)
                        , ("comment", "this is terrain")
                        , ("bounding_box_offset_x", 30)
                        , ("bounding_box_offset_y", 30)
                        , ("center_of_terrain_z", 0)
                        , ("rotation_around_Z", 0)
                    }
                }
                , setterTest2Helper: new PropertiesHelper<terrain>{
                    Properties = new (string, object)[]{
                        ("no", 1)
                        , ("type", terrain_type.INCLINED_PLANE)
                        , ("bounding_box_offset_x", 29)
                        , ("bounding_box_offset_y", 31)
                        , ("coordinate_system", 1)
                    }
                }
            );
        }

#if RFEM
        public static bool Test_Special\u2040Objects_Soil\u2040Massif()
        {
            return RunSetterGetterTest(
                setterTest1Helper: new PropertiesHelper<soil_massif>
                {
                    Properties = new (string, object)[]{
                        ("no", 1)
                        , ("type", soil_massif_type.TYPE_STANDARD)
                        , ("user_defined_name_enabled", true)
                        , ("name", "♥")
                        , ("assigned_to_type", soil_massif_assigned_to_type.ASSIGNED_TO_TYPE_SOIL_SOLIDS)
                        , ("topology_type", soil_massif_topology_type.TOPOLOGY_TYPE_RECTANGLE)
                        , ("depth_according_to_soil_samples", true)
                        , ("center_x", 0)
                        , ("center_y", 0)
                        , (
                            "size"
                            , new PropertiesHelper<vector_3d>
                            {
                                Properties = new (string, object)[]{
                                      ("x", 100D)
                                    , ("y", 100D)
                                    , ("z", double.NaN)
                                }
                            }
                        )
                        , ("rotation_about_z", 0)
                        , ("groundwater", false)
                    }
                }
                , setterTest2Helper: new PropertiesHelper<soil_massif>
                {
                    Properties = new (string, object)[]{
                        ("no", 1)
                        , ("type", soil_massif_type.TYPE_STANDARD)
                        , ("user_defined_name_enabled", false)
                        , ("center_x", 1)
                        , ("center_y", 2)
                        , ("size_x", 200)
                        , ("size_y", 200)
                        , ("rotation_about_z", 0.2618)
                        , ("groundwater", true)
                    }
                }
                , new string[] { "special_solutions.geotechnical_analysis_active" }
            );
        }
#endif
    }
}

#endif // !RSECTION
