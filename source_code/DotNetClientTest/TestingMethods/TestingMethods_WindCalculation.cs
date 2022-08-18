#if RFEM

using Dlubal.WS.Common.Tools;
using System;
using static Dlubal.WS.Common.Tools.DataLogger;

using Dlubal.WS.Rfem6.Model;

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
        public static bool Test_Wind⁀Calculations_Profile_Get()
        {
            DataLogger.AddText("Reading wind profile...");
            try
            {
                ReadWindProfile();
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            return true;
        }

        public static bool Test_Wind⁀Calculations_Simulation_Get()
        {
            DataLogger.AddText("Reading wind simulation...");
            try
            {
                ReadWindSimulation();
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            return true;
        }

        public static bool Test_Wind⁀Calculations_Simulation⁀Analysis⁀Settings_Get()
        {
            DataLogger.AddText("Reading wind simulation analysis settings...");
            try
            {
                ReadWindSimulationAnalysisSettings();
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            return true;
        }

        public static bool Test_Wind⁀Calculations_Profile_Create()
        {
            DataLogger.AddText("Creating wind profile...");
            DataLogger.InitializeProgressBar(0, 50, 0);
            bool result = Test_General_Delete⁀All();
            if (!result || (SoapModelClient == null))
            {
                DataLogger.AddLogEnd(LogResultType.FAILED);
                return false;
            }

            DataLogger.SetProgressBarValue(10);
            try
            {
                CreateWindProfile();
                DataLogger.ResetProgressBar();
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                DataLogger.AddLogEnd(LogResultType.FAILED);
                return false;
            }

            DataLogger.AddLogEnd(LogResultType.DONE);
            return true;
        }

        public static bool Test_Wind⁀Calculations_Simulation_Create()
        {
            DataLogger.AddText("Creating wind simulation...");
            DataLogger.InitializeProgressBar(0, 50, 0);
            bool result = Test_General_Delete⁀All();
            if (!result || (SoapModelClient == null))
            {
                DataLogger.AddLogEnd(LogResultType.FAILED);
                return false;
            }

            DataLogger.SetProgressBarValue(10);
            try
            {
                CreateWindSimulation();
                DataLogger.ResetProgressBar();
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                DataLogger.AddLogEnd(LogResultType.FAILED);
                return false;
            }

            DataLogger.AddLogEnd(LogResultType.DONE);
            return true;
        }

        public static bool Test_Wind⁀Calculations_Simulation⁀Analysis⁀Settings_Create()
        {
            DataLogger.AddText("Creating wind simulation analysis settings...");
            DataLogger.InitializeProgressBar(0, 50, 0);
            bool result = Test_General_Delete⁀All();
            if (!result || (SoapModelClient == null))
            {
                DataLogger.AddLogEnd(LogResultType.FAILED);
                return false;
            }

            DataLogger.SetProgressBarValue(10);
            try
            {
                CreateWindSimulationAnalysisSettings();
                DataLogger.ResetProgressBar();
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                DataLogger.AddLogEnd(LogResultType.FAILED);
                return false;
            }

            DataLogger.AddLogEnd(LogResultType.DONE);
            return true;
        }

        public static bool Test_Wind⁀Calculations_Profile_Delete()
        {
            DataLogger.AddText("Deleting wind profile...");
            try
            {
                bool result = DeleteObjects(object_types.E_OBJECT_TYPE_WIND_PROFILE, 0, "Wind profile");
                DataLogger.AddLogEnd(LogResultType.DONE);
                return result;
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
        }

        public static bool Test_Wind⁀Calculations_Simulation_Delete()
        {
            DataLogger.AddText("Deleting simulation...");
            try
            {
                bool result = DeleteObjects(object_types.E_OBJECT_TYPE_WIND_SIMULATION, 0, "Wind simulation");
                DataLogger.AddLogEnd(LogResultType.DONE);
                return result;
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
        }

        public static bool Test_Wind⁀Calculations_Simulation⁀Analysis⁀Settings_Delete()
        {
            DataLogger.AddText("Deleting simulation analysis settings...");
            try
            {
                bool result = DeleteObjects(object_types.E_OBJECT_TYPE_WIND_SIMULATION_ANALYSIS_SETTINGS, 0, "Wind simulation analysis settings");
                DataLogger.AddLogEnd(LogResultType.DONE);
                return result;
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
        }

        private static void CreateWindProfile()
        {
            SoapModelClient.begin_modification(nameof(CreateWindProfile));
            DataLogger.SetProgressBarValue(20);
            DataLogger.AddText("Generating wind profile...");
            var profile = new wind_profile
            {
                no = 1,
                name = "Generated wind profile",
                type = wind_profile_type.USER_DEFINED,
                typeSpecified = true,
                user_defined_wind_profile = GetWindProfileUserDefinedWindProfileRows(),
                user_defined_wind_profile_uniform_intensity_enabled = false,
                user_defined_wind_profile_uniform_intensity_enabledSpecified = true,
            };
            DataLogger.SetProgressBarValue(40);
            SoapModelClient.set_wind_profile(profile);
            DataLogger.AddText("Generated profile");
            SoapModelClient.finish_modification();
            DataLogger.SetProgressBarValue(100);
        }

        private static wind_profile_user_defined_wind_profile_row[] GetWindProfileUserDefinedWindProfileRows()
        {
            const uint rowsCount = 30;
            const int heightMultiplier = 5;
            var rows = new wind_profile_user_defined_wind_profile_row[rowsCount];
            for (int i = 0; i < rowsCount; i++)
            {
                rows[i] = new wind_profile_user_defined_wind_profile_row()
                {
                    no = i + 1,
                    noSpecified = true,
                    row = new wind_profile_user_defined_wind_profile
                    {
                        height = i * heightMultiplier,
                        heightSpecified = true,
                        turbulence_intensity = 0.15,
                        turbulence_intensitySpecified = true,
                        velocity = i,
                        velocitySpecified = true
                    }
                };
            }
            return rows;
        }

        private static wind_simulation_analysis_settings GetWindSimulationAnalysisSettings()
        {
            var settings = new wind_simulation_analysis_settings
            {
                no = 1,
                name = "Generated wind simulation analysis settings",
                consider_turbulence = true,
                consider_turbulenceSpecified = true,
                member_load_distribution = wind_simulation_analysis_settings_member_load_distribution.UNIFORM,
                member_load_distributionSpecified = true,
                density = 1.5,
                densitySpecified = true,
            };
            return settings;
        }

        private static void CreateWindSimulation()
        {
            SoapModelClient.begin_modification(nameof(CreateWindSimulation));
            DataLogger.SetProgressBarValue(20);
            DataLogger.AddText("Generating wind simulation...");
            var settings = GetWindSimulationAnalysisSettings();
            DataLogger.AddText("Setting analysis settings for simulation...");
            SoapModelClient.set_wind_simulation_analysis_settings(settings);
            DataLogger.SetProgressBarValue(50);
            var simulation = new wind_simulation
            {
                no = 1,
                name = "Generated simulation",
                wind_simulation_analysis_settings = 1,
                wind_simulation_analysis_settingsSpecified = true,
            };

            DataLogger.SetProgressBarValue(40);
            SoapModelClient.set_wind_simulation(simulation);
            DataLogger.AddText("Generated wind simulation");
            SoapModelClient.finish_modification();
            DataLogger.SetProgressBarValue(100);
        }

        private static void CreateWindSimulationAnalysisSettings()
        {
            SoapModelClient.begin_modification(nameof(CreateWindSimulationAnalysisSettings));
            DataLogger.SetProgressBarValue(20);
            DataLogger.AddText("Generating wind simulation analysis settings...");
            var settings = GetWindSimulationAnalysisSettings();
            DataLogger.SetProgressBarValue(40);
            SoapModelClient.set_wind_simulation_analysis_settings(settings);
            DataLogger.AddText("Generated wind simulation analysis settings");
            SoapModelClient.finish_modification();
            DataLogger.SetProgressBarValue(100);
        }

        private static void ReadWindSimulationAnalysisSettings()
        {
            int count = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_WIND_SIMULATION_ANALYSIS_SETTINGS, 0);
            int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_WIND_SIMULATION_ANALYSIS_SETTINGS, 0);
            if (count != numbers.Length)
            {
                throw new Exception("Object count does not match object number count.");
            }

            DataLogger.AddText($"{numbers.Length} objects have been read.");
            DataLogger.InitializeProgressBar(0, numbers.Length, 0);
            for (int i = 0; i < count; i++)
            {
                var settings = SoapModelClient.get_wind_simulation_analysis_settings(numbers[i]);
                LogWindSimulationAnalysisSettings(settings);
                DataLogger.SetProgressBarValue(i);
            }
        }

        private static void LogWindSimulationAnalysisSettings(wind_simulation_analysis_settings settings)
        {
            DataLogger.AddText($"No: {settings.no}");
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Name: {settings.name}");
            DataLogger.AddText($"Simulation type: {settings.simulation_type}");
            DataLogger.AddText($"Density: {settings.density}");
            DataLogger.AddText($"Kinematic viscosity: {settings.kinematic_viscosity}");
            DataLogger.AddText($"Consider turbulance: {settings.consider_turbulence}");
            DataLogger.AddText($"Slip boundary condition on bottom boundary: {settings.slip_boundary_condition_on_bottom_boundary}");
            DataLogger.AddText($"Consider surface roughness: {settings.consider_surface_roughness}");
            DataLogger.AddText($"User defined dimensions of wind tunnel: {settings.user_defined_dimensions_of_wind_tunnel}");
            DataLogger.AddText($"Save solver data to continue calculation: {settings.save_solver_data_to_continue_calculation}");
            DataLogger.AddText($"Consider surface roughness: {settings.consider_surface_roughness}");
            DataLogger.AddText($"member load distribution: {settings.member_load_distribution}");
            DataLogger.AddText($"Finite volume mesh density: {settings.finite_volume_mesh_density}");
            DataLogger.AddText($"Use potential flow for initial condition: {settings.use_potential_flow_for_initial_condition}");
            DataLogger.AddText($"Use second order numerical scheme: {settings.use_second_order_numerical_scheme}");
            DataLogger.AddText($"Maximum number of iterations: {settings.maximum_number_of_iterations}");
            DataLogger.AddText($"Turbulency model: {settings.turbulence_model_type}");
            DataLogger.AddText($"Residual pressure: {settings.residual_pressure}");
            DataLogger.DecrementOffset();
        }

        private static void ReadWindProfile()
        {
            int count = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_WIND_PROFILE, 0);
            int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_WIND_PROFILE, 0);
            if (count != numbers.Length)
            {
                throw new Exception("Object count does not match object number count.");
            }

            DataLogger.AddText($"{numbers.Length} objects have been read.");
            DataLogger.InitializeProgressBar(0, numbers.Length, 0);
            for (int i = 0; i < count; i++)
            {
                var profile = SoapModelClient.get_wind_profile(numbers[i]);
                DataLogger.IncrementOffset();
                LogWindProfile(profile);
                DataLogger.DecrementOffset();
                DataLogger.SetProgressBarValue(i);
            }
        }

        private static void ReadWindSimulation()
        {
            int count = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_WIND_SIMULATION, 0);
            int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_WIND_SIMULATION, 0);
            if (count != numbers.Length)
            {
                throw new Exception("Object count does not match object number count.");
            }

            DataLogger.AddText($"{numbers.Length} objects have been read.");
            DataLogger.InitializeProgressBar(0, numbers.Length, 0);
            for (int i = 0; i < count; i++)
            {
                var simulation = SoapModelClient.get_wind_simulation(numbers[i]);
                DataLogger.IncrementOffset();
                LogWindSimulation(simulation);
                DataLogger.DecrementOffset();
                DataLogger.SetProgressBarValue(i);
            }
        }

        private static void LogWindSimulation(wind_simulation simulation)
        {
            DataLogger.IncrementOffset();
            DataLogger.AddText($"No: {simulation.no}");
            DataLogger.AddText($"Name: {simulation.name}");
            DataLogger.AddText($"Wind profile: {simulation.wind_profile}");
            DataLogger.AddText($"Initial state definition type: {simulation.initial_state_definition_type}");
            DataLogger.AddText($"Wind direction type: {simulation.wind_direction_type}");
            DataLogger.AddText($"Step angle: {simulation.uniform_wind_direction_step}");
            DataLogger.AddText($"Start angle: {simulation.uniform_wind_direction_range_start}");
            DataLogger.AddText($"End angle: {simulation.uniform_wind_direction_range_end}");
            foreach(var loadCase in simulation.generate_into_load_cases)
            {
                LogWindSimulationGenerateIntoLoadCasesRow(loadCase);
            }

            DataLogger.DecrementOffset();
        }

        private static void LogWindSimulationGenerateIntoLoadCasesRow(wind_simulation_generate_into_load_cases_row loadCaseRow)
        {
            DataLogger.IncrementOffset();
            DataLogger.AddText($"No: {loadCaseRow.no}");
            LogWindSimulationGenerateIntoLoadCases(loadCaseRow.row);
            DataLogger.DecrementOffset();
        }

        private static void LogWindSimulationGenerateIntoLoadCases(wind_simulation_generate_into_load_cases loadCase)
        {
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Direction: {loadCase.direction}");
            DataLogger.AddText($"LoadCase: {loadCase.load_case}");
            DataLogger.DecrementOffset();
        }

        private static void LogWindProfile(wind_profile profile)
        {
            DataLogger.AddText($"No: {profile.no}");
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Name: {profile.name}");
            DataLogger.AddText($"Comment: {profile.comment}");
            foreach(var userWindProfileRow in profile.user_defined_wind_profile)
            {
                LogWindUserProfileRow(userWindProfileRow);
            }

            DataLogger.AddText($"Period step: {profile.user_defined_wind_profile_period_step}");
            DataLogger.AddText($"Turbulence uniform speed itensity: {profile.user_defined_wind_profile_uniform_intensity}");
            DataLogger.DecrementOffset();
        }

        private static void LogWindUserProfileRow(wind_profile_user_defined_wind_profile_row userProfileRow)
        {
            DataLogger.IncrementOffset();
            DataLogger.AddText($"No: {userProfileRow.no}");
            LogWindUserProfile(userProfileRow.row);
            DataLogger.DecrementOffset();
        }

        private static void LogWindUserProfile(wind_profile_user_defined_wind_profile userProfile)
        {
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Height: {userProfile.height}");
            DataLogger.AddText($"Velocity: {userProfile.velocity}");
            DataLogger.AddText($"Turbulence intensity: {userProfile.turbulence_intensity}");
            DataLogger.DecrementOffset();
        }
    }
}

#endif // RFEM