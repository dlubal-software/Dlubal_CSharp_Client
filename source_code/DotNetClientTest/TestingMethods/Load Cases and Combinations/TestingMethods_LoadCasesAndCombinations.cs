using Dlubal.WS.Common.Tools;
using System;

#if RFEM
using Dlubal.WS.Rfem6.Model;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public partial class TestingMethods
    {
        public static bool Test_Load\u2040Cases\u2040and\u2040Combinations_Settings_Get()
        {
            DataLogger.AddLogStart("Get load cases and combination settings...");
            DataLogger.InitializeProgressBar(0, 10, 0);

            bool succeeded = InitializeTest();
            if (!succeeded)
            {
                return false;
            }

            try
            {
                load_cases_and_combinations settings = SoapModelClient.get_load_cases_and_combinations();

                DumpObject(settings);

                DataLogger.SetProgressBarValue(10);
            }
            catch (Exception ex)
            {
                succeeded = false;
                ModelWsExceptionHandler(ex);
                return false;
            }

            DataLogger.AddLogEnd(succeeded ? DataLogger.LogResultType.DONE : DataLogger.LogResultType.FAILED);
            DataLogger.ResetProgressBar();
            return succeeded;
        }

        public static bool Test_Load\u2040Cases\u2040and\u2040Combinations_Settings_Set()
        {
            DataLogger.AddLogStart("Get load cases and combination settings...");
            DataLogger.InitializeProgressBar(0, 10, 0);

            bool succeeded = InitializeTest();
            if (!succeeded)
            {
                return false;
            }

            try
            {
                string[] ignorePropertiesList = new string[0];

                load_cases_and_combinations settingsWritten = new load_cases_and_combinations
                {
                    activate_combination_wizard = false,
                    activate_combination_wizardSpecified = true,
                    activate_combination_wizard_and_classification = true,
                    activate_combination_wizard_and_classificationSpecified = true,
                    combination_name_according_to_action_category = false,
                    combination_name_according_to_action_categorySpecified = true,
                    current_standard_for_combination_wizard = 6232,
                    current_standard_for_combination_wizardSpecified = true,
                    result_combinations_active = false,
                    result_combinations_activeSpecified = true,
                    result_combinations_consider_sub_results = false,
                    result_combinations_consider_sub_resultsSpecified = true,
                    result_combinations_parentheses_active = true,
                    result_combinations_parentheses_activeSpecified = true,
                };

                SoapModelClient.set_load_cases_and_combinations(settingsWritten);

                load_cases_and_combinations settingsRead = SoapModelClient.get_load_cases_and_combinations();

                AreObjectsEqual(settingsWritten, settingsRead, nameof(load_cases_and_combinations), ignorePropertiesList);

                DataLogger.SetProgressBarValue(10);

                settingsWritten = settingsRead;
                settingsWritten.activate_combination_wizard = true;
                settingsWritten.activate_combination_wizardSpecified = true;

                SoapModelClient.set_load_cases_and_combinations(settingsWritten);

                settingsRead = SoapModelClient.get_load_cases_and_combinations();

                AreObjectsEqual(settingsWritten, settingsRead, nameof(load_cases_and_combinations), ignorePropertiesList);

                DataLogger.SetProgressBarValue(20);
            }
            catch (Exception ex)
            {
                succeeded = false;
                ModelWsExceptionHandler(ex);
                return false;
            }

            DataLogger.AddLogEnd(succeeded ? DataLogger.LogResultType.DONE : DataLogger.LogResultType.FAILED);
            DataLogger.ResetProgressBar();
            return succeeded;
        }

        public static bool Test_Load\u2040Cases\u2040and\u2040Combinations_Modal\u2040Analysis\u2040Settings()
        {
            return RunSetterGetterTest(
                setterTest1Helper: new PropertiesHelper<modal_analysis_settings>{
                    Properties = new (string, object)[]{
                        ("no", 1)
                        , ("acting_masses_about_axis_x_enabled", true)
                        , ("acting_masses_about_axis_y_enabled", true)
                        , ("acting_masses_about_axis_z_enabled", true)
                        , ("acting_masses_in_direction_x_enabled", true)
                        , ("acting_masses_in_direction_y_enabled", true)
                        , ("acting_masses_in_direction_z_enabled", true)
                        , ("find_eigenvectors_beyond_frequency", false)
                        , ("mass_conversion_type", modal_analysis_settings_mass_conversion_type.MASS_CONVERSION_TYPE_Z_COMPONENTS_OF_LOADS)
                        , ("mass_matrix_type", modal_analysis_settings_mass_matrix_type.MASS_MATRIX_TYPE_CONSISTENT)
                        , ("number_of_modes_method", modal_analysis_settings_number_of_modes_method.NUMBER_OF_MODES_METHOD_USER_DEFINED)
                        , ("number_of_modes", 4)
                        , ("user_defined_name_enabled", true)
                        , ("name", "♥")
                        , ("neglect_masses", modal_analysis_settings_neglect_masses.E_NEGLECT_MASSES_NO_NEGLECTION)
                    }
                }
                , setterTest2Helper: new PropertiesHelper<modal_analysis_settings>{
                    Properties = new (string, object)[]{
                        ("no", 1)
                        , ("acting_masses_about_axis_x_enabled", false)
                        , ("acting_masses_about_axis_y_enabled", false)
                        , ("acting_masses_about_axis_z_enabled", false)
                        , ("acting_masses_in_direction_x_enabled", false)
                        , ("acting_masses_in_direction_y_enabled", false)
                        , ("acting_masses_in_direction_z_enabled", false)
                        , ("activate_minimum_initial_prestress", true)
                        , ("solution_method", modal_analysis_settings_solution_method.METHOD_ROOT_OF_CHARACTERISTIC_POLYNOMIAL)
                        , ("find_eigenvectors_beyond_frequency", true)
                        , ("user_defined_name_enabled", false)
#if RSTAB
                        , ("solution_method", modal_analysis_settings_solution_method.METHOD_SUBSPACE_ITERATION)
#endif
                    }
                }
                , new string[] { "dynamic_analysis_settings.modal_active" }
            );
        }

        public static bool Test_Load\u2040Cases\u2040and\u2040Combinations_Stability\u2040Analysis\u2040Settings()
        {
            return RunSetterGetterTest(
                setterTest1Helper: new PropertiesHelper<stability_analysis_settings>{
                    Properties = new (string, object)[]{
                        ("no", 1)
                        , ("user_defined_name_enabled", true)
                        , ("name", "♥")
                        , ("analysis_type", stability_analysis_settings_analysis_type.EIGENVALUE_METHOD)
                        , ("number_of_lowest_eigenvalues", 4)
                        , ("considered_favored_effect", true)
                        , ("find_eigenvectors_beyond_critical_load_factor", false)
                        , ("calculate_without_loading_for_instability", false)
                        , ("activate_minimum_initial_prestress", true)
                        , ("minimum_initial_strain", 1e-05)
                        , ("display_local_torsional_rotations", false)
#if RFEM
                        , ("eigenvalue_method", stability_analysis_settings_eigenvalue_method.EIGENVALUE_METHOD_LANCZOS)
#elif RSTAB
                        , ("eigenvalue_method", stability_analysis_settings_eigenvalue_method.E_EIGENVALUE_METHOD_SHIFTED_INVERSE_POWER_METHOD)
#endif
                        , ("matrix_type", stability_analysis_settings_matrix_type.MATRIX_TYPE_STANDARD)
                    }
                }
                , setterTest2Helper: new PropertiesHelper<stability_analysis_settings>{
                    Properties = new (string, object)[]{
                        ("no", 1)
                        , ("user_defined_name_enabled", false)
                        , ("analysis_type", stability_analysis_settings_analysis_type.INCREMENTALY_METHOD_WITH_EIGENVALUE)
                        , ("number_of_lowest_eigenvalues", 5)
                        , ("considered_favored_effect", false)
                        , ("find_eigenvectors_beyond_critical_load_factor", false)
                        , ("calculate_without_loading_for_instability", false)
                        , ("activate_minimum_initial_prestress", true)
                        , ("minimum_initial_strain", 2e-05)
                        , ("display_local_torsional_rotations", true)
#if RFEM
                        , ("eigenvalue_method", stability_analysis_settings_eigenvalue_method.EIGENVALUE_METHOD_ROOTS_OF_CHARACTERISTIC_POLYNOMIAL)
#elif RSTAB
                        , ("eigenvalue_method", stability_analysis_settings_eigenvalue_method.EIGENVALUE_METHOD_SUBSPACE_ITERATION)
#endif
                        , ("matrix_type", stability_analysis_settings_matrix_type.MATRIX_TYPE_UNIT)
                    }
                }
                , new string[] { "analysis.structure_stability_active" }
            );
        }

        public static bool Test_Load\u2040Cases\u2040and\u2040Combinations_Spectral\u2040Analysis\u2040Settings()
        {
            return RunSetterGetterTest(
                setterTest1Helper: new PropertiesHelper<spectral_analysis_settings>{
                    Properties = new(string, object)[]{
                        ("no", 1)
                        , ("user_defined_name_enabled", true)
                        , ("name", "♥")
                        , ("combination_rule_for_periodic_responses", spectral_analysis_settings_combination_rule_for_periodic_responses.SRSS)
                        , ("use_equivalent_linear_combination", false)
                        , ("signed_results_using_dominant_mode", false)
                        , ("save_results_of_all_selected_modes", false)
                        , ("combination_rule_for_directional_components", spectral_analysis_settings_combination_rule_for_directional_components.SRSS)
                    }
                }
                , setterTest2Helper: new PropertiesHelper<spectral_analysis_settings>{
                    Properties = new (string, object)[]{
                        ("no", 1)
                        , ("user_defined_name_enabled", false)
                        , ("combination_rule_for_periodic_responses", spectral_analysis_settings_combination_rule_for_periodic_responses.CQC)
                        , ("use_equivalent_linear_combination", true)
                        , ("signed_results_using_dominant_mode", true)
                        , ("save_results_of_all_selected_modes", true)
                        , ("combination_rule_for_directional_components", spectral_analysis_settings_combination_rule_for_directional_components.SCALED_SUM)
                    }
                }
                , new string[] { "dynamic_analysis_settings.spectral_active" }
            );
        }

#if RFEM
        public static bool Test_Load\u2040Cases\u2040and\u2040Combinations_Wind\u2040Simulation\u2040Analysis\u2040Settings()
        {
            return RunSetterGetterTest(
                setterTest1Helper: new PropertiesHelper<wind_simulation_analysis_settings>
                {
                    Properties = new (string, object)[]{
                        ("no", 1)
                        , ("simulation_type", wind_simulation_analysis_settings_simulation_type.STEADY_FLOW)
                        , ("user_defined_name_enabled", true)
                        , ("name", "♥")
                        , ("density", 1.25)
                        , ("kinematic_viscosity", 1.5e-05)
                        , ("numerical_solver", wind_simulation_analysis_settings_numerical_solver.OPEN_FOAM)
                        , ("finite_volume_mesh_density", 0.2)
                        , ("maximum_number_of_iterations", 500)
                        , ("mesh_refinement_type", wind_simulation_analysis_settings_mesh_refinement_type.DISTANCE_FROM_SURFACE)
                        , ("snap_to_model_edges", true)
                        , ("boundary_layers_checked", false)
                        , ("consider_turbulence", true)
                        , ("slip_boundary_condition_on_bottom_boundary", false)
                        , ("use_potential_flow_for_initial_condition", true)
                        , ("use_second_order_numerical_scheme", false)
                        , ("user_defined_dimensions_of_wind_tunnel", false)
                        , ("turbulence_model_type", wind_simulation_analysis_settings_turbulence_model_type.TURBULENCE_TYPE_EPSILON)
                        , ("velocity_field", 0.5)
                        , ("specific_turbulence_dissipation_rate", 0.5)
                        , ("turbulence_dissipation_rate", 0.5)
                        , ("turbulence_intermittency", 0.5)
                        , ("turbulence_kinetic_energy", 0.5)
                        , ("turbulence_kinetic_viscosity", 0.5)
                        , ("save_solver_data_to_continue_calculation", false)
                        , ("pressure_field", 0.2)
                        , ("residual_pressure", 0.001)
                        , ("momentum_thickness_reynolds_number", 0.5)
                        , ("consider_surface_roughness", false)
                    }
                }
                , setterTest2Helper: new PropertiesHelper<wind_simulation_analysis_settings>
                {
                    Properties = new (string, object)[]{
                        ("no", 1)
                        , ("simulation_type", wind_simulation_analysis_settings_simulation_type.TRANSIENT_FLOW)
                        , ("user_defined_name_enabled", false)
                        , ("density", 1.3)
                        , ("kinematic_viscosity", 2e-05)
                        , ("finite_volume_mesh_density", 0.21)
                        , ("maximum_number_of_iterations", 300)
                        , ("boundary_layers_checked", true)
                        , ("boundary_layers_value", 6)
                        , ("consider_turbulence", false)
                        , ("slip_boundary_condition_on_bottom_boundary", true)
                        , ("user_defined_dimensions_of_wind_tunnel", true)
                        , ("member_load_distribution", wind_simulation_analysis_settings_member_load_distribution.TRAPEZOIDAL)
                        , ("sand_grain_roughness_height", 0.002)
                        , ("roughness_constant", 0.5)
                        , ("user_defined_simulation_time", false)
                        , ("velocity_field", 0.51)
                        , ("user_defined_in_domain_for_flow_animation", true)
                        , ("user_defined_in_point_probes", true)
                        , ("specific_turbulence_dissipation_rate", 0.51)
                        , ("steady_flow_from_solver", true)
                        , ("turbulence_dissipation_rate", 0.51)
                        , ("turbulence_intermittency", 0.51)
                        , ("turbulence_kinetic_energy", 0.51)
                        , ("turbulence_kinetic_viscosity", 0.51)
                        , ("save_solver_data_to_continue_calculation", true)
                        , ("pressure_field", 0.21)
                        , ("residual_pressure", 0.011)
                        , ("momentum_thickness_reynolds_number", 0.51)
                        , ("data_compression_error_tolerance", 0.001)
                        , ("consider_surface_roughness", true)
                    }
                }
                , new string[] { "special_solutions.wind_simulation_active" }
            );
        }
#endif
    }
}
