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
        public static bool Test_Load\u2040Wizards_Wind\u2040Simulations()
        {
            return RunSetterGetterTest(
                setterTest1Helper: new PropertiesHelper<wind_simulation>{
                    Properties = new (string, object)[]{
                        ("no", 1)
                        , ("type", wind_simulation_type.STANDARD)
                        , ("user_defined_name_enabled", true)
                        , ("name", "♥")
                        , ("active", true)
                        , ("wind_profile", 1)
                        , ("wind_simulation_analysis_settings", 1)
                        , ("wind_direction_type", wind_simulation_wind_direction_type.UNIFORM)
                        , ("uniform_wind_direction_step", 0.17453292519943295)
                        , ("uniform_wind_direction_range_start", 0)
                        , ("uniform_wind_direction_range_end", 6.283185307179586)
                        , ("consider_initial_state", false)
                    }
                }
                , setterTest2Helper: new PropertiesHelper<wind_simulation>{
                    Properties = new (string, object)[]{
                        ("no", 1)
                        , ("user_defined_name_enabled", false)
                        , ("uniform_wind_direction_step", 0.15707963267948966)
                        , ("uniform_wind_direction_range_start", 0.017453292519943295)
                        , ("uniform_wind_direction_range_end", 4.886921905584122)
                        , ("consider_initial_state", true)
                        , ("initial_state_case", 1)
                        , ("initial_state_definition_type", wind_simulation_initial_state_definition_type.DEFINITION_TYPE_FINAL_STATE)
                    }
                }
                , new string[] { "special_solutions.wind_simulation_active" }
            );
        }

        public static bool Test_Load\u2040Wizards_Wind\u2040Profile()
        {
            return RunSetterGetterTest(
                setterTest1Helper: new PropertiesHelper<wind_profile>{
                    Properties = new (string, object)[]{
                        ("no", 1)
                        , ("type", wind_profile_type.ACCORDING_TO_STANDARD)
                        , ("user_defined_name_enabled", true)
                        , ("name", "♥")
                    }
                }
                , setterTest2Helper: new PropertiesHelper<wind_profile>{
                    Properties = new (string, object)[]{
                        ("no", 1)
                        , ("type", wind_profile_type.USER_DEFINED)
                        , ("user_defined_name_enabled", false)
                        , (
                            "user_defined_wind_profile"
                            , new PropertiesHelper<wind_profile_user_defined_wind_profile_row>[]{
                                new PropertiesHelper<wind_profile_user_defined_wind_profile_row>{
                                    Properties = new (string, object)[]{
                                        ("no", 1)
                                        , (
                                            "row"
                                            , new PropertiesHelper<wind_profile_user_defined_wind_profile>{
                                                Properties = new (string, object)[]{
                                                    ("height", 0)
                                                    , ("velocity", 20)
                                                    , ("turbulence_intensity", 0.15)
                                                }
                                            }
                                        )
                                    }
                                }
                                , new PropertiesHelper<wind_profile_user_defined_wind_profile_row>{
                                    Properties = new (string, object)[]{
                                        ("no", 2)
                                        , (
                                            "row"
                                            , new PropertiesHelper<wind_profile_user_defined_wind_profile>{
                                                Properties = new (string, object)[]{
                                                    ("height", 1)
                                                    , ("velocity", 25)
                                                    , ("turbulence_intensity", 0.17)
                                                }
                                            }
                                        )
                                    }
                                }
                            }
                        )
                    }
                }
                , new string[] { "special_solutions.wind_simulation_active" }
            );
        }
#endif
    }
}
