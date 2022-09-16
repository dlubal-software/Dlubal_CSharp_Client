using Dlubal.WS.Common.Tools;
using System;

#if RFEM
using Dlubal.WS.Rfem6.Model;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
#elif RSECTION
using Dlubal.WS.RSection1.Model;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
#if RFEM
        public static bool Test_Configs_Mesh\u2040Settings()
        {
            return RunSetterGetterTestForConfig(
                getter: (() => SoapModelClient.get_mesh_settings())
                , setter: (settings => SoapModelClient.set_mesh_settings(settings))
                , setterTest1Helper: new PropertiesHelper<meshConfig>{
                    Properties = new (string, object)[]{
                        ("general_target_length_of_fe", 0.5)
                        , ("general_maximum_distance_between_node_and_line", 0.001)
                        , ("general_maximum_number_of_mesh_nodes", 500)
                        , ("members_number_of_divisions_for_special_types", 10)
                        , ("members_number_of_divisions_for_result_diagram", 10)
                        , ("members_number_of_divisions_for_min_max_values", 10)
                        , ("members_number_of_divisions_for_concrete_members", 10)
                        , ("members_length_of_fe", 0.5)
                        , ("surfaces_maximum_ratio_of_fe", 1.8)
                        , ("solids_target_length_of_fe_for_type_soil", 1)
                        , ("solids_maximum_number_of_elements", 200)
                        , ("members_minimum_number_of_divisions", 1)
                        , ("surfaces_integrate_also_unutilized_objects", false)
                        , (
                            "surfaces_shape_of_finite_elements"
                            , meshConfig_surfaces_shape_of_finite_elements_type.E_SHAPE_OF_FINITE_ELEMENTS_FOR_SURFACES__TRIANGLES_AND_QUADRANGLES
                        )
                        , ("solids_mesh_nodes_in_position_of_precise_layer_surfaces", false)
                        , (
                            "SurfacesMeshQualityConfig"
                            , new PropertiesHelper<SurfacesMeshQualityConfig>{
                                Properties = new (string, object)[]{
                                    ("mesh_quality_color_indicator_ok_color", "#00ff00")
                                    , (
                                        "QualityCriteriaConfigForSurfaces"
                                        , new PropertiesHelper<QualityCriteriaConfigForSurfaces>{
                                            Properties = new (string, object)[]{
                                                ("quality_criterion_check_aspect_ratio", true)
                                                , ("quality_criterion_check_aspect_ratio_warning", 20)
                                                , ("quality_criterion_check_aspect_ratio_failure", 100)
                                                , ("quality_criterion_parallel_deviations_failure", 1.892526803190927)
                                                , ("quality_criterion_corner_angles_of_triangle_elements_warning", 1.8325957145940461)
                                                , ("quality_criterion_corner_angles_of_triangle_elements_failure", 3.0543261909900767)
                                                , ("quality_criterion_corner_angles_of_quadrangle_elements_warning", 2.705260340591211)
                                                , ("quality_criterion_corner_angles_of_quadrangle_elements_failure", 3.0543261909900767)
                                                , ("quality_criterion_jacobian_ratio_warning", 30)
                                                , ("quality_criterion_jacobian_ratio_failure", 800)
                                            }
                                        }
                                    )
                                }
                            }
                        )
                        , (
                            "windSimulationMeshConfig"
                            , new PropertiesHelper<windSimulationMeshConfig>{
                                Properties = new (string, object)[]{
                                    ("windsimulation_mesh_config_value_simplify_model", true)
                                    , (
                                        "windsimulation_mesh_config_value_determine_details_by"
                                        , windSimulationMeshConfig_windsimulation_mesh_config_value_determine_details_by_type
                                          .E_WINDSIMULATION_DETERMINE_DETAILS_BY__LEVEL_OF_DETAILS
                                    )
                                    , ("windsimulation_mesh_config_value_detail_size", 0.01)
                                    , ("windsimulation_mesh_config_value_optimized_member_topo_value", 5)
                                }
                            }
                        )
                    }
                }
                , setterTest2Helper: new PropertiesHelper<meshConfig>{
                    Properties = new (string, object)[]{
                        ("general_target_length_of_fe", 0.6)
                        , ("general_maximum_distance_between_node_and_line", 0.002)
                        , ("general_maximum_number_of_mesh_nodes", 600)
                        , ("members_number_of_divisions_for_special_types", 11)
                        , ("members_number_of_divisions_for_result_diagram", 11)
                        , ("members_number_of_divisions_for_min_max_values", 11)
                        , ("members_number_of_divisions_for_concrete_members", 11)
                        , ("members_length_of_fe", 0.6)
                        , ("surfaces_maximum_ratio_of_fe", 2.8)
                        , ("solids_target_length_of_fe_for_type_soil", 2)
                        , ("solids_maximum_number_of_elements", 210)
                        , ("members_minimum_number_of_divisions", 2)
                        , ("surfaces_integrate_also_unutilized_objects", true)
                        , (
                            "surfaces_shape_of_finite_elements"
                            , meshConfig_surfaces_shape_of_finite_elements_type.E_SHAPE_OF_FINITE_ELEMENTS_FOR_SURFACES__TRIANGLES_ONLY
                        )
                        , ("surfaces_triangles_for_membranes", true)
                        , (
                            "SurfacesMeshQualityConfig"
                            , new PropertiesHelper<SurfacesMeshQualityConfig>{
                                Properties = new (string, object)[]{
                                    ("mesh_quality_color_indicator_ok_color", "#00aa00")
                                    , (
                                        "QualityCriteriaConfigForSurfaces"
                                        , new PropertiesHelper<QualityCriteriaConfigForSurfaces>{
                                            Properties = new (string, object)[]{
                                                ("quality_criterion_check_aspect_ratio", false)
                                                , ("quality_criterion_check_aspect_ratio_warning", 25)
                                                , ("quality_criterion_check_aspect_ratio_failure", 90)
                                                , ("quality_criterion_parallel_deviations_failure", 1.792526803190927)
                                                , ("quality_criterion_corner_angles_of_triangle_elements_warning", 1.8425957145940461)
                                                , ("quality_criterion_corner_angles_of_triangle_elements_failure", 3.0553261909900767)
                                                , ("quality_criterion_corner_angles_of_quadrangle_elements_warning", 2.715260340591211)
                                                , ("quality_criterion_corner_angles_of_quadrangle_elements_failure", 3.0553261909900767)
                                                , ("quality_criterion_jacobian_ratio_warning", 35)
                                                , ("quality_criterion_jacobian_ratio_failure", 850)
                                            }
                                        }
                                    )
                                }
                            }
                        )
                        , (
                            "SolidsMeshQualityConfig"
                            , new PropertiesHelper<SolidsMeshQualityConfig>{
                                Properties = new (string, object)[]{
                                    ("mesh_quality_color_indicator_warning_color", "#ff1100")
                                    , (
                                        "QualityCriteriaConfigForSolids"
                                        , new PropertiesHelper<QualityCriteriaConfigForSolids>{
                                            Properties = new (string, object)[]{
                                                ("quality_criterion_check_aspect_ratio", false)
                                                , ("quality_criterion_check_aspect_ratio_warning", 25)
                                                , ("quality_criterion_check_aspect_ratio_failure", 90)
                                                , ("quality_criterion_parallel_deviations_failure", 1.792526803190927)
                                                , ("quality_criterion_corner_angles_of_triangle_elements_warning", 1.8425957145940461)
                                                , ("quality_criterion_corner_angles_of_triangle_elements_failure", 3.0553261909900767)
                                                , ("quality_criterion_corner_angles_of_quadrangle_elements_warning", 2.715260340591211)
                                                , ("quality_criterion_corner_angles_of_quadrangle_elements_failure", 3.0553261909900767)
                                                , ("quality_criterion_jacobian_ratio_warning", 35)
                                                , ("quality_criterion_jacobian_ratio_failure", 850)
                                            }
                                        }
                                    )
                                }
                            }
                        )
                    }
                }
            );
        }
#endif

        public static bool Test_Configs_Optimization\u2040Settings()
        {
            return RunSetterGetterTestForConfig(
                getter: (() => SoapModelClient.get_optimization_settings())
                , setter: (settings => SoapModelClient.set_optimization_settings(settings))
                , setterTest1Helper: new PropertiesHelper<optimizationSettingsConfig>
                {
                    Properties = new (string, object)[]{
                        ("general_optimization_active", false)
                        , ("general_keep_best_number_model_mutations", 11)
                        , ("general_optimize_on", optimizationSettingsConfig_general_optimize_on_type.E_OPTIMIZE_ON_TYPE_MIN_WHOLE_WEIGHT)
                        , ("general_optimizer", optimizationSettingsConfig_general_optimizer_type.E_OPTIMIZER_TYPE_ALL_MUTATIONS)
                        , ("general_number_random_mutations", 0.2)
                    }
                }
                , setterTest2Helper: new PropertiesHelper<optimizationSettingsConfig>
                {
                    Properties = new (string, object)[]{
                        ("general_optimization_active", true)
                        , ("general_keep_best_number_model_mutations", 11)
                        , ("general_optimize_on", optimizationSettingsConfig_general_optimize_on_type.E_OPTIMIZE_ON_TYPE_MIN_VECTORIAL_DISPLACEMENT)
                        , ("general_optimizer", optimizationSettingsConfig_general_optimizer_type.E_OPTIMIZER_TYPE_PERCENTS_OF_RANDOM_MUTATIONS)
                        , ("general_number_random_mutations", 0.25)
                    }
                }
            );
        }

        public static bool Test_Configs_Model\u2040Info()
        {
            DataLogger.AddLogStart($"Testing {nameof(modelInfo)}...");
            DataLogger.InitializeProgressBar(0, 1, 0);

            bool succeeded = true;

            try
            {
                DataLogger.AddText($"Invoke {nameof(SoapModelClient.get_model_info)}.");
                var info = SoapModelClient.get_model_info();
                DataLogger.SetProgressBarValue(1);
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                succeeded = false;
            }
            finally
            {
                DataLogger.ResetProgressBar();
            }

            DataLogger.AddLogEnd(succeeded ? DataLogger.LogResultType.DONE : DataLogger.LogResultType.FAILED);
            return succeeded;
        }
    }
}
