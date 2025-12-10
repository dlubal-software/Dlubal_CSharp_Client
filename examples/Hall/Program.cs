using Dlubal.WS.Rfem6.Application;
using Dlubal.WS.Rfem6.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ComponentModel; 

namespace Hall
{

    internal class Program
    {

        public static EndpointAddress Address { get; set; } = new EndpointAddress("http://localhost:8081");

        private static BasicHttpBinding Binding
        {
            get
            {
                BasicHttpBinding binding = new BasicHttpBinding
                {
                    // Send timeout is set to 180 seconds.
                    SendTimeout = new TimeSpan(0, 0, 180),
                    UseDefaultWebProxy = true,
                    MaxReceivedMessageSize = 1000000000,
                };

                return binding;
            }
        }
        private static RfemApplicationClient application = null;

        static string DecodeHtmlString(string myEncodedString)
        {
            StringWriter myWriter = new StringWriter();
            // Decode the encoded string.
            System.Net.WebUtility.HtmlDecode(myEncodedString, myWriter);
            string myDecodedString = myWriter.ToString();

            return myDecodedString;



            //Console.Write($"Decoded string of the above encoded string is: {myDecodedString}");
        }

        public static void TestingExample(RfemModelClient model, Logger logger)
        {
            string CurrentDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            string ExamplesDirectory = CurrentDirectory + @"\ExampleFiles\";
            
            #region Model type
            model_type modelType = model.get_model_type();
            Console.WriteLine("Model type is : {0}", modelType.ToString());

            #endregion
            
            #region AddonList
            addon_list_type addon = model.get_addon_statuses();
            Console.WriteLine("Material nonlinear analysis active?: {0}", addon.analysis.material_nonlinear_analysis_active ? "Yes" : "No");
            Console.WriteLine("Structure stability active?: {0}", addon.analysis.structure_stability_active ? "Yes" : "No");
            Console.WriteLine("Construction stages active?: {0}", addon.analysis.construction_stages_active ? "Yes" : "No");
            Console.WriteLine("Time dependent active?: {0}", addon.analysis.time_dependent_active ? "Yes" : "No");
            Console.WriteLine("Form finding active?: {0}", addon.analysis.form_finding_active ? "Yes" : "No");
            Console.WriteLine("Warping active?: {0}", addon.analysis.torsional_warping_active ? "Yes" : "No");
            Console.WriteLine("Modal analysis active?: {0}", addon.dynamic_analysis_settings.modal_active ? "Yes" : "No");
            Console.WriteLine("Spectral analysis active?: {0}", addon.dynamic_analysis_settings.spectral_active ? "Yes" : "No");
            Console.WriteLine("Building model active?: {0}", addon.special_solutions.building_model_active ? "Yes" : "No");
            Console.WriteLine("Wind simulation active?: {0}", addon.special_solutions.wind_simulation_active ? "Yes" : "No");
            Console.WriteLine("Geotechnical analysis active?: {0}", addon.special_solutions.geotechnical_analysis_active ? "Yes" : "No");
            Console.WriteLine("Stress analysis active?: {0}", addon.design_addons.stress_analysis_active ? "Yes" : "No");
            Console.WriteLine("Concrete desing active?: {0}", addon.design_addons.concrete_design_active ? "Yes" : "No");
            Console.WriteLine("Steel desing active?: {0}", addon.design_addons.steel_design_active ? "Yes" : "No");
            Console.WriteLine("Timber desing active?: {0}", addon.design_addons.timber_design_active ? "Yes" : "No");
            Console.WriteLine("Masonry desing active?: {0}", addon.masonry_design_active ? "Yes" : "No");
            Console.WriteLine("Aluminum desing active?: {0}", addon.design_addons.aluminum_design_active ? "Yes" : "No");
            Console.WriteLine("Steel joints desing active?: {0}", addon.design_addons.steel_joints_active ? "Yes" : "No");
            Console.WriteLine("Cost estimation active?: {0}", addon.analysis.cost_estimation_active ? "Yes" : "No");

            addon.design_addons.concrete_design_active = true;
            addon.design_addons.concrete_design_activeSpecified = true;
            addon.design_addons.steel_design_active = true;
            addon.design_addons.steel_design_activeSpecified = true;
            addon.design_addons.timber_design_active = true;
            addon.design_addons.timber_design_activeSpecified = true;
            addon.special_solutions.wind_simulation_active = true;
            addon.special_solutions.wind_simulation_activeSpecified = true;
            addon.dynamic_analysis_settings.modal_active = true;
            addon.dynamic_analysis_settings.modal_activeSpecified = true;
            addon.dynamic_analysis_settings.spectral_active = true;
            addon.dynamic_analysis_settings.spectral_activeSpecified = true;
            addon.analysis.structure_stability_active = true;
            addon.analysis.structure_stability_activeSpecified = true;
            addon.design_addons.steel_joints_active = false;
            addon.design_addons.steel_joints_activeSpecified = true;
            addon.design_addons.aluminum_design_active = true;
            addon.design_addons.aluminum_design_activeSpecified = true;
            addon.special_solutions.geotechnical_analysis_active = true;
            addon.special_solutions.geotechnical_analysis_activeSpecified = true;

            try
            {
                model.begin_modification("Set AddOns");
                model.set_addon_statuses(addon);
            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something wrong in setting AddOns\n" + exception.Message + "\n");
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of creation of AddOns\n" + exception.Message + "\n");
                }
            }
            #endregion

            #region Model parameters - Location
            // model_parameters_location_row[] modelLocation = model.get_model_parameters_location();
            List<model_parameters_location_row> modelLocation = new List<model_parameters_location_row>();
            modelLocation.Add(
                new model_parameters_location_row()
                {
                    no = 1,
                    row = new model_parameters_location()
                    {
                        name = "Country",
                        value = "CZE"
                    }
                }
            );
            modelLocation.Add(
                new model_parameters_location_row()
                {
                    no = 2,
                    row = new model_parameters_location()
                    {
                        name = "Street",
                        value = "Kosmonautů 1282"
                    }
                }
            );
            modelLocation.Add(
                new model_parameters_location_row()
                {
                    no = 3,
                    row = new model_parameters_location()
                    {
                        name = "Zip / Postal code",
                        value = "535 01"
                    }
                }
            );
            modelLocation.Add(
                new model_parameters_location_row()
                {
                    no = 4,
                    row = new model_parameters_location()
                    {
                        name = "City",
                        value = "Přelouč"
                    }
                }
            );

            try
            {
                model.begin_modification("Location");
                // model.set_model_parameters_location(modelLocation.ToArray());
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                logger.Error(exception, "Something wrong in setting of location\n" + exception.Message + "\n");
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in location creation\n" + exception.Message + "\n");
                }
            }


            #endregion // Model parameters - Location

            #region Base data - model parameters
            // Base data - model parameters - 16 items in the table, modelID and projectID cannot be changed
            model_parameters_row[] modelParameters = model.get_model_parameters();
            var clientNameRow = from line in modelParameters where line.row.name == "Client name" select line;
            modelParameters.Where(line => line.row.name == "Client name").First().row.description_1 = "Karel Hlavacek";
            var lastFilledRow = modelParameters.Where(line => string.IsNullOrEmpty(line.row.name)).First().no;
            Console.WriteLine(clientNameRow.FirstOrDefault().row.name);
            model_parameters_row myModelParameter = new model_parameters_row()
            {
                no = lastFilledRow,
                row = new model_parameters()
                {
                    name = "My new parameter",
                    description_1 = "not sure what to put here",
                    description_2 = "not sure what to put here too",
                }

            };
            var paramList = modelParameters.ToList();
            paramList.Add(myModelParameter);

            try
            {
                model.begin_modification("Model params");
                model.set_model_parameters(paramList.ToArray());

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                logger.Error(exception, "Something wrong in setting of model params\n" + exception.Message + "\n");
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in location creation\n" + exception.Message + "\n");
                }
            }

            #endregion

            #region Terrain
            // terrain terrain = model.get_terrain(1);
            terrain terrainModel = new terrain()
            {
                no = 1,
                type = terrain_type.BOREHOLES,
                coordinate_system = 0,
                coordinate_systemSpecified = true,
                typeSpecified = true,
                bounding_box_offset_x = 100.0,
                bounding_box_offset_xSpecified = true,
                bounding_box_offset_y = 100.0,
                bounding_box_offset_ySpecified = true,
                center_of_terrain_z = 0.0,
                center_of_terrain_zSpecified = true,
                rotation_around_Z = 0.0,
                rotation_around_ZSpecified = true,
                comment = "Terrain 1",
                consider_boreholes = false,
                consider_boreholesSpecified = true,
            };
            try
            {
                model.begin_modification("Terrain 1");
                model.set_terrain(terrainModel);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                logger.Error(exception, "Something wrong in setting of terrain\n" + exception.Message + "\n");
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in terrain creation\n" + exception.Message + "\n");
                }
            }
            #endregion


            #region Soil samples

            // create material
            material gravel = new material
            {
                no = 11,
                name = "Gravel, closely graded"
            };

            material sand = new material
            {
                no = 12,
                name = "Sand",
            };

            material clay = new material
            {
                no = 13,
                name = "Clay, slightly plastic"
            };
            borehole_layers_table_row[] soilSampleLayersTable = new borehole_layers_table_row[3];
            soilSampleLayersTable[0] = new borehole_layers_table_row()
            {
                no = 1,
                row = new borehole_layers_table()
                {
                    soil_material = sand.no,
                    soil_materialSpecified = true,
                    depth = 2.0,
                    depthSpecified = true,
                    bottom_ordinate = 1.0,
                    bottom_ordinateSpecified = true,
                }
            };
            soilSampleLayersTable[1] = new borehole_layers_table_row()
            {
                no = 2,
                row = new borehole_layers_table()
                {
                    soil_material = gravel.no,
                    soil_materialSpecified = true,
                    depth = 3.0,
                    depthSpecified = true,
                    bottom_ordinate = 2.0,
                    bottom_ordinateSpecified = true,
                }
            };
            soilSampleLayersTable[2] = new borehole_layers_table_row()
            {
                no = 3,
                row = new borehole_layers_table()
                {
                    soil_material = clay.no,
                    soil_materialSpecified = true,
                    depth = 1.0,
                    depthSpecified = true,
                    bottom_ordinate = 3.0,
                    bottom_ordinateSpecified = true,
                }
            };




            borehole soilSampleModel = new borehole()
            {
                no = 1,
                comment = "Soil Sample 1",
                type = borehole_type.TYPE_STANDARD,
                typeSpecified = true,
                name = "Soil Sample 1",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                coordinate_0 = 1.0,
                coordinate_0Specified = true,
                coordinate_1 = 2.0,
                coordinate_1Specified = true,
                coordinate_2 = 0.0,
                coordinate_2Specified = true,
                groundwater = true,
                groundwaterSpecified = true,
                groundwater_ordinate = 1.0,
                groundwater_ordinateSpecified = true,
                layers_table = soilSampleLayersTable,
                import_coordinate_z_from_terrain = true,
                import_coordinate_z_from_terrainSpecified = true,
            };
            try
            {
                model.begin_modification("Soil sample");
                model.set_material(gravel);
                model.set_material(sand);
                model.set_material(clay);
                model.set_borehole(soilSampleModel);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                logger.Error(exception, "Something wrong in setting of soil sample\n" + exception.Message + "\n");
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in soil sample\n" + exception.Message + "\n");
                }
            }
            #endregion

            #region Soil massif
            soil_massif soilMassif = new soil_massif()
            {
                no = 1,
                type = soil_massif_type.TYPE_STANDARD,
                typeSpecified = true,
                name = "Soil Massif 1",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                comment = "Scripted massif",
                topology_type = soil_massif_topology_type.TOPOLOGY_TYPE_RECTANGLE,
                topology_typeSpecified = true,
                analysis_type = soil_massif_analysis_type.ANALYSIS_TYPE_ADVANCED_HALF_SPACE_METHOD,
                analysis_typeSpecified = true,
                assigned_to_boreholes = new int[] { soilSampleModel.no },
                assigned_to_type = soil_massif_assigned_to_type.ASSIGNED_TO_TYPE_BOREHOLES,
                assigned_to_typeSpecified = true,
                center_x = 1.0,
                center_xSpecified = true,
                center_y = 1.0,
                center_ySpecified = true,
                size_x = 10.0,
                size_xSpecified = true,
                size_y = 10.0,
                size_ySpecified = true,
                size_z = 6,
                size_zSpecified = true,
                rotation_about_z = 15.0 * Math.PI / 180.0,
                rotation_about_zSpecified = true,
                groundwater = true,
                groundwaterSpecified = true,
                //degree_of_nurbs_surface = 3,
                //degree_of_nurbs_surfaceSpecified = true,
                //degree_of_plate_surface = 3,
                //degree_of_plate_surfaceSpecified = true,
                depth_according_to_boreholes = true,
                depth_according_to_boreholesSpecified = true,
                mapped_mesh_under_surfaces = false,
                mapped_mesh_under_surfacesSpecified = true,
                //max_iterations = 3,
                //max_iterationsSpecified = true,
                //max_pieces = 50,
                //max_piecesSpecified = true,
                //nurbs_surface_geometry_settings_enabled = false,
                //nurbs_surface_geometry_settings_enabledSpecified = true,
                rock_beneath_last_layer = false,
                rock_beneath_last_layerSpecified = true,
            };
            try
            {
                model.begin_modification("Soilmassif 1");
                // model.set_soil_massif(soilMassif); // to long for calculation
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                logger.Error(exception, "Something wrong in setting of soil massif\n" + exception.Message + "\n");
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in soil massif\n" + exception.Message + "\n");
                }
            }
            #endregion

            #region Model parameters
            model_main_parameters model_Main_Parameters = model.get_model_main_parameters();

            Console.WriteLine("Model name: " + model_Main_Parameters.model_name);
            Console.WriteLine("Model path: " + model_Main_Parameters.model_path);
            Console.WriteLine("Model id: " + model_Main_Parameters.model_id);
            Console.WriteLine("Model description: " + model_Main_Parameters.model_description);
            Console.WriteLine("Model comment: " + model_Main_Parameters.model_comment);
            Console.WriteLine("Project name: " + model_Main_Parameters.project_name);
            Console.WriteLine("Project folder: " + model_Main_Parameters.project_folder);
            Console.WriteLine("Project id: " + model_Main_Parameters.project_id);
            Console.WriteLine("Project description: " + model_Main_Parameters.project_description);
            #endregion

            #region Materials, cross_sections and thicknesses
            // create material
            material materialSteel = new material
            {
                no = 1,
                name = "S235 (DIN EN 1993-1-1:2010-12)",
                // name = "Steel",
                material_type = material_material_type.TYPE_STEEL,
                // user_defined = true,
                // user_definedSpecified = true,
                // user_defined_name_enabled = true,
                // user_defined_name_enabledSpecified = true,
            };

            material materialConcrete = new material
            {
                no = 2,
                name = "C20/25 | EN 1992-1-1:2004/A1:2014",
                material_type = material_material_type.TYPE_CONCRETE,
            };

            material materialReinforcementBars = new material
            {
                no = 3,
                name = "B550S(A)",
                material_type = material_material_type.TYPE_REINFORCING_STEEL,
            };

            material materialTimber = new material
            {
                no = 4,
                name = "C24",
                material_type = material_material_type.TYPE_TIMBER,
                material_typeSpecified = true
            };

            material materialAluminum = new material
            {
                no = 5,
                name = "EN AW-3004 H14",
                material_type = material_material_type.TYPE_ALUMINUM,
                material_typeSpecified = true
            };

            // create cross_section
            cross_section sectionSteelCSBeam = new cross_section
            {
                no = 1,
                material = materialSteel.no,
                materialSpecified = true,
                name = "IPE 200",
                typeSpecified = true,
                type = cross_section_type.TYPE_STANDARDIZED_STEEL,
                manufacturing_type = cross_section_manufacturing_type.MANUFACTURING_TYPE_HOT_ROLLED,
                manufacturing_typeSpecified = true,
                thin_walled_model = true,
                thin_walled_modelSpecified = true,
            };


            cross_section sectionRectangle = new cross_section
            {
                no = 2,
                material = materialConcrete.no,
                materialSpecified = true,
                type = cross_section_type.TYPE_PARAMETRIC_MASSIVE_I,
                typeSpecified = true,
                parametrization_type = cross_section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_RECTANGLE__R_M1,
                parametrization_typeSpecified = true,
                name = "R_M1 0.5/1.0", // width/height as in RFEM, SI units
            };

            cross_section sectionSquare = new cross_section
            {
                no = 3,
                material = materialConcrete.no,
                materialSpecified = true,
                type = cross_section_type.TYPE_PARAMETRIC_MASSIVE_I,
                typeSpecified = true,
                parametrization_type = cross_section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_SQUARE__SQ_M1,
                parametrization_typeSpecified = true,
                name = "SQ_M1 0.5", // width as in RFEM

            };

            cross_section sectionSteelCSColumn = new cross_section
            {
                no = 4,
                material = materialSteel.no,
                materialSpecified = true,
                name = "HEB 200",
                typeSpecified = true,
                type = cross_section_type.TYPE_STANDARDIZED_STEEL,
                manufacturing_type = cross_section_manufacturing_type.MANUFACTURING_TYPE_HOT_ROLLED,
                manufacturing_typeSpecified = true,
                thin_walled_model = true,
                thin_walled_modelSpecified = true,
                rotation_angle = 90 * Math.PI / 180,
                rotation_angleSpecified = true,
            };

            cross_section sectionSquareTimber = new cross_section
            {
                no = 5,
                material = materialTimber.no,
                materialSpecified = true,
                type = cross_section_type.TYPE_PARAMETRIC_MASSIVE_I,
                typeSpecified = true,
                parametrization_type = cross_section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_SQUARE__SQ_M1,
                parametrization_typeSpecified = true,
                name = "SQ_M1 0.5", // width as in RFEM

            };
            cross_section sectionAluminium = new cross_section
            {
                no = 6,
                material = materialAluminum.no,
                materialSpecified = true,
                type = cross_section_type.TYPE_PARAMETRIC_THIN_WALLED,
                typeSpecified = true,
                parametrization_type = cross_section_parametrization_type.PARAMETRIC_THIN_WALLED__UNSYMMETRIC_CHANNEL__UU,
                parametrization_typeSpecified = true,
                manufacturing_type = cross_section_manufacturing_type.MANUFACTURING_TYPE_WELDED,
                manufacturing_typeSpecified = true,
                name = "UUESI 0.300/0.150/0.200/0.010/0.014/0.012/0.050/0.025/0.008/0.16/0/0/0/0", // width as in RFEM
            };


            thickness slabThickness = new thickness
            {
                no = 1,
                material = materialConcrete.no,
                materialSpecified = true,
                type = thickness_type.TYPE_UNIFORM,
                typeSpecified = true,
                uniform_thickness = 0.5,
                uniform_thicknessSpecified = true,
            };
            try
            {
                model.begin_modification("Set material and crosssection data");
                model.set_material(materialSteel);
                model.set_material(materialConcrete);
                model.set_material(materialReinforcementBars);
                model.set_material(materialTimber);
                model.set_material(materialAluminum);
                model.set_cross_section(sectionSteelCSBeam);
                model.set_cross_section(sectionRectangle);
                model.set_cross_section(sectionSquare);
                model.set_cross_section(sectionSteelCSColumn);
                model.set_cross_section(sectionSquareTimber);
                model.set_cross_section(sectionAluminium);
                model.set_thickness(slabThickness);
            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something wrong in setting materials and cross_section data\n" + exception.Message + "\n");
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of creation of materials and cross_section data\n" + exception.Message + "\n");

                }
            }
            #endregion


            #region favorite cross_section list
            model.create_my_section_list("MyFavoriteList");
            model.add_section_to_my_section_list("MyFavoriteList", sectionSteelCSBeam.name);
            model.add_section_to_my_section_list("MyFavoriteList", sectionRectangle.name);
            model.add_section_to_my_section_list("MyFavoriteList", sectionSquare.name);
            model.add_section_to_my_section_list("MyFavoriteList", sectionSteelCSColumn.name);

            get_my_section_lists__list[] mySectionsList = model.get_my_section_lists();
            foreach (get_my_section_lists__list mySectionListItem in mySectionsList)
            {
                string[] cross_sections = mySectionListItem.list;
                Console.WriteLine("Favorite list: {0}", mySectionListItem.name);
                foreach (string cross_section in cross_sections)
                {
                    Console.WriteLine("\tSection: {0}", cross_section);
                }
            }
            // based on name
            model.create_section_by_name(16, materialSteel.no, "HEB 200");
            // or based on the name from list - based on the favorite list
            model.create_section_by_name(7, materialSteel.no, mySectionsList[1].list[1]);
            model.delete_section_from_my_section_list("MyFavoriteList", sectionSteelCSColumn.name);
            model.delete_section_from_my_section_list("MyFavoriteList", sectionSteelCSBeam.name);
            model.delete_my_section_list("MyFavoriteList");
            #endregion


            #region Section from RSECTION file
            try
            {
                model.create_section_from_rsection_file(8, ExamplesDirectory + @"\TestRFEM.rsc");
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Something wrong in setting materials and cross_section data\n" + exception.Message + "\n");
                throw;
            }
            #endregion

            #region first free number for id of object
            int materialIDAvaliable = model.get_first_free_number(object_types.E_OBJECT_TYPE_MATERIAL, 0);
            Console.WriteLine("First available ID for material is {0}", materialIDAvaliable.ToString());

            #endregion


            #region Concrete Design input data
            member_concrete_longitudinal_reinforcement_items_row longitudinalReinforcement = new member_concrete_longitudinal_reinforcement_items_row()
            {
                no = 1,
                row = new member_concrete_longitudinal_reinforcement_items()
                {
                    rebar_type = rebar_type.REBAR_TYPE_SYMMETRICAL,
                    rebar_typeSpecified = true,
                    material = materialReinforcementBars.no,
                    materialSpecified = true,
                    bar_count_symmetrical = 3,
                    bar_count_symmetricalSpecified = true,
                    bar_diameter_symmetrical = 0.008,
                    bar_diameter_symmetricalSpecified = true,
                    span_position_reference_type = member_concrete_longitudinal_reinforcement_items_span_position_reference_type.LONGITUDINAL_REINFORCEMENT_ITEM_REFERENCE_START,
                    span_position_reference_typeSpecified = true,
                    span_position_definition_format_type = member_concrete_longitudinal_reinforcement_items_span_position_definition_format_type.LONGITUDINAL_REINFORCEMENT_SPAN_DEFINITION_FORMAT_RELATIVE,
                    span_position_definition_format_typeSpecified = true,
                    span_start_relative = 0.0,
                    span_start_relativeSpecified = true,
                    span_end_relative = 0.75,
                    span_end_relativeSpecified = true,
                    anchorage_start_anchor_type = anchorage_start_anchor_type.ANCHORAGE_TYPE_NONE,
                    anchorage_end_anchor_type = anchorage_end_anchor_type.ANCHORAGE_TYPE_NONE,
                }

            };
            member_concrete_shear_reinforcement_spans_row shearReinforcement = new member_concrete_shear_reinforcement_spans_row()
            {
                no = 1,
                row = new member_concrete_shear_reinforcement_spans()
                {
                    material = materialReinforcementBars.no,
                    stirrup_type = stirrup_type.STIRRUP_TYPE_FOUR_LEGGED_CLOSED_HOOK_135,
                    stirrup_distances = 0.3,
                    stirrup_diameter = 0.08,
                    span_start_relative = 0.0,
                    span_start_relativeSpecified = true,
                    span_end_relative = 0.75,
                    span_end_relativeSpecified = true,
                    span_position_reference_type = span_position_reference_type.SHEAR_REINFORCEMENT_SPAN_REFERENCE_START,
                    span_position_reference_typeSpecified = true,
                    span_position_definition_format_type = span_position_definition_format_type.SHEAR_REINFORCEMENT_SPAN_DEFINITION_FORMAT_RELATIVE,
                    span_position_definition_format_typeSpecified = true,
                }
            };

            design_support design_Support = new design_support()
            {
                no = 1,
                type = design_support_type.DESIGN_SUPPORT_TYPE_CONCRETE,
                typeSpecified = true,
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                name = "Concrete design support scripted",
                comment = " scripted support",
                activate_in_y = true,
                activate_in_ySpecified = true,
                activate_in_z = true,
                activate_in_zSpecified = true,
                consider_in_deflection_design_y = true,
                consider_in_deflection_design_ySpecified = true,
                consider_in_deflection_design_z = true,
                concrete_monolithic_connection_z_enabled = true,
                concrete_monolithic_connection_z_enabledSpecified = true,
                concrete_ratio_of_moment_redistribution_z = 1,
                concrete_ratio_of_moment_redistribution_zSpecified = true,
                design_support_orientation_z = design_support_design_support_orientation_z.DESIGN_SUPPORT_ORIENTATION_ZAXIS_POSITIVE,
                design_support_orientation_zSpecified = true,
                direct_support_z_enabled = true,
                direct_support_z_enabledSpecified = true,
                inner_support_z_enabled = true,
                inner_support_z_enabledSpecified = true,
                support_depth_by_section_width_of_member_z_enabled = true,
                support_depth_by_section_width_of_member_z_enabledSpecified = true,
                support_width_z = 0.3,
                support_width_zSpecified = true,
                support_depth_by_section_width_of_member_y_enabled = true,
                support_depth_by_section_width_of_member_y_enabledSpecified = true,
            };

            concrete_effective_lengths_factors_row factors = new concrete_effective_lengths_factors_row()
            {
                no = 1,
                row = new concrete_effective_lengths_factors()
                {
                    flexural_buckling_y = 1,
                    flexural_buckling_ySpecified = true,
                    flexural_buckling_z = 1,
                    flexural_buckling_zSpecified = true,
                }
            };
            concrete_effective_lengths_nodal_supports_row celNodalSupportsStart = new concrete_effective_lengths_nodal_supports_row()
            {
                no = 1,
                row = new concrete_effective_lengths_nodal_supports()
                {
                    support_type = support_type.SUPPORT_TYPE_FIXED_IN_Z,
                    support_typeSpecified = true,
                    support_in_y_type = support_in_y_type.SUPPORT_STATUS_NO,
                    support_in_y_typeSpecified = true,
                    support_in_z = true,
                    support_in_zSpecified = true,
                    eccentricity_type = eccentricity_type.ECCENTRICITY_TYPE_NONE,
                    eccentricity_typeSpecified = true,
                    eccentricity_ez = 0,
                    eccentricity_ezSpecified = true,
                    restraint_about_x_type = restraint_about_x_type.SUPPORT_STATUS_NO,
                    restraint_about_x_typeSpecified = true,
                    restraint_about_z_type = restraint_about_z_type.SUPPORT_STATUS_NO,
                    restraint_about_z_typeSpecified = true,
                    restraint_spring_about_x = 0,
                    restraint_spring_about_xSpecified = true,
                    restraint_spring_about_z = 0,
                    restraint_spring_about_zSpecified = true,
                    restraint_spring_warping = 0,
                    restraint_spring_warpingSpecified = true,
                    restraint_warping_type = restraint_warping_type.SUPPORT_STATUS_NO,
                    restraint_warping_typeSpecified = true,
                    support_spring_in_y = 0,
                    support_spring_in_ySpecified = true,
                }
            };
            concrete_effective_lengths_nodal_supports_row celNodalSupportsEnd = new concrete_effective_lengths_nodal_supports_row()
            {
                no = 2,
                row = new concrete_effective_lengths_nodal_supports()
                {
                    support_type = support_type.SUPPORT_TYPE_FIXED_ALL,
                    support_typeSpecified = true,
                    support_in_y_type = support_in_y_type.SUPPORT_STATUS_YES,
                    support_in_y_typeSpecified = true,
                    support_in_z = true,
                    support_in_zSpecified = true,
                    eccentricity_type = eccentricity_type.ECCENTRICITY_TYPE_NONE,
                    eccentricity_typeSpecified = true,
                    eccentricity_ez = 0,
                    eccentricity_ezSpecified = true,
                    restraint_about_x_type = restraint_about_x_type.SUPPORT_STATUS_NO,
                    restraint_about_x_typeSpecified = true,
                    restraint_about_z_type = restraint_about_z_type.SUPPORT_STATUS_NO,
                    restraint_about_z_typeSpecified = true,
                    restraint_spring_about_x = 0,
                    restraint_spring_about_xSpecified = true,
                    restraint_spring_about_z = 0,
                    restraint_spring_about_zSpecified = true,
                    restraint_spring_warping = 0,
                    restraint_spring_warpingSpecified = true,
                    restraint_warping_type = restraint_warping_type.SUPPORT_STATUS_NO,
                    restraint_warping_typeSpecified = true,
                    support_spring_in_y = 0,
                    support_spring_in_ySpecified = true,
                }
            };
            concrete_effective_lengths concrete_Effective_Lengths = new concrete_effective_lengths()
            {
                no = 2,
                flexural_buckling_about_y = true,
                flexural_buckling_about_ySpecified = true,
                flexural_buckling_about_z = true,
                flexural_buckling_about_zSpecified = true,
                structure_type_about_axis_y = concrete_effective_lengths_structure_type_about_axis_y.STRUCTURE_TYPE_UNBRACED,
                structure_type_about_axis_ySpecified = true,
                structure_type_about_axis_z = concrete_effective_lengths_structure_type_about_axis_z.STRUCTURE_TYPE_UNBRACED,
                nodal_supports = new concrete_effective_lengths_nodal_supports_row[] { celNodalSupportsStart, celNodalSupportsEnd },
                factors = new concrete_effective_lengths_factors_row[] { factors },
                different_properties = true,
                different_propertiesSpecified = true,
                intermediate_nodes = false,
                intermediate_nodesSpecified = true,


            };


            concrete_durability concrete_Durability = new concrete_durability()
            {
                no = 1,
                corrosion_induced_by_carbonation_enabled = true,
                corrosion_induced_by_carbonation_enabledSpecified = true,
                corrosion_induced_by_carbonation = concrete_durability_corrosion_induced_by_carbonation.CORROSION_INDUCED_BY_CARBONATION_TYPE_DRY_OR_PERMANENTLY_WET,
                corrosion_induced_by_carbonationSpecified = true,
                structural_class_type = concrete_durability_structural_class_type.STANDARD,
                increase_design_working_life_from_50_to_100_years_enabled = false,
                increase_design_working_life_from_50_to_100_years_enabledSpecified = true,
                position_of_reinforcement_not_affected_by_construction_process_enabled = false,
                position_of_reinforcement_not_affected_by_construction_process_enabledSpecified = true,
                special_quality_control_of_production_enabled = false,
                special_quality_control_of_production_enabledSpecified = true,
                air_entrainment_of_more_than_4_percent_enabled = false,
                air_entrainment_of_more_than_4_percent_enabledSpecified = true,
                additional_protection_enabled = false,
                additional_protection_enabledSpecified = true,
                allowance_of_deviation_type = concrete_durability_allowance_of_deviation_type.STANDARD,
                allowance_of_deviation_typeSpecified = true,

            };

            //reinforcement_direction Reinforcement_Direction = model.get_reinforcement_direction(1);
            reinforcement_direction Reinforcement_Direction = new reinforcement_direction()
            {
                no = 1,
                reinforcement_direction_type = reinforcement_direction_reinforcement_direction_type.REINFORCEMENT_DIRECTION_TYPE_FIRST_REINFORCEMENT_IN_X,
                reinforcement_direction_typeSpecified = true,

            };

            // surface_reinforcement Surface_Reinforcement = model.get_surface_reinforcement(1);
            surface_reinforcement Surface_Reinforcement = new surface_reinforcement()
            {
                no = 1,
                location_type = surface_reinforcement_location_type.LOCATION_TYPE_ON_SURFACE,
                location_typeSpecified = true,
                material = materialReinforcementBars.no,
                materialSpecified = true,
                rebar_diameter = 0.01,
                rebar_diameterSpecified = true,
                rebar_spacing = 0.15,
                rebar_spacingSpecified = true,
                //additional_transverse_reinforcement_enabled = false,
                //additional_transverse_reinforcement_enabledSpecified = true,
                additional_offset_to_concrete_cover_top = 0.0,
                additional_offset_to_concrete_cover_topSpecified = true,
                additional_offset_to_concrete_cover_bottom = 0.0,
                additional_offset_to_concrete_cover_bottomSpecified = true,
                alignment_bottom_enabled = true,
                alignment_bottom_enabledSpecified = true,
                alignment_top_enabled = false,
                alignment_top_enabledSpecified = true,
                reinforcement_direction_type = surface_reinforcement_reinforcement_direction_type.REINFORCEMENT_DIRECTION_TYPE_IN_DESIGN_REINFORCEMENT_DIRECTION,
                reinforcement_direction_typeSpecified = true,
                design_reinforcement_direction = surface_reinforcement_design_reinforcement_direction.DESIGN_REINFORCEMENT_DIRECTION_A_S_1,
                design_reinforcement_directionSpecified = true,
            };

            surface_reinforcement Surface_ReinforcementMesh = new surface_reinforcement()
            {
                no = 2,
                location_type = surface_reinforcement_location_type.LOCATION_TYPE_ON_SURFACE,
                location_typeSpecified = true,
                material = materialReinforcementBars.no,
                materialSpecified = true,
                //reinforcement_type = surface_reinforcement_reinforcement_type.REINFORCEMENT_TYPE_MESH,
                //reinforcement_typeSpecified = true,
                mesh_name = "Q188A",
                mesh_product_range = surface_reinforcement_mesh_product_range.MESHSTANDARD_GERMANY_2008_01_01,
                mesh_shape = surface_reinforcement_mesh_shape.MESHSHAPE_Q_MESH,
                additional_offset_to_concrete_cover_top = 0.0,
                additional_offset_to_concrete_cover_topSpecified = true,
                additional_offset_to_concrete_cover_bottom = 0.0,
                additional_offset_to_concrete_cover_bottomSpecified = true,
                alignment_bottom_enabled = false,
                alignment_bottom_enabledSpecified = true,
                alignment_top_enabled = true,
                alignment_top_enabledSpecified = true,
                reinforcement_direction_type = surface_reinforcement_reinforcement_direction_type.REINFORCEMENT_DIRECTION_TYPE_IN_DESIGN_REINFORCEMENT_DIRECTION,
                reinforcement_direction_typeSpecified = true,
                design_reinforcement_direction = surface_reinforcement_design_reinforcement_direction.DESIGN_REINFORCEMENT_DIRECTION_A_S_1,
                design_reinforcement_directionSpecified = true,
            };

            try
            {
                model.begin_modification("Set concrete design input data");
                model.set_design_support(design_Support);
                model.set_concrete_effective_lengths(concrete_Effective_Lengths);
                model.set_concrete_durability(concrete_Durability);
                model.set_reinforcement_direction(Reinforcement_Direction);
                model.set_surface_reinforcement(Surface_Reinforcement);
                model.set_surface_reinforcement(Surface_ReinforcementMesh);

            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something wrong in setting concrete design input data\n" + exception.Message + "\n");
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of creation of concrete design input data\n" + exception.Message + "\n");

                }
            }
            #endregion

            #region design configurations
            concrete_design_uls_configuration concreteULSConfiguration = new concrete_design_uls_configuration()
            {
                no = 2,
                name = "ScriptedULSConfiguration",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                assigned_to_all_members = true,
                assigned_to_all_membersSpecified = true,
                assigned_to_all_surfaces = true,
                assigned_to_all_surfacesSpecified = true,

            };
            concrete_design_sls_configuration concreteSLSConfiguration = new concrete_design_sls_configuration()
            {
                no = 2,
                name = "ScriptedSLSConfiguration",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                assigned_to_all_members = true,
                assigned_to_all_membersSpecified = true,
                assigned_to_all_surfaces = true,
                assigned_to_all_surfacesSpecified = true,

            };
            try
            {
                model.begin_modification("Set concrete design configurations");
                model.set_concrete_design_uls_configuration(concreteULSConfiguration);
                model.set_concrete_design_sls_configuration(concreteSLSConfiguration);
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Something wrong in setting concrete design configurations\n" + exception.Message + "\n");
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of creation of concrete design configurations\n" + exception.Message + "\n");

                }
            }
            #endregion

            #region Geometry definition
            // number of bays in x direction
            int nx = 5;
            // span in x direction
            double lx = 3.0;
            // number of bays in y direction
            int ny = 1;
            // span in y direction
            double ly = 4;
            // height of concrete floor
            double hc = 3.6;
            // height of steel part
            double hs = 4.0;

            #region nodes
            SortedList<int, node> nodes = new();
            int nodeID = model.get_first_free_number(object_types.E_OBJECT_TYPE_NODE, 0);
            int FirstNodeID = nodeID;
            for (int i = 0; i <= ny; i++)
            {
                for (int j = 0; j <= nx; j++)
                {
                    double x = j * lx;
                    double y = i * ly;
                    double z = 0.0;
                    node n = new() // creates node class
                    {
                        no = nodeID,
                        coordinates = new vector_3d() { x = x, y = y, z = z },
                        coordinate_system_type = node_coordinate_system_type.COORDINATE_SYSTEM_CARTESIAN,
                        coordinate_system_typeSpecified = true,
                        comment = "concrete part"
                    };
                    nodes.Add(nodeID, n);
                    nodeID++;
                    z = -1 * hc; // z downward
                    node nhc = new()
                    {
                        no = nodeID,
                        coordinates = new vector_3d() { x = x, y = y, z = z },
                        coordinate_system_type = node_coordinate_system_type.COORDINATE_SYSTEM_CARTESIAN,
                        coordinate_system_typeSpecified = true,
                        comment = "concrete top + steel bottom"
                    };
                    nodes.Add(nodeID, nhc);
                    nodeID++;
                    z = -1 * hc - hs; // z downward
                    node ns = new()
                    {
                        no = nodeID,
                        coordinates = new vector_3d() { x = x, y = y, z = z },
                        coordinate_system_type = node_coordinate_system_type.COORDINATE_SYSTEM_CARTESIAN,
                        coordinate_system_typeSpecified = true,
                        comment = "steel top"
                    };
                    nodes.Add(nodeID, ns);
                    nodeID++;
                }
            }
            #endregion


            #region punching reinforcement
            punching_reinforcement punchingReinforcement = new punching_reinforcement()
            {
                no = 1,
                type = punching_reinforcement_type.TYPE_VERTICAL,
                typeSpecified = true,
                name = "Punching reinforcement",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                placement_type = punching_reinforcement_placement_type.PLACEMENT_TYPE_AXIAL,
                nodes = new int[] { FirstNodeID + 22 },
                material = materialReinforcementBars.no,
                materialSpecified = true,
                bend_up_diameter = 0.01,
                bend_up_diameterSpecified = true,
                perimeter_area = 0,
                perimeter_areaSpecified = true,
                total_area = 0,
                total_areaSpecified = true,
            };

            try
            {
                model.begin_modification("Set concrete punching");
                model.set_punching_reinforcement(punchingReinforcement);

            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something wrong in setting concrete design input data\n" + exception.Message + "\n");
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of creation of concrete design input data\n" + exception.Message + "\n");

                }
            }
            #endregion

            #region lines & members
            SortedList<int, line> lines = new();
            SortedList<int, member> members = new();
            int lineID = model.get_first_free_number(object_types.E_OBJECT_TYPE_LINE, 0);
            int nodeStart = FirstNodeID;
            int memberID = model.get_first_free_number(object_types.E_OBJECT_TYPE_MEMBER, 0);

            for (int i = 0; i <= ny; i++)
            {
                for (int j = 0; j <= nx; j++)
                {
                    int indexStart = nodes[nodeStart].no;
                    int indexEnd = nodes[nodeStart + 1].no;
                    line lineC = new()
                    {
                        no = lineID,
                        definition_nodes = new int[] { indexStart, indexEnd },
                        comment = "lines for columns",
                    };
                    lines.Add(lineID, lineC);
                    lineID++;
                    // create member
                    member memberC = new()
                    {
                        no = memberID,
                        line = lineC.no,
                        lineSpecified = true,
                        cross_section_start = sectionSquare.no,
                        cross_section_startSpecified = true,
                        cross_section_end = sectionSquare.no,
                        cross_section_endSpecified = true,
                        comment = "concrete column",
                        concrete_durability = concrete_Durability.no,
                        concrete_durabilitySpecified = true,
                        concrete_longitudinal_reinforcement_items = new member_concrete_longitudinal_reinforcement_items_row[] { longitudinalReinforcement },
                        concrete_shear_reinforcement_spans = new member_concrete_shear_reinforcement_spans_row[] { shearReinforcement },
                        concrete_effective_lengths = concrete_Effective_Lengths.no,
                        concrete_effective_lengthsSpecified = true,
                        deflection_check_direction = member_deflection_check_direction.DEFLECTION_CHECK_DIRECTION_LOCAL_AXIS_Z_AND_Y,
                        deflection_check_directionSpecified = true,
                        deflection_check_displacement_reference = member_deflection_check_displacement_reference.DEFLECTION_CHECK_DISPLACEMENT_REFERENCE_DEFORMED_UNDEFORMED_SYSTEM,
                        deflection_check_displacement_referenceSpecified = true,
                        design_support_on_member_start = design_Support.no,
                        design_support_on_member_startSpecified = true,
                        design_support_on_member_end = design_Support.no,
                        design_support_on_member_endSpecified = true,
                    };
                    members.Add(memberID, memberC);
                    memberID++;
                    indexStart = indexEnd;
                    indexEnd = indexStart + 1;
                    line lineS = new()
                    {
                        no = lineID,
                        definition_nodes = new int[] { indexStart, indexEnd },
                        comment = "lines for columns",
                    };
                    lines.Add(lineID, lineS);
                    lineID++;
                    nodeStart += 3;
                    member memberS = new()
                    {
                        no = memberID,
                        line = lineS.no,
                        lineSpecified = true,
                        cross_section_start = sectionSteelCSColumn.no,
                        cross_section_startSpecified = true,
                        cross_section_end = sectionSteelCSColumn.no,
                        cross_section_endSpecified = true,
                        comment = "steel column",
                    };
                    members.Add(memberID, memberS);
                    memberID++;
                }
            }
            nodeStart = FirstNodeID + 1;
            for (int i = 0; i < ny; i++)
            {
                for (int j = 0; j <= nx; j++)
                {
                    int indexStart = nodes[nodeStart].no;
                    int indexEnd = nodes[nodeStart + (nx + 1) * 3].no;
                    line lineC = new()
                    {
                        no = lineID,
                        definition_nodes = new int[] { indexStart, indexEnd },
                        comment = "lines for beams",
                        type = line_type.TYPE_POLYLINE,
                        typeSpecified = true,
                    };
                    lines.Add(lineID, lineC);
                    lineID++;
                    // create member
                    member memberC = new()
                    {
                        no = memberID,
                        line = lineC.no,
                        lineSpecified = true,
                        cross_section_start = sectionRectangle.no,
                        cross_section_startSpecified = true,
                        cross_section_end = sectionSquare.no,
                        cross_section_endSpecified = true,
                        comment = "concrete beam",
                        concrete_durability = concrete_Durability.no,
                        concrete_durabilitySpecified = true,
                        concrete_longitudinal_reinforcement_items = new member_concrete_longitudinal_reinforcement_items_row[] { longitudinalReinforcement },
                        concrete_shear_reinforcement_spans = new member_concrete_shear_reinforcement_spans_row[] { shearReinforcement },
                        concrete_effective_lengths = concrete_Effective_Lengths.no,
                        concrete_effective_lengthsSpecified = true,
                        deflection_check_direction = member_deflection_check_direction.DEFLECTION_CHECK_DIRECTION_LOCAL_AXIS_Z_AND_Y,
                        deflection_check_directionSpecified = true,
                        deflection_check_displacement_reference = member_deflection_check_displacement_reference.DEFLECTION_CHECK_DISPLACEMENT_REFERENCE_DEFORMED_UNDEFORMED_SYSTEM,
                        deflection_check_displacement_referenceSpecified = true,
                        deflection_segments_defined_length_y_axis_enabled = false,
                        deflection_segments_defined_length_y_axis_enabledSpecified = true,
                        deflection_segments_defined_length_z_axis_enabled = false,
                        deflection_segments_defined_length_z_axis_enabledSpecified = true,
                        deflection_segments_y_axis = new member_deflection_segments_y_axis_row[]{
                            new member_deflection_segments_y_axis_row(){
                                no = 1,
                                row = new member_deflection_segments_y_axis(){
                                    active = true,
                                    activeSpecified = true,
                                    precamber = 0.0,
                                    precamberSpecified = true,
                                }
                            }
                        },
                        deflection_segments_z_axis = new member_deflection_segments_z_axis_row[]{
                            new member_deflection_segments_z_axis_row(){
                                no = 1,
                                row = new member_deflection_segments_z_axis(){
                                    active = true,
                                    activeSpecified = true,
                                    precamber = 0.0,
                                    precamberSpecified = true,
                                }
                            }
                        },
                        //design support
                        design_support_on_member_start = design_Support.no,
                        design_support_on_member_startSpecified = true,
                        design_support_on_member_end = design_Support.no,
                        design_support_on_member_endSpecified = true,

                    };
                    members.Add(memberID, memberC);
                    memberID++;
                    indexStart = nodes[nodeStart + 1].no;
                    indexEnd = nodes[nodeStart + 1 + (nx + 1) * 3].no;
                    line lineS = new()
                    {
                        no = lineID,
                        definition_nodes = new int[] { indexStart, indexEnd },
                        comment = "lines for beams",
                        type = line_type.TYPE_POLYLINE,
                        typeSpecified = true,
                    };
                    lines.Add(lineID, lineS);
                    lineID++;
                    member memberS = new()
                    {
                        no = memberID,
                        line = lineS.no,
                        lineSpecified = true,
                        cross_section_start = sectionSteelCSBeam.no,
                        cross_section_startSpecified = true,
                        cross_section_end = sectionSteelCSBeam.no,
                        cross_section_endSpecified = true,
                        comment = "steel beam",


                    };
                    members.Add(memberID, memberS);
                    memberID++;

                    nodeStart += 3;
                }
            }

            #region Servis category
            timber_service_class serviceClass = new timber_service_class()
            {
                no = 1,
                name = "Service class dry - scripted",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                service_class = timber_service_class_service_class.TIMBER_SERVICE_CLASS_TYPE_2,
                service_classSpecified = true,
            };
            try
            {
                model.begin_modification("Set timber class");
                model.set_timber_service_class(serviceClass);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                logger.Error(exception, "Something wrong in Set timber class\n" + exception.Message + "\n");
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong inSet timber class \n" + exception.Message + "\n");
                }
            }
            #endregion

            #region Shear panel
            member_shear_panel timberShearPanel = new member_shear_panel()
            {
                no = 1,
                name = "Timber shear panel",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                definition_type = member_shear_panel_definition_type.DEFINITION_TYPE_DEFINE_S_PROV,
                definition_typeSpecified = true,
                stiffness = 1000,
                stiffnessSpecified = true,
            };
            try
            {
                model.begin_modification("Set timber shear panel");
                model.set_member_shear_panel(timberShearPanel);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                logger.Error(exception, "Something wrong in timber shear panel\n" + exception.Message + "\n");
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in timber shear panel \n" + exception.Message + "\n");
                }
            }
            #endregion

            #region Timber member rotational restrain
            member_rotational_restraint timberRotationalRestrain = new member_rotational_restraint()
            {
                no = 1,
                name = "Timber rotational restrain",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                type = member_rotational_restraint_type.TYPE_MANUALLY,
                typeSpecified = true,
                total_rotational_spring_stiffness = 1000,
                total_rotational_spring_stiffnessSpecified = true,
            };
            try
            {
                model.begin_modification("Set timber shear panel");
                model.set_member_rotational_restraint(timberRotationalRestrain);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                logger.Error(exception, "Something wrong in timber shear panel\n" + exception.Message + "\n");
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in timber shear panel \n" + exception.Message + "\n");
                }
            }
            #endregion

            #region Timber local reduction
            timber_member_local_cross_section_reduction_components_row timberSectionReductionComponent = new timber_member_local_cross_section_reduction_components_row()
            {
                no = 1,
                row = new timber_member_local_cross_section_reduction_components()
                {
                    note = "Scripted local reduction",
                    reduction_type = timber_member_local_cross_section_reduction_components_reduction_type.REDUCTION_COMPONENT_TYPE_END_NOTCH,
                    reduction_typeSpecified = true,
                    position = 1.5,
                    positionSpecified = true,
                    multiple = false,
                    multipleSpecified = true,
                    orientation_type = orientation_type.E_ORIENTATION_DEPTH,
                    orientation_typeSpecified = true,
                    // z_axis_reference_type = z_axis_reference_type.E_POSITION_REFERENCE_TOP,
                    // z_axis_reference_typeSpecified = true,
                    depth = 0.15,
                    depthSpecified = true,
                    length = 0.4,
                    lengthSpecified = true,
                }
            };
            timber_member_local_cross_section_reduction timberLocalReduction = new timber_member_local_cross_section_reduction()
            {
                no = 1,
                name = "Timber local reduction",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                components = new timber_member_local_cross_section_reduction_components_row[] { timberSectionReductionComponent },
            };
            try
            {
                model.begin_modification("Set timber local reduction");
                model.set_timber_member_local_cross_section_reduction(timberLocalReduction);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                logger.Error(exception, "Something wrong in timber local reduction\n" + exception.Message + "\n");
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in local reduction \n" + exception.Message + "\n");
                }
            }
            #endregion

            #region Timber effective lengths
            timber_effective_lengths_nodal_supports_row timberNodalSupportsStart = new timber_effective_lengths_nodal_supports_row()
            {
                no = 1,
                row = new timber_effective_lengths_nodal_supports()
                {
                    support_type = support_type.SUPPORT_TYPE_FIXED_IN_Z_Y_AND_TORSION,
                    support_typeSpecified = true,
                    support_in_z = true,
                    support_in_zSpecified = true,
                    eccentricity_type = eccentricity_type.ECCENTRICITY_TYPE_NONE,
                    eccentricity_typeSpecified = true,
                    support_in_y_type = support_in_y_type.SUPPORT_STATUS_YES,
                    support_in_y_typeSpecified = true,
                    restraint_about_x_type = restraint_about_x_type.SUPPORT_STATUS_YES,
                    nodes = new int[] { 21 },
                }
            };
            timber_effective_lengths_nodal_supports_row timberNodalSupportsEnd = new timber_effective_lengths_nodal_supports_row()
            {
                no = 2,
                row = new timber_effective_lengths_nodal_supports()
                {
                    support_type = support_type.SUPPORT_TYPE_FIXED_IN_Z_Y_AND_TORSION,
                    support_typeSpecified = true,
                    support_in_z = true,
                    support_in_zSpecified = true,
                    eccentricity_type = eccentricity_type.ECCENTRICITY_TYPE_NONE,
                    eccentricity_typeSpecified = true,
                    support_in_y_type = support_in_y_type.SUPPORT_STATUS_YES,
                    support_in_y_typeSpecified = true,
                    restraint_about_x_type = restraint_about_x_type.SUPPORT_STATUS_SPRING,
                    nodes = new int[] { 36 },

                }
            };
            timber_effective_lengths_factors_row timberEffectiveLengthsFactors = new timber_effective_lengths_factors_row()
            {
                no = 1,
                row = new timber_effective_lengths_factors()
                {
                    flexural_buckling_u = 1,
                    flexural_buckling_uSpecified = true,
                    flexural_buckling_v = 1,
                    flexural_buckling_vSpecified = true,
                    lateral_torsional_buckling = 1,
                    lateral_torsional_bucklingSpecified = true,
                }
            };
            timber_effective_lengths timberEffectiveLengths = new timber_effective_lengths()
            {
                no = 1,
                name = "Timber effective lengths",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                comment = "Scripted effective lengths",
                factors = new timber_effective_lengths_factors_row[] { timberEffectiveLengthsFactors },
                nodal_supports = new timber_effective_lengths_nodal_supports_row[] { timberNodalSupportsStart, timberNodalSupportsEnd },
                members = new int[] { 38 },
                flexural_buckling_about_y = true,
                flexural_buckling_about_ySpecified = true,
                flexural_buckling_about_z = true,
                flexural_buckling_about_zSpecified = true,
                lateral_torsional_buckling = true,
                lateral_torsional_bucklingSpecified = true,
                intermediate_nodes = false,
                intermediate_nodesSpecified = true,
                different_properties = true,
                different_propertiesSpecified = true,
                factors_definition_absolute = false,
                factors_definition_absoluteSpecified = true,
                fire_design_different_buckling_factors = false,
                fire_design_different_buckling_factorsSpecified = true,
                import_from_stability_analysis_enabled = false,
                import_from_stability_analysis_enabledSpecified = true,
                determination_type = timber_effective_lengths_determination_type.DETERMINATION_ANALYTICAL,
                determination_typeSpecified = true,
            };
            try
            {
                model.begin_modification("Set Timber effective lengths");
                model.set_timber_effective_lengths(timberEffectiveLengths);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                logger.Error(exception, "Something wrong in Timber effective lengths\n" + exception.Message + "\n");
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in Timber effective lengths \n" + exception.Message + "\n");
                }
            }
            #endregion

            #region  timber design configurations
            timber_design_uls_configuration timberDesignUlsConfiguration = new timber_design_uls_configuration()
            {
                no = 2,
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                name = "timber ULS config",
                comment = "Scripted configuration",
                assigned_to_all_members = true,
                assigned_to_all_membersSpecified = true,
            };
            timber_design_sls_configuration timberDesignSlsConfiguration = new timber_design_sls_configuration()
            {
                no = 2,
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                name = "timber SLS config",
                comment = "Scripted configuration",
                assigned_to_all_members = true,
                assigned_to_all_membersSpecified = true,
            };
            timber_design_fr_configuration timberDesignFrConfiguration = new timber_design_fr_configuration()
            {
                no = 2,
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                name = "timber FR config",
                comment = "Scripted configuration",
                assigned_to_all_members = true,
                assigned_to_all_membersSpecified = true,
            };
            try
            {
                model.begin_modification("Set  timber configurations");
                model.set_timber_design_uls_configuration(timberDesignUlsConfiguration);
                model.set_timber_design_sls_configuration(timberDesignSlsConfiguration);
                model.set_timber_design_fr_configuration(timberDesignFrConfiguration);
            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something happen when creation of  timber_member_design configurations: " + exception.Message);
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of  steel_member_design configurations\n" + exception.Message + "\n");

                }
            }

            #endregion

            var firstNodeT = from n in nodes where n.Value.coordinates.x == 0.0 && n.Value.coordinates.y == 0.0 && n.Value.coordinates.z == -hc - hs select n.Value;
            var SecondNode = from n in nodes where n.Value.coordinates.x == 0.0 && n.Value.coordinates.y == ny * ly && n.Value.coordinates.z == -hc - hs select n.Value;
            var ThirdNode = from n in nodes where n.Value.coordinates.x == nx * lx && n.Value.coordinates.y == ny * ly && n.Value.coordinates.z == -hc - hs select n.Value;
            var FourthNode = from n in nodes where n.Value.coordinates.x == nx * lx && n.Value.coordinates.y == 0.0 && n.Value.coordinates.z == -hc - hs select n.Value;
            var FifthNode = from n in nodes where n.Value.coordinates.x == nx && n.Value.coordinates.y == ny && n.Value.coordinates.z == -hc - hs select n.Value;
            line lineTimberOne = new()
            {
                no = lineID,
                definition_nodes = new int[] { firstNodeT.FirstOrDefault().no, FourthNode.FirstOrDefault().no },
                comment = "lines for timber beam",
                type = line_type.TYPE_POLYLINE,
                typeSpecified = true,
            };
            lines.Add(lineID, lineTimberOne);
            lineID++;
            line lineTimberTwo = new()
            {
                no = lineID,
                definition_nodes = new int[] { SecondNode.FirstOrDefault().no, ThirdNode.FirstOrDefault().no },
                comment = "lines for timber beam",
                type = line_type.TYPE_POLYLINE,
                typeSpecified = true,
            };
            lines.Add(lineID, lineTimberTwo);
            lineID++;
            line lineAluminum = new()
            {
                no = lineID,
                definition_nodes = new int[] { FirstNodeID + 2, FirstNodeID + 23 },
                comment = "lines for aluminum beam",
                type = line_type.TYPE_POLYLINE,
                typeSpecified = true,
            };
            lines.Add(lineID, lineAluminum);
            lineID++;
            // create member
            member memberTimberOne = new()
            {
                no = memberID,
                line = lineTimberOne.no,
                lineSpecified = true,
                cross_section_start = sectionSquareTimber.no,
                cross_section_startSpecified = true,
                cross_section_end = sectionSquare.no,
                cross_section_endSpecified = true,
                comment = "timber beam",
                timber_service_class = serviceClass.no,
                timber_service_classSpecified = true,
            };
            members.Add(memberID, memberTimberOne);
            memberID++;
            member memberTimberTwo = new()
            {
                no = memberID,
                line = lineTimberTwo.no,
                lineSpecified = true,
                cross_section_start = sectionSquareTimber.no,
                cross_section_startSpecified = true,
                cross_section_end = sectionSquare.no,
                cross_section_endSpecified = true,
                comment = "timber beam",
                timber_service_class = serviceClass.no,
                timber_service_classSpecified = true,

            };
            members.Add(memberID, memberTimberTwo);
            memberID++;
            member memberAluminum = new()
            {
                no = memberID,
                line = lineAluminum.no,
                lineSpecified = true,
                cross_section_start = sectionAluminium.no,
                cross_section_startSpecified = true,
                cross_section_end = sectionSquare.no,
                cross_section_endSpecified = true,
                comment = "Aluminium beam",
            };
            members.Add(memberID, memberAluminum);
            memberID++;
            #endregion
            var firstNode = from n in nodes where n.Value.coordinates.x == 0.0 && n.Value.coordinates.y == 0.0 && n.Value.coordinates.z == -hc select n.Value;
            var YNode = from n in nodes where n.Value.coordinates.x == 0.0 && n.Value.coordinates.y == ny * ly && n.Value.coordinates.z == -hc select n.Value;
            var XYNode = from n in nodes where n.Value.coordinates.x == nx * lx && n.Value.coordinates.y == ny * ly && n.Value.coordinates.z == -hc select n.Value;
            var XNode = from n in nodes where n.Value.coordinates.x == nx * lx && n.Value.coordinates.y == 0.0 && n.Value.coordinates.z == -hc select n.Value;

            line slabLine = new()
            {
                no = lineID,
                definition_nodes = new int[] { firstNode.FirstOrDefault().no, YNode.FirstOrDefault().no, XYNode.FirstOrDefault().no, XNode.FirstOrDefault().no, firstNode.FirstOrDefault().no },
                type = line_type.TYPE_POLYLINE,
                typeSpecified = true,
                comment = "closed line for slab",

            };
            lines.Add(lineID, slabLine);


            // surface
            surface concreteSlab = new()
            {
                no = model.get_first_free_number(object_types.E_OBJECT_TYPE_SURFACE, 0),
                material = materialConcrete.no,
                materialSpecified = true,
                thickness = slabThickness.no,
                boundary_lines = new int[] { slabLine.no },
                type = surface_type.TYPE_STANDARD,
                typeSpecified = true,
                geometry = surface_geometry.GEOMETRY_PLANE,
                geometrySpecified = true,
                concrete_durability_top = concrete_Durability.no,
                concrete_durability_topSpecified = true,
                concrete_durability_bottom = concrete_Durability.no,
                concrete_durability_bottomSpecified = true,
                reinforcement_direction_top = Reinforcement_Direction.no,
                reinforcement_direction_topSpecified = true,
                reinforcement_direction_bottom = Reinforcement_Direction.no,
                reinforcement_direction_bottomSpecified = true,
                surface_reinforcements = new int[] { Surface_Reinforcement.no, Surface_ReinforcementMesh.no },
                //surface_concrete_design_sls_configuration = 1,
                //surface_concrete_design_sls_configurationSpecified = true,
                //surface_concrete_design_uls_configuration = 1,
                //surface_concrete_design_uls_configurationSpecified = true,
                deflection_check_surface_type = surface_deflection_check_surface_type.DEFLECTION_CHECK_SURFACE_TYPE_DOUBLE_SUPPORTED,
                deflection_check_surface_typeSpecified = true,
                deflection_check_displacement_reference = surface_deflection_check_displacement_reference.DEFLECTION_CHECK_DISPLACEMENT_REFERENCE_DEFORMED_USER_DEFINED_REFERENCE_PLANE,
                deflection_check_reference_length_z_definition_type = surface_deflection_check_reference_length_z_definition_type.DEFLECTION_CHECK_REFERENCE_LENGTH_DEFINITION_TYPE_BY_MAXIMUM_BOUNDARY_LINE,
                deflection_check_reference_length_z_definition_typeSpecified = true,
            };
            // opening
            node openingNodeOne = new node()
            {
                no = nodeID,
                coordinates = new vector_3d()
                {
                    x = firstNode.FirstOrDefault().coordinate_1 + 1.0,
                    y = firstNode.FirstOrDefault().coordinate_1 + 1.0,
                    z = -hc,
                },
                comment = "opening node",

            };
            nodes.Add(nodeID, openingNodeOne);
            nodeID++;
            node openingNodeTwo = new node()
            {
                no = nodeID,
                coordinates = new vector_3d()
                {
                    x = firstNode.FirstOrDefault().coordinate_1 + 1.0,
                    y = firstNode.FirstOrDefault().coordinate_1 + 2.0,
                    z = -hc,
                },
                comment = "opening node",

            };
            nodes.Add(nodeID, openingNodeTwo);
            nodeID++;
            node openingNodeThree = new node()
            {
                no = nodeID,
                coordinates = new vector_3d()
                {
                    x = firstNode.FirstOrDefault().coordinate_1 + 2.0,
                    y = firstNode.FirstOrDefault().coordinate_1 + 2.0,
                    z = -hc,
                },
                comment = "opening node",

            };
            nodes.Add(nodeID, openingNodeThree);
            nodeID++;
            node openingNodeFour = new node()
            {
                no = nodeID,
                coordinates = new vector_3d()
                {
                    x = firstNode.FirstOrDefault().coordinate_1 + 2.0,
                    y = firstNode.FirstOrDefault().coordinate_1 + 1.0,
                    z = -hc,
                },
                comment = "opening node",

            };
            nodes.Add(nodeID, openingNodeFour);
            nodeID++;
            lineID++;
            line openingLine = new()
            {
                no = lineID,
                definition_nodes = new int[] { openingNodeOne.no, openingNodeTwo.no, openingNodeThree.no, openingNodeFour.no, openingNodeOne.no },
                type = line_type.TYPE_POLYLINE,
                typeSpecified = true,
                comment = "closed line for slab",

            };
            lines.Add(lineID, openingLine);

            opening slabOpening = new opening()
            {
                no = 1,
                boundary_lines = new int[] { openingLine.no },
                surfaces = new int[] { concreteSlab.no },
            };

            try
            {
                model.begin_modification("Set geometry model data");
                foreach (KeyValuePair<int, node> nodeItem in nodes)
                {
                    model.set_node(nodeItem.Value);
                }
                foreach (KeyValuePair<int, line> lineItem in lines)
                {
                    model.set_line(lineItem.Value);
                }
                foreach (KeyValuePair<int, member> memberItem in members)
                {
                    model.set_member(memberItem.Value);
                }
                model.set_surface(concreteSlab);
                model.set_opening(slabOpening);


            }
            catch (Exception exception)
            {
                //model.cancel_modification();
                logger.Error(exception, "Something wrong in creation of geometry\n" + exception.Message + "\n");
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of geometry\n" + exception.Message + "\n");

                }
            }
            #endregion

            #region supports
            //create nodal support
            var SupportNodes = from n in nodes where n.Value.coordinates.z == 0.0 select n.Value;
            List<int> nodalSupportsNodes = new();
            foreach (var supportNode in SupportNodes)
            {
                nodalSupportsNodes.Add(supportNode.no);
            }
            nodal_support support = new()
            {
                no = 1,
                nodes = nodalSupportsNodes.ToArray(),
                spring = new vector_3d() { x = double.PositiveInfinity, y = double.PositiveInfinity, z = double.PositiveInfinity },
                rotational_restraint = new vector_3d() { x = double.PositiveInfinity, y = double.PositiveInfinity, z = double.PositiveInfinity },
                spring_x_nonlinearity = nodal_support_spring_x_nonlinearity.NONLINEARITY_TYPE_NONE,
                spring_x_nonlinearitySpecified = true,
                spring_y_nonlinearity = nodal_support_spring_y_nonlinearity.NONLINEARITY_TYPE_NONE,
                spring_y_nonlinearitySpecified = true,
                spring_z_nonlinearity = nodal_support_spring_z_nonlinearity.NONLINEARITY_TYPE_DIAGRAM,
                spring_z_nonlinearitySpecified = true,
                rotational_restraint_x_nonlinearity = nodal_support_rotational_restraint_x_nonlinearity.NONLINEARITY_TYPE_NONE,
                rotational_restraint_x_nonlinearitySpecified = true,
                rotational_restraint_y_nonlinearity = nodal_support_rotational_restraint_y_nonlinearity.NONLINEARITY_TYPE_NONE,
                rotational_restraint_y_nonlinearitySpecified = true,
                rotational_restraint_z_nonlinearity = nodal_support_rotational_restraint_z_nonlinearity.NONLINEARITY_TYPE_NONE,
                rotational_restraint_z_nonlinearitySpecified = true,
                axes_sequence = nodal_support_axes_sequence.SEQUENCE_XYZ,
                axes_sequenceSpecified = true,
                coordinate_system = 1,
                coordinate_systemSpecified = true,
                diagram_along_z_end = nodal_support_diagram_along_z_end.DIAGRAM_ENDING_TYPE_CONTINUOUS,
                diagram_along_z_endSpecified = true,
                diagram_along_z_is_sorted = true,
                diagram_along_z_is_sortedSpecified = true,
                diagram_along_z_start = nodal_support_diagram_along_z_start.DIAGRAM_ENDING_TYPE_CONTINUOUS,
                diagram_along_z_startSpecified = true,
                diagram_along_z_symmetric = true,
                diagram_along_z_symmetricSpecified = true,
                diagram_along_z_table = new nodal_support_diagram_along_z_table_row[] {
                    new nodal_support_diagram_along_z_table_row() {
                        no = 1,
                        row = new nodal_support_diagram_along_z_table(){
                            displacement =0.001,
                            displacementSpecified = true,
                            force = 1000,
                            forceSpecified = true,
                            spring= 1e+06,
                            springSpecified = true,
                        }
                    }
                },
                fictitious_column_enabled = false,
                fictitious_column_enabledSpecified = true,
                partial_activity_along_x_negative_slippage = 0,
                partial_activity_along_x_negative_slippageSpecified = true,
                partial_activity_along_x_positive_slippage = 0,
                partial_activity_along_x_positive_slippageSpecified = true,
                partial_activity_along_y_negative_slippage = 0,
                partial_activity_along_y_negative_slippageSpecified = true,
                partial_activity_along_y_positive_slippage = 0,
                partial_activity_along_y_positive_slippageSpecified = true,
                partial_activity_along_z_negative_slippage = 0,
                partial_activity_along_z_negative_slippageSpecified = true,
                partial_activity_along_z_positive_slippage = 0,
                partial_activity_along_z_positive_slippageSpecified = true,
                partial_activity_around_x_negative_slippage = 0,
                partial_activity_around_x_negative_slippageSpecified = true,
                partial_activity_around_x_positive_slippage = 0,
                partial_activity_around_x_positive_slippageSpecified = true,
                partial_activity_around_y_negative_slippage = 0,
                partial_activity_around_y_negative_slippageSpecified = true,
                partial_activity_around_y_positive_slippage = 0,
                partial_activity_around_y_positive_slippageSpecified = true,
                partial_activity_around_z_negative_slippage = 0,
                partial_activity_around_z_negative_slippageSpecified = true,
                partial_activity_around_z_positive_slippage = 0,
                partial_activity_around_z_positive_slippageSpecified = true,
                rotated_about_angle_1 = 0,
                rotated_about_angle_1Specified = true,
                rotated_about_angle_2 = 0,
                rotated_about_angle_2Specified = true,
                rotated_about_angle_3 = 0,
                rotated_about_angle_3Specified = true,
                rotated_about_angle_x = 0,
                rotated_about_angle_xSpecified = true,
                rotated_about_angle_y = 0,
                rotated_about_angle_ySpecified = true,
                rotated_about_angle_z = 0,
                rotated_about_angle_zSpecified = true,
                specific_direction_enabled = false,
                specific_direction_enabledSpecified = true,
                specific_direction_type = nodal_support_specific_direction_type.DIRECTION_TYPE_ROTATED_VIA_3_ANGLES,
                specific_direction_typeSpecified = true,
                support_dimensions_enabled = false,
                support_dimensions_enabledSpecified = true,
                diagram_along_x_ac_yield_minus = 1,
                diagram_along_x_ac_yield_minusSpecified = true,
                diagram_along_y_ac_yield_minus = 1,
                diagram_along_y_ac_yield_minusSpecified = true,
                diagram_along_z_ac_yield_minus = 1,
                diagram_along_z_ac_yield_minusSpecified = true,
                diagram_around_x_ac_yield_minus = 1,
                diagram_around_x_ac_yield_minusSpecified = true,
                diagram_around_y_ac_yield_minus = 1,
                diagram_around_y_ac_yield_minusSpecified = true,
                diagram_around_z_ac_yield_minus = 1,
                diagram_around_z_ac_yield_minusSpecified = true,
                diagram_along_x_ac_yield_plus = 1,
                diagram_along_x_ac_yield_plusSpecified = true,
                diagram_along_y_ac_yield_plus = 1,
                diagram_along_y_ac_yield_plusSpecified = true,
                diagram_along_z_ac_yield_plus = 1,
                diagram_along_z_ac_yield_plusSpecified = true,
                diagram_around_x_ac_yield_plus = 1,
                diagram_around_x_ac_yield_plusSpecified = true,
                diagram_around_y_ac_yield_plus = 1,
                diagram_around_y_ac_yield_plusSpecified = true,
                diagram_around_z_ac_yield_plus = 1,
                diagram_around_z_ac_yield_plusSpecified = true,
                diagram_along_z_acceptance_criteria_active = true,
                diagram_along_z_acceptance_criteria_activeSpecified = true,
                diagram_along_x_minus_color_one = "#a0a0a4",
                diagram_along_y_minus_color_one = "#a0a0a4",
                diagram_along_z_minus_color_one = "#a0a0a4",
                diagram_around_x_minus_color_one = "#a0a0a4",
                diagram_around_y_minus_color_one = "#a0a0a4",
                diagram_around_z_minus_color_one = "#a0a0a4",
                diagram_along_x_minus_color_two = "#ffff00",
                diagram_along_y_minus_color_two = "#ffff00",
                diagram_along_z_minus_color_two = "#ffff00",
                diagram_around_x_minus_color_two = "#ffff00",
                diagram_around_y_minus_color_two = "#ffff00",
                diagram_around_z_minus_color_two = "#ffff00",
                diagram_along_x_plus_color_one = "#ead1dc",
                diagram_along_y_plus_color_one = "#ead1dc",
                diagram_along_z_plus_color_one = "#ead1dc",
                diagram_around_x_plus_color_one = "#ead1dc",
                diagram_around_y_plus_color_one = "#ead1dc",
                diagram_around_z_plus_color_one = "#ead1dc",
                diagram_along_x_plus_color_two = "#ff0000",
                diagram_along_y_plus_color_two = "#ff0000",
                diagram_along_z_plus_color_two = "#ff0000",
                diagram_around_x_plus_color_two = "#ff0000",
                diagram_around_y_plus_color_two = "#ff0000",
                diagram_around_z_plus_color_two = "#ff0000",
                diagram_along_z_color_table = new nodal_support_diagram_along_z_color_table_row[]{
                    new nodal_support_diagram_along_z_color_table_row()
                    {
                        no = 1,
                        row = new nodal_support_diagram_along_z_color_table()
                        {
                            color = "#800000",
                        }
                    },
                    new nodal_support_diagram_along_z_color_table_row()
                    {
                        no = 2,
                        row = new nodal_support_diagram_along_z_color_table(){
                            color = "#ead1dc"
                        }
                    }
                }
            };

            member_hinge hinge = new member_hinge()
            {
                no = 1,
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                name = "Scripted hinge",
                coordinate_system = "Local",
                axial_release_n = double.PositiveInfinity,
                axial_release_nSpecified = true,
                axial_release_n_nonlinearity = member_hinge_axial_release_n_nonlinearity.NONLINEARITY_TYPE_NONE,
                axial_release_n_nonlinearitySpecified = true,
                axial_release_vy = double.PositiveInfinity,
                axial_release_vySpecified = true,
                axial_release_vy_nonlinearity = member_hinge_axial_release_vy_nonlinearity.NONLINEARITY_TYPE_NONE,
                axial_release_vy_nonlinearitySpecified = true,
                axial_release_vz = double.PositiveInfinity,
                axial_release_vzSpecified = true,
                axial_release_vz_nonlinearity = member_hinge_axial_release_vz_nonlinearity.NONLINEARITY_TYPE_NONE,
                axial_release_vz_nonlinearitySpecified = true,
                comment = "Scripted hinge comment",
                diagram_around_z_end = member_hinge_diagram_around_z_end.DIAGRAM_ENDING_TYPE_CONTINUOUS,
                diagram_around_z_endSpecified = true,
                diagram_around_z_is_sorted = true,
                diagram_around_z_is_sortedSpecified = true,
                diagram_around_z_start = member_hinge_diagram_around_z_start.DIAGRAM_ENDING_TYPE_CONTINUOUS,
                diagram_around_z_startSpecified = true,
                diagram_around_z_symmetric = true,
                diagram_around_z_symmetricSpecified = true,
                diagram_around_z_table = new[] { new member_hinge_diagram_around_z_table_row() {
                    no = 1,
                    row = new member_hinge_diagram_around_z_table(){
                    rotation = 0.01,
                    rotationSpecified = true,
                    moment = 20000,
                    momentSpecified = true,
                    spring = 2e+06,
                    springSpecified = true,
                    // note = "rrfrefre",
                    }
                }
                },
                members = "",
                moment_release_mt = double.PositiveInfinity,
                moment_release_mtSpecified = true,
                moment_release_mt_nonlinearity = member_hinge_moment_release_mt_nonlinearity.NONLINEARITY_TYPE_NONE,
                moment_release_mt_nonlinearitySpecified = true,
                moment_release_my = double.PositiveInfinity,
                moment_release_mySpecified = true,
                moment_release_my_nonlinearity = member_hinge_moment_release_my_nonlinearity.NONLINEARITY_TYPE_NONE,
                moment_release_my_nonlinearitySpecified = true,
                moment_release_mz = double.PositiveInfinity,
                moment_release_mzSpecified = true,
                moment_release_mz_nonlinearity = member_hinge_moment_release_mz_nonlinearity.NONLINEARITY_TYPE_DIAGRAM,
                moment_release_mz_nonlinearitySpecified = true,
                partial_activity_along_x_negative_slippage = 0,
                partial_activity_along_x_negative_slippageSpecified = true,
                partial_activity_along_x_positive_slippage = 0,
                partial_activity_along_x_positive_slippageSpecified = true,
                partial_activity_along_y_negative_slippage = 0,
                partial_activity_along_y_negative_slippageSpecified = true,
                partial_activity_along_y_positive_slippage = 0,
                partial_activity_along_y_positive_slippageSpecified = true,
                partial_activity_along_z_negative_slippage = 0,
                partial_activity_along_z_negative_slippageSpecified = true,
                partial_activity_along_z_positive_slippage = 0,
                partial_activity_along_z_positive_slippageSpecified = true,
                partial_activity_around_x_negative_slippage = 0,
                partial_activity_around_x_negative_slippageSpecified = true,
                partial_activity_around_x_positive_slippage = 0,
                partial_activity_around_x_positive_slippageSpecified = true,
            };

            member_hinge hingePlastic = new member_hinge()
            {
                no = 1,
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                name = "scriptedPlasticHinge",
                members = "",
                coordinate_system = "Local",
                axial_release_n = double.PositiveInfinity,
                axial_release_nSpecified = true,
                axial_release_vy = double.PositiveInfinity,
                axial_release_vySpecified = true,
                axial_release_vz = double.PositiveInfinity,
                axial_release_vzSpecified = true,
                moment_release_mt = 0,
                moment_release_my = 0,
                moment_release_mz = 0,
                //     axial_release_n_nonlinearity = NONLINEARITY_TYPE_NONE </ axial_release_n_nonlinearity =
                //    axial_release_vy_nonlinearity = NONLINEARITY_TYPE_NONE </ axial_release_vy_nonlinearity =
                //    axial_release_vz_nonlinearity = NONLINEARITY_TYPE_NONE </ axial_release_vz_nonlinearity =
                //    moment_release_mt_nonlinearity = NONLINEARITY_TYPE_NONE </ moment_release_mt_nonlinearity =
                //    moment_release_my_nonlinearity = NONLINEARITY_TYPE_PLASTIC_FEMA_356_ELASTIC </ moment_release_my_nonlinearity =
                //    moment_release_mz_nonlinearity = NONLINEARITY_TYPE_NONE </ moment_release_mz_nonlinearity =
                //    partial_activity_along_x_negative_slippage = 0 </ partial_activity_along_x_negative_slippage =
                //    partial_activity_along_x_positive_slippage = 0 </ partial_activity_along_x_positive_slippage =
                //    partial_activity_along_y_negative_slippage = 0 </ partial_activity_along_y_negative_slippage =
                //    partial_activity_along_y_positive_slippage = 0 </ partial_activity_along_y_positive_slippage =
                //    partial_activity_along_z_negative_slippage = 0 </ partial_activity_along_z_negative_slippage =
                //    partial_activity_along_z_positive_slippage = 0 </ partial_activity_along_z_positive_slippage =
                //    diagram_along_x_ac_yield_minus = 0.0010000000474974513 </ diagram_along_x_ac_yield_minus =
                //    diagram_along_y_ac_yield_minus = 0.0010000000474974513 </ diagram_along_y_ac_yield_minus =
                //    diagram_along_z_ac_yield_minus = 0.0010000000474974513 </ diagram_along_z_ac_yield_minus =
                //    diagram_around_x_ac_yield_minus = 0.0010000000474974513 </ diagram_around_x_ac_yield_minus =
                //    diagram_around_y_ac_yield_minus = 0.0010000000474974513 </ diagram_around_y_ac_yield_minus =
                //    diagram_around_z_ac_yield_minus = 0.0010000000474974513 </ diagram_around_z_ac_yield_minus =
                //    diagram_along_x_ac_yield_plus = 0.0010000000474974513 </ diagram_along_x_ac_yield_plus =
                //    diagram_along_y_ac_yield_plus = 0.0010000000474974513 </ diagram_along_y_ac_yield_plus =
                //    diagram_along_z_ac_yield_plus = 0.0010000000474974513 </ diagram_along_z_ac_yield_plus =
                //    diagram_around_x_ac_yield_plus = 0.0010000000474974513 </ diagram_around_x_ac_yield_plus =
                //    diagram_around_y_ac_yield_plus = 0.0010000000474974513 </ diagram_around_y_ac_yield_plus =
                //    diagram_around_z_ac_yield_plus = 0.0010000000474974513 </ diagram_around_z_ac_yield_plus =
                //    diagram_along_x_minus_color_one = #a0a0a4</diagram_along_x_minus_color_one = 
                //     diagram_along_y_minus_color_one = #a0a0a4</diagram_along_y_minus_color_one = 
                //     diagram_along_z_minus_color_one = #a0a0a4</diagram_along_z_minus_color_one = 
                //     diagram_around_x_minus_color_one = #a0a0a4</diagram_around_x_minus_color_one = 
                //     diagram_around_y_minus_color_one = #a0a0a4</diagram_around_y_minus_color_one = 
                //     diagram_around_z_minus_color_one = #a0a0a4</diagram_around_z_minus_color_one = 
                //     diagram_along_x_minus_color_two = #0000ff</diagram_along_x_minus_color_two = 
                //     diagram_along_y_minus_color_two = #0000ff</diagram_along_y_minus_color_two = 
                //     diagram_along_z_minus_color_two = #0000ff</diagram_along_z_minus_color_two = 
                //     diagram_around_x_minus_color_two = #0000ff</diagram_around_x_minus_color_two = 
                //     diagram_around_y_minus_color_two = #0000ff</diagram_around_y_minus_color_two = 
                //     diagram_around_z_minus_color_two = #0000ff</diagram_around_z_minus_color_two = 
                //     diagram_along_x_plus_color_one = #a0a0a4</diagram_along_x_plus_color_one = 
                //     diagram_along_y_plus_color_one = #a0a0a4</diagram_along_y_plus_color_one = 
                //     diagram_along_z_plus_color_one = #a0a0a4</diagram_along_z_plus_color_one = 
                //     diagram_around_x_plus_color_one = #a0a0a4</diagram_around_x_plus_color_one = 
                //     diagram_around_y_plus_color_one = #a0a0a4</diagram_around_y_plus_color_one = 
                //     diagram_around_z_plus_color_one = #a0a0a4</diagram_around_z_plus_color_one = 
                //     diagram_along_x_plus_color_two = #ff0000</diagram_along_x_plus_color_two = 
                //     diagram_along_y_plus_color_two = #ff0000</diagram_along_y_plus_color_two = 
                //     diagram_along_z_plus_color_two = #ff0000</diagram_along_z_plus_color_two = 
                //     diagram_around_x_plus_color_two = #ff0000</diagram_around_x_plus_color_two = 
                //     diagram_around_y_plus_color_two = #ff0000</diagram_around_y_plus_color_two = 
                //     diagram_around_z_plus_color_two = #ff0000</diagram_around_z_plus_color_two = 
                //     < plastic_diagram_around_y_table =

                //        < member_hinge_plastic_diagram_around_y_table =

                //            < no = 1 </ no =

                //            < description = </ description =

                //            < row =

                //                < force_ratio = -0.6 </ force_ratio =

                //                < deflection_ratio = -12 </ deflection_ratio =

                //            </ row =

                //        </ member_hinge_plastic_diagram_around_y_table =

                //        < member_hinge_plastic_diagram_around_y_table =

                //            < no = 2 </ no =

                //            < description = </ description =

                //            < row =

                //                < force_ratio = -0.6 </ force_ratio =

                //                < deflection_ratio = -10 </ deflection_ratio =

                //            </ row =

                //        </ member_hinge_plastic_diagram_around_y_table =

                //        < member_hinge_plastic_diagram_around_y_table =

                //            < no = 3 </ no =

                //            < description = </ description =

                //            < row =

                //                < force_ratio = -1.27 </ force_ratio =

                //                < deflection_ratio = -10 </ deflection_ratio =

                //            </ row =

                //        </ member_hinge_plastic_diagram_around_y_table =

                //        < member_hinge_plastic_diagram_around_y_table =

                //            < no = 4 </ no =

                //            < description = </ description =

                //            < row =

                //                < force_ratio = -1 </ force_ratio =

                //                < deflection_ratio = -1 </ deflection_ratio =

                //            </ row =

                //        </ member_hinge_plastic_diagram_around_y_table =

                //        < member_hinge_plastic_diagram_around_y_table =

                //            < no = 5 </ no =

                //            < description = </ description =

                //            < row =

                //                < force_ratio = 0 </ force_ratio =

                //                < deflection_ratio = 0 </ deflection_ratio =

                //            </ row =

                //        </ member_hinge_plastic_diagram_around_y_table =

                //        < member_hinge_plastic_diagram_around_y_table =

                //            < no = 6 </ no =

                //            < description = </ description =

                //            < row =

                //                < force_ratio = 1 </ force_ratio =

                //                < deflection_ratio = 1 </ deflection_ratio =

                //            </ row =

                //        </ member_hinge_plastic_diagram_around_y_table =

                //        < member_hinge_plastic_diagram_around_y_table =

                //            < no = 7 </ no =

                //            < description = </ description =

                //            < row =

                //                < force_ratio = 1.27 </ force_ratio =

                //                < deflection_ratio = 10 </ deflection_ratio =

                //            </ row =

                //        </ member_hinge_plastic_diagram_around_y_table =

                //        < member_hinge_plastic_diagram_around_y_table =

                //            < no = 8 </ no =

                //            < description = </ description =

                //            < row =

                //                < force_ratio = 0.6 </ force_ratio =

                //                < deflection_ratio = 10 </ deflection_ratio =

                //            </ row =

                //        </ member_hinge_plastic_diagram_around_y_table =

                //        < member_hinge_plastic_diagram_around_y_table =

                //            < no = 9 </ no =

                //            < description = </ description =

                //            < row =

                //                < force_ratio = 0.6 </ force_ratio =

                //                < deflection_ratio = 12 </ deflection_ratio =

                //            </ row =

                //        </ member_hinge_plastic_diagram_around_y_table =

                //    </ plastic_diagram_around_y_table =

                //    < plastic_diagram_around_y_antimetric = true </ plastic_diagram_around_y_antimetric =

                //    < plastic_diagram_around_y_force_interaction = false </ plastic_diagram_around_y_force_interaction =

                //    < plastic_diagram_around_y_user_defined = true </ plastic_diagram_around_y_user_defined =

                //    < plastic_diagram_around_y_is_user_defined_member_length = false </ plastic_diagram_around_y_is_user_defined_member_length =

                //    < plastic_diagram_around_y_attached_members_min_max_length / =

                //    < plastic_diagram_around_y_io_negative = 1 </ plastic_diagram_around_y_io_negative =

                //    < plastic_diagram_around_y_io_positive = 1 </ plastic_diagram_around_y_io_positive =

                //    < plastic_diagram_around_y_ls_negative = 6 </ plastic_diagram_around_y_ls_negative =

                //    < plastic_diagram_around_y_ls_positive = 6 </ plastic_diagram_around_y_ls_positive =

                //    < plastic_diagram_around_y_cp_negative = 8 </ plastic_diagram_around_y_cp_negative =

                //    < plastic_diagram_around_y_cp_positive = 8 </ plastic_diagram_around_y_cp_positive =

                //    < plastic_diagram_around_y_minus_color_one = #a0a0a4</plastic_diagram_around_y_minus_color_one = 
                //     < plastic_diagram_around_y_minus_color_two = #008000</plastic_diagram_around_y_minus_color_two = 
                //     < plastic_diagram_around_y_minus_color_three = #ffff00</plastic_diagram_around_y_minus_color_three = 
                //     < plastic_diagram_around_y_minus_color_four = #ff0000</plastic_diagram_around_y_minus_color_four = 
                //     < plastic_diagram_around_y_plus_color_one = #a0a0a4</plastic_diagram_around_y_plus_color_one = 
                //     < plastic_diagram_around_y_plus_color_two = #008000</plastic_diagram_around_y_plus_color_two = 
                //     < plastic_diagram_around_y_plus_color_three = #ffff00</plastic_diagram_around_y_plus_color_three = 
                //     < plastic_diagram_around_y_plus_color_four = #ff0000</plastic_diagram_around_y_plus_color_four = 
                //     < plastic_diagram_around_y_component_type = COMPONENT_TYPE_PRIMARY </ plastic_diagram_around_y_component_type =

                //    < plastic_diagram_around_y_color_table =

                //        < member_hinge_plastic_diagram_around_y_color_table =

                //            < no = 1 </ no =

                //            < description = </ description =

                //            < row =

                //                < color = #ff0000</color = 
                //             </ row =

                //        </ member_hinge_plastic_diagram_around_y_color_table =

                //        < member_hinge_plastic_diagram_around_y_color_table =

                //            < no = 2 </ no =

                //            < description = </ description =

                //            < row =

                //                < color = #bf40bf</color = 
                //             </ row =

                //        </ member_hinge_plastic_diagram_around_y_color_table =

                //        < member_hinge_plastic_diagram_around_y_color_table =

                //            < no = 3 </ no =

                //            < description = </ description =

                //            < row =

                //                < color = #ffb6c1</color = 
                //             </ row =

                //        </ member_hinge_plastic_diagram_around_y_color_table =

                //        < member_hinge_plastic_diagram_around_y_color_table =

                //            < no = 4 </ no =

                //            < description = </ description =

                //            < row =

                //                < color = #ffa500</color = 
                //             </ row =

                //        </ member_hinge_plastic_diagram_around_y_color_table =

                //        < member_hinge_plastic_diagram_around_y_color_table =

                //            < no = 5 </ no =

                //            < description = </ description =

                //            < row =

                //                < color = #000000</color = 
                //             </ row =

                //        </ member_hinge_plastic_diagram_around_y_color_table =

                //        < member_hinge_plastic_diagram_around_y_color_table =

                //            < no = 6 </ no =

                //            < description = </ description =

                //            < row =

                //                < color = #ffff00</color = 
                //             </ row =

                //        </ member_hinge_plastic_diagram_around_y_color_table =

                //        < member_hinge_plastic_diagram_around_y_color_table =

                //            < no = 7 </ no =

                //            < description = </ description =

                //            < row =

                //                < color = #ffa500</color = 
                //             </ row =

                //        </ member_hinge_plastic_diagram_around_y_color_table =

                //        < member_hinge_plastic_diagram_around_y_color_table =

                //            < no = 8 </ no =

                //            < description = </ description =

                //            < row =

                //                < color = #ffb6c1</color = 
                //             </ row =

                //        </ member_hinge_plastic_diagram_around_y_color_table =

                //        < member_hinge_plastic_diagram_around_y_color_table =

                //            < no = 9 </ no =

                //            < description = </ description =

                //            < row =

                //                < color = #bf40bf</color = 
                //             </ row =

                //        </ member_hinge_plastic_diagram_around_y_color_table =

                //        < member_hinge_plastic_diagram_around_y_color_table =

                //            < no = 10 </ no =

                //            < description = </ description =

                //            < row =

                //                < color = #ff0000</color = 
                //             </ row =

                //        </ member_hinge_plastic_diagram_around_y_color_table =

                //    </ plastic_diagram_around_y_color_table =

                //    < comment = </ comment =
            };

            line_hinge lineHinge = new line_hinge()
            {
                no = 1,
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                name = "scripted line hinge",
                translational_release_u_x = double.PositiveInfinity,
                translational_release_u_xSpecified = true,
                translational_release_u_x_nonlinearity = line_hinge_translational_release_u_x_nonlinearity.NONLINEARITY_TYPE_NONE,
                translational_release_u_x_nonlinearitySpecified = true,
                translational_release_u_y = double.PositiveInfinity,
                translational_release_u_ySpecified = true,
                translational_release_u_y_nonlinearity = line_hinge_translational_release_u_y_nonlinearity.NONLINEARITY_TYPE_NONE,
                translational_release_u_y_nonlinearitySpecified = true,
                translational_release_u_z = double.PositiveInfinity,
                translational_release_u_zSpecified = true,
                translational_release_u_z_nonlinearity = line_hinge_translational_release_u_z_nonlinearity.NONLINEARITY_TYPE_NONE,
                translational_release_u_z_nonlinearitySpecified = true,
                rotational_release_phi_x = 0,
                rotational_release_phi_xSpecified = true,
                rotational_release_phi_x_nonlinearity = line_hinge_rotational_release_phi_x_nonlinearity.NONLINEARITY_TYPE_DIAGRAM,
                rotational_release_phi_x_nonlinearitySpecified = true,
                partial_activity_along_x_negative_slippage = 0,
                partial_activity_along_x_negative_slippageSpecified = true,
                partial_activity_along_x_positive_slippage = 0,
                partial_activity_along_x_positive_slippageSpecified = true,
                partial_activity_along_y_negative_slippage = 0,
                partial_activity_along_y_negative_slippageSpecified = true,
                partial_activity_along_y_positive_slippage = 0,
                partial_activity_along_y_positive_slippageSpecified = true,
                partial_activity_along_z_negative_slippage = 0,
                partial_activity_along_z_negative_slippageSpecified = true,
                partial_activity_along_z_positive_slippage = 0,
                partial_activity_along_z_positive_slippageSpecified = true,
                diagram_around_x_symmetric = true,
                diagram_around_x_symmetricSpecified = true,
                diagram_around_x_is_sorted = true,
                diagram_around_x_is_sortedSpecified = true,
                diagram_around_x_table = new line_hinge_diagram_around_x_table_row[]{
                    new line_hinge_diagram_around_x_table_row()
                    {
                        no = 1,
                        row = new line_hinge_diagram_around_x_table()
                        {
                            rotation = 0.01,
                            rotationSpecified = true,
                            moment = 20000,
                            momentSpecified = true,
                            spring = 2e+06,
                            springSpecified = true,
                        }
                    },
                    new line_hinge_diagram_around_x_table_row()
                    {
                        no = 2,
                         row = new line_hinge_diagram_around_x_table()
                         {
                             rotation = 0.03,
                             rotationSpecified = true,
                             moment = 40000,
                             momentSpecified = true,
                             spring = 1000000.0000000001,
                             springSpecified = true,
                         }
                    },
                    new line_hinge_diagram_around_x_table_row()
                    {
                        no = 3,
                        row = new line_hinge_diagram_around_x_table()
                        {
                            rotation = 0.05,
                            rotationSpecified = true,
                            moment = 60000,
                            momentSpecified = true,
                            spring = 999999.9999999998,
                            springSpecified = true,
                        }
                    }
                },
                diagram_around_x_start = line_hinge_diagram_around_x_start.DIAGRAM_ENDING_TYPE_CONTINUOUS,
                diagram_around_x_startSpecified = true,
                diagram_around_x_end = line_hinge_diagram_around_x_end.DIAGRAM_ENDING_TYPE_CONTINUOUS,
                diagram_around_x_endSpecified = true,
                diagram_along_x_ac_yield_minus = 0.0010000000474974513,
                diagram_along_x_ac_yield_minusSpecified = true,
                diagram_along_y_ac_yield_minus = 0.0010000000474974513,
                diagram_along_y_ac_yield_minusSpecified = true,
                diagram_along_z_ac_yield_minus = 0.0010000000474974513,
                diagram_along_z_ac_yield_minusSpecified = true,
                diagram_around_x_ac_yield_minus = 0.0010000000474974513,
                diagram_around_x_ac_yield_minusSpecified = true,
                diagram_along_x_ac_yield_plus = 0.0010000000474974513,
                diagram_along_x_ac_yield_plusSpecified = true,
                diagram_along_y_ac_yield_plus = 0.0010000000474974513,
                diagram_along_y_ac_yield_plusSpecified = true,
                diagram_along_z_ac_yield_plus = 0.0010000000474974513,
                diagram_along_z_ac_yield_plusSpecified = true,
                diagram_around_x_ac_yield_plus = 0.0010000000474974513,
                diagram_around_x_ac_yield_plusSpecified = true,
                diagram_around_x_acceptance_criteria_active = true,
                diagram_around_x_acceptance_criteria_activeSpecified = true,
                diagram_along_x_minus_color_one = "#ead1dc",
                diagram_along_y_minus_color_one = "#a0a0a4",
                diagram_along_z_minus_color_one = "#a0a0a4",
                diagram_around_x_minus_color_one = "#a0a0a4",
                diagram_along_x_minus_color_two = "#ffff00",
                diagram_along_y_minus_color_two = "#ffff00",
                diagram_along_z_minus_color_two = "#ffff00",
                diagram_around_x_minus_color_two = "#ffff00",
                diagram_along_x_plus_color_one = "#a0a0a4",
                diagram_along_y_plus_color_one = "#a0a0a4",
                diagram_along_z_plus_color_one = "#a0a0a4",
                diagram_around_x_plus_color_one = "#ead1dc",
                diagram_along_x_plus_color_two = "#ff0000",
                diagram_along_y_plus_color_two = "#ff0000",
                diagram_along_z_plus_color_two = "#ff0000",
                diagram_around_x_plus_color_two = "#ff0000",
                diagram_around_x_color_table = new line_hinge_diagram_around_x_color_table_row[]{
                    new line_hinge_diagram_around_x_color_table_row(){
                        no = 1,
                        row = new line_hinge_diagram_around_x_color_table(){
                            color = "#800000",
                        }
                    },
                      new line_hinge_diagram_around_x_color_table_row(){
                        no = 2,
                        row = new line_hinge_diagram_around_x_color_table(){
                             color = "#ead1dc",
                       }
                    },
                      new line_hinge_diagram_around_x_color_table_row(){
                        no = 3,
                        row = new line_hinge_diagram_around_x_color_table(){
                              color = "#b97171",
                       }
                    },
                      new line_hinge_diagram_around_x_color_table_row(){
                        no = 4,
                        row = new line_hinge_diagram_around_x_color_table(){
                              color = "#243700",
                       }
                    }
                },
                slab_wall_connection = false,
                slab_wall_connectionSpecified = true,
            };


            try
            {
                model.begin_modification("Set boundary conditions");
                model.set_nodal_support(support);
                model.set_member_hinge(hinge);
                model.set_line_hinge(lineHinge);
            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something happen when creation of nodal support: " + exception.Message);
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of nodal support\n" + exception.Message + "\n");

                }
            }
            #endregion

            #region Steel design - Boundary conditions
            steel_boundary_conditions_nodal_supports_row startSteelBCNodalSupports = new steel_boundary_conditions_nodal_supports_row()
            {
                no = 1,
                row = new steel_boundary_conditions_nodal_supports()
                {
                    nodes = new int[] { 2, 3, 20 },
                    // support_type = support_type.SUPPORT_TYPE_FIXED_IN_Y_AND_TORSION,
                    support_typeSpecified = true,
                    support_in_y = true,
                    support_in_ySpecified = true,
                    restraint_about_x = true,
                    restraint_about_xSpecified = true,
                    restraint_about_z = false,
                    restraint_about_zSpecified = true,
                    restraint_warping = false,
                    restraint_warpingSpecified = true,
                    eccentricity_type_z_type = eccentricity_type_z_type.ECCENTRICITY_TYPE_USER_VALUE,
                    eccentricity_type_z_typeSpecified = true,
                    eccentricity_x = 0.0,
                    eccentricity_xSpecified = true,
                    eccentricity_y = 0.0,
                    eccentricity_ySpecified = true,
                    rotation = 0.0,
                    rotationSpecified = true,
                }
            };
            steel_boundary_conditions_nodal_supports_row endSteelBCNodalSupports = new steel_boundary_conditions_nodal_supports_row()
            {
                no = 2,
                row = new steel_boundary_conditions_nodal_supports()
                {
                    nodes = new int[] { 3, 21 },
                    // support_type = support_type.SUPPORT_TYPE_FIXED_IN_Y_AND_TORSION,
                    support_typeSpecified = true,
                    support_in_y = true,
                    support_in_ySpecified = true,
                    restraint_about_x = true,
                    restraint_about_xSpecified = true,
                    restraint_about_z = false,
                    restraint_about_zSpecified = true,
                    restraint_warping = false,
                    restraint_warpingSpecified = true,
                    eccentricity_type_z_type = eccentricity_type_z_type.ECCENTRICITY_TYPE_USER_VALUE,
                    eccentricity_type_z_typeSpecified = true,
                    eccentricity_x = 0.0,
                    eccentricity_xSpecified = true,
                    eccentricity_y = 0.0,
                    eccentricity_ySpecified = true,
                    rotation = 0.0,
                    rotationSpecified = true,
                }
            };
            steel_boundary_conditions_member_hinges_row startSteelBCMemberHinges = new steel_boundary_conditions_member_hinges_row()
            {
                no = 1,
                row = new steel_boundary_conditions_member_hinges()
                {
                    node_seq_no = "Start",
                    nodes = new int[] { 2, 3, 20 },
                    release_in_y = false,
                    release_in_ySpecified = true,
                    release_about_x = true,
                    release_about_xSpecified = true,
                    release_about_z = false,
                    release_about_zSpecified = true,
                    release_warping = false,
                    release_warpingSpecified = true,
                }
            };
            steel_boundary_conditions_member_hinges_row endSteelBCMemberHinges = new steel_boundary_conditions_member_hinges_row()
            {
                no = 2,
                row = new steel_boundary_conditions_member_hinges()
                {
                    node_seq_no = "",
                    nodes = new int[] { 3, 21 },
                    release_in_y = false,
                    release_in_ySpecified = true,
                    release_about_x = true,
                    release_about_xSpecified = true,
                    release_about_z = false,
                    release_about_zSpecified = true,
                    release_warping = false,
                    release_warpingSpecified = true,
                }
            };
            steel_boundary_conditions steelBoundaryConditions = new()
            {
                no = 1,
                definition_type = steel_boundary_conditions_definition_type.TYPE_2D,
                definition_typeSpecified = true,
                coordinate_system = "COORDINATE_SYSTEM_TYPE_LOCAL",
                members = new int[] { 2, 14, 26 },
                nodal_supports = new steel_boundary_conditions_nodal_supports_row[] { startSteelBCNodalSupports, endSteelBCNodalSupports },
                member_hinges = new steel_boundary_conditions_member_hinges_row[] { startSteelBCMemberHinges },

            };
            try
            {
                model.begin_modification("Set steel boundary conditions");
                model.set_steel_boundary_conditions(steelBoundaryConditions);
            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something happen when creation of Set steel boundary condition: " + exception.Message);
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of Set steel boundary condition\n" + exception.Message + "\n");

                }
            }

            #endregion

            #region Steel design effective lengths
            var steelMembers = from m in members where m.Value.comment == "steel column" select m.Value;
            List<int> steelColumns = new();
            foreach (var mem in steelMembers)
            {
                steelColumns.Add(mem.no);
            }
            steel_effective_lengths_nodal_supports_row startSteelEffectiveLengthsNodalSupports = new steel_effective_lengths_nodal_supports_row()
            {
                no = 1,
                row = new steel_effective_lengths_nodal_supports()
                {
                    support_type = support_type.SUPPORT_TYPE_FIXED_IN_Z_Y_AND_TORSION,
                    support_typeSpecified = true,
                    support_in_z = true,
                    support_in_zSpecified = true,
                    support_spring_in_y = 0,
                    support_spring_in_ySpecified = true,
                    eccentricity_type = eccentricity_type.ECCENTRICITY_TYPE_NONE,
                    eccentricity_typeSpecified = true,
                    eccentricity_ez = 0,
                    eccentricity_ezSpecified = true,
                    restraint_spring_about_x = 0,
                    restraint_spring_about_xSpecified = true,
                    restraint_spring_about_z = 0,
                    restraint_spring_about_zSpecified = true,
                    restraint_spring_warping = 0,
                    restraint_spring_warpingSpecified = true,
                    support_in_y_type = support_in_y_type.SUPPORT_STATUS_YES,
                    support_in_y_typeSpecified = true,
                    restraint_about_x_type = restraint_about_x_type.SUPPORT_STATUS_YES,
                    restraint_about_x_typeSpecified = true,
                    restraint_about_z_type = restraint_about_z_type.SUPPORT_STATUS_NO,
                    restraint_about_z_typeSpecified = true,
                    restraint_warping_type = restraint_warping_type.SUPPORT_STATUS_NO,
                    restraint_warping_typeSpecified = true,
                    //nodes = new int[] { 2, 5, 8 }
                }
            };
            steel_effective_lengths_nodal_supports_row endSteelEffectiveLengthsNodalSupports = new steel_effective_lengths_nodal_supports_row()
            {
                no = 2,
                row = new steel_effective_lengths_nodal_supports()
                {
                    support_type = support_type.SUPPORT_TYPE_FIXED_IN_Z_Y_AND_TORSION,
                    support_typeSpecified = true,
                    support_in_z = true,
                    support_in_zSpecified = true,
                    support_spring_in_y = 0,
                    support_spring_in_ySpecified = true,
                    eccentricity_type = eccentricity_type.ECCENTRICITY_TYPE_NONE,
                    eccentricity_typeSpecified = true,
                    eccentricity_ez = 0,
                    eccentricity_ezSpecified = true,
                    restraint_spring_about_x = 0,
                    restraint_spring_about_xSpecified = true,
                    restraint_spring_about_z = 0,
                    restraint_spring_about_zSpecified = true,
                    restraint_spring_warping = 0,
                    restraint_spring_warpingSpecified = true,
                    support_in_y_type = support_in_y_type.SUPPORT_STATUS_YES,
                    support_in_y_typeSpecified = true,
                    restraint_about_x_type = restraint_about_x_type.SUPPORT_STATUS_YES,
                    restraint_about_x_typeSpecified = true,
                    restraint_about_z_type = restraint_about_z_type.SUPPORT_STATUS_NO,
                    restraint_about_z_typeSpecified = true,
                    restraint_warping_type = restraint_warping_type.SUPPORT_STATUS_NO,
                    restraint_warping_typeSpecified = true,
                    //nodes = new int[] { 3, 6, 9 }
                }
            };

            steel_effective_lengths_factors_row steelEffectiveLengthsFactors = new steel_effective_lengths_factors_row()
            {
                no = 1,
                row = new steel_effective_lengths_factors()
                {
                    flexural_buckling_u = 1,
                    flexural_buckling_uSpecified = true,
                    flexural_buckling_v = 1,
                    flexural_buckling_vSpecified = true,
                    flexural_buckling_y = 0,
                    flexural_buckling_ySpecified = true,
                    flexural_buckling_z = 0,
                    flexural_buckling_zSpecified = true,
                    torsional_buckling = 1,
                    torsional_bucklingSpecified = true,
                    lateral_torsional_buckling = 0,
                    lateral_torsional_bucklingSpecified = true,
                    lateral_torsional_buckling_top = 0,
                    lateral_torsional_buckling_topSpecified = true,
                    lateral_torsional_buckling_bottom = 0,
                    lateral_torsional_buckling_bottomSpecified = true,
                    twist_restraint = 0,
                    twist_restraintSpecified = true,
                    lateral_torsional_restraint = 0,
                    lateral_torsional_restraintSpecified = true,
                    critical_moment = 0,
                    critical_momentSpecified = true,
                }
            };
            steel_effective_lengths steelEffectiveLengths = new steel_effective_lengths()
            {
                no = 1,
                name = "Steel effective lengths",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                members = steelColumns.ToArray(),
                flexural_buckling_about_y = true,
                flexural_buckling_about_ySpecified = true,
                flexural_buckling_about_z = true,
                flexural_buckling_about_zSpecified = true,
                torsional_buckling = true,
                torsional_bucklingSpecified = true,
                lateral_torsional_buckling = true,
                lateral_torsional_bucklingSpecified = true,
                principal_section_axes = true,
                principal_section_axesSpecified = true,
                geometric_section_axes = false,
                geometric_section_axesSpecified = true,
                nodal_supports = new steel_effective_lengths_nodal_supports_row[] { startSteelEffectiveLengthsNodalSupports, endSteelEffectiveLengthsNodalSupports },
                factors = new steel_effective_lengths_factors_row[] { steelEffectiveLengthsFactors },
                intermediate_nodes = false,
                intermediate_nodesSpecified = true,
                different_properties = true,
                different_propertiesSpecified = true,
                factors_definition_absolute = false,
                factors_definition_absoluteSpecified = true,
                import_from_stability_analysis_enabled = false,
                import_from_stability_analysis_enabledSpecified = true,
                determination_mcr_europe = steel_effective_lengths_determination_mcr_europe.DETERMINATION_EUROPE_EIGENVALUE,
                determination_mcr_europeSpecified = true,
            };
            try
            {
                model.begin_modification("Set steel effective lengths");
                model.set_steel_effective_lengths(steelEffectiveLengths);
            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something happen when creation of steel effective lengths: " + exception.Message);
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of steel effective lengths\n" + exception.Message + "\n");

                }
            }
            #endregion

            #region Steel member rotational restrain types
            member_rotational_restraint steelMemberRotationalRestraint = new member_rotational_restraint()
            {
                no = 2,
                name = "Scripted Continuous (Members : 26,36)",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                // members = new int[] { 26, 36 },
                type = member_rotational_restraint_type.TYPE_CONTINUOUS,
                typeSpecified = true,
                sheeting_name = "Arval (-) 35/207 - 0.63 (b: 1) | DIN 18807 | Arval",
                material_name = "S235J2",
                position_of_sheeting = member_rotational_restraint_position_of_sheeting.SHEETING_POSITION_NEGATIVE,
                position_of_sheetingSpecified = true,
                method_of_determining_cda = member_rotational_restraint_method_of_determining_cda.EN_1993_1_3_TABLE_10_3,
                method_of_determining_cdaSpecified = true,
                continuous_beam_effect = member_rotational_restraint_continuous_beam_effect.CONTINUOUS_BEAM_EFFECT_END_PANEL,
                continuous_beam_effectSpecified = true,
                section_deformation_cdb = true,
                section_deformation_cdbSpecified = true,
                beam_spacing = 3.0,
                beam_spacingSpecified = true,
                spring_stiffness = 5.20,
                spring_stiffnessSpecified = true,
            };
            try
            {
                model.begin_modification("Set  member_rotational_restraint");
                model.set_member_rotational_restraint(steelMemberRotationalRestraint);
            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something happen when creation of  member_rotational_restraint: " + exception.Message);
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of  member_rotational_restraint\n" + exception.Message + "\n");

                }
            }
            #endregion

            #region Steel member shear panels
            member_shear_panel steelMemberShearPanel = new member_shear_panel()
            {
                no = 2,
                definition_type = member_shear_panel_definition_type.DEFINITION_TYPE_TRAPEZOIDAL_SHEETING,
                definition_typeSpecified = true,
                user_defined_name_enabled = true,
                name = "Trapezoidal sheeting (Members : 26,28,30,32,34,36)",
                comment = "Scripted element",
                // members = new int[] { 26, 28, 30, 32, 34, 36 },
                panel_length = 10,
                panel_lengthSpecified = true,
                beam_spacing = 3,
                beam_spacingSpecified = true,
                fastening_arrangement = member_shear_panel_fastening_arrangement.FASTENING_ARRANGEMENT_EVERY_RIB,
                coefficient_k1 = 0.00024,
                coefficient_k1Specified = true,
                coefficient_k2 = 0.00664,
                coefficient_k2Specified = true,
                sheeting_name = "Arval (-) 35/207 - 0.63 (b: 1) | DIN 18807 | Arval",//get returns b in mm - input is in meters
            };
            try
            {
                model.begin_modification("Set  member_shear_panel");
                model.set_member_shear_panel(steelMemberShearPanel);
            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something happen when creation of  member_shear_panel: " + exception.Message);
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of  steel_member_rotational_restraint\n" + exception.Message + "\n");

                }
            }
            #endregion

            #region Set member_transverse_stiffener
            member_transverse_stiffener_components_row TransverseStiffenerComponent = new member_transverse_stiffener_components_row()
            {
                no = 1,
                row = new member_transverse_stiffener_components()
                {
                    stiffener_type = stiffener_type.STIFFENER_COMPONENT_TYPE_WARPING_RESTRAINT,
                    stiffener_typeSpecified = true,
                    position = 1,
                    positionSpecified = true,
                    position_type = position_type.STIFFENER_COMPONENT_POSITION_DOUBLE_SIDED,
                    position_typeSpecified = true,
                    multiple = true,
                    multipleSpecified = true,
                    note = "Warping Restraint",
                    multiple_number = 2,
                    multiple_numberSpecified = true,
                    multiple_offset_definition_type = multiple_offset_definition_type.OFFSET_DEFINITION_TYPE_RELATIVE,
                    multiple_offset_definition_typeSpecified = true,
                    multiple_offset = 0.25,
                    multiple_offsetSpecified = true,
                    consider_stiffener = false,
                    consider_stiffenerSpecified = true,
                    full_warping_restraint = true,
                    full_warping_restraintSpecified = true,
                    user_defined_restraint = false,
                    user_defined_restraintSpecified = true,
                }
            };
            member_transverse_stiffener TransverseStiffener = new member_transverse_stiffener()
            {
                no = 1,
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                name = "1 Transverse Stiffener(s) | Transverse Stiffener (Members : 26)",
                members = new int[] { 26, 28, 30, 32, 34, 36 },
                components = new member_transverse_stiffener_components_row[] { TransverseStiffenerComponent },
            };
            try
            {
                model.begin_modification("Set  member_transverse_stiffener");
                model.set_member_transverse_stiffener(TransverseStiffener);
            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something happen when creation of  member_transverse_stiffener: " + exception.Message);
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of  steel_member_rotational_restraint\n" + exception.Message + "\n");

                }
            }
            #endregion


            #region  steel design configurations
            steel_design_uls_configuration steelDesignUlsConfiguration = new steel_design_uls_configuration()
            {
                no = 2,
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                name = "Steel ULS config",
                comment = "Scripted configuration",
                assigned_to_all_members = true,
                assigned_to_all_membersSpecified = true,
            };
            steel_design_sls_configuration steelDesignSlsConfiguration = new steel_design_sls_configuration()
            {
                no = 2,
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                name = "Steel SLS config",
                comment = "Scripted configuration",
                assigned_to_all_members = true,
                assigned_to_all_membersSpecified = true,
            };
            steel_design_fr_configuration steelDesignFrConfiguration = new steel_design_fr_configuration()
            {
                no = 2,
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                name = "Steel FR config",
                comment = "Scripted configuration",
                assigned_to_all_members = true,
                assigned_to_all_membersSpecified = true,
            };
            try
            {
                model.begin_modification("Set  steel_member_design configurations");
                model.set_steel_design_uls_configuration(steelDesignUlsConfiguration);
                model.set_steel_design_sls_configuration(steelDesignSlsConfiguration);
                model.set_steel_design_fr_configuration(steelDesignFrConfiguration);
            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something happen when creation of  steel_member_design configurations: " + exception.Message);
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of  steel_member_design configurations\n" + exception.Message + "\n");

                }
            }

            #endregion

            #region Aluminum member local cross_section reduction
            aluminum_member_local_cross_section_reduction_components_row aluminumReductionComponentDesignParams = new aluminum_member_local_cross_section_reduction_components_row()
            {
                no = 2,
                row = new aluminum_member_local_cross_section_reduction_components()
                {
                    // reduction_type = reduction_type.REDUCTION_COMPONENT_TYPE_DESIGN_PARAMETERS,
                    // reduction_typeSpecified = true,
                    // position = 2.0,
                    // positionSpecified = true,
                    // multiple = false,
                    // multipleSpecified = true,
                    // fastener_definition_type = fastener_definition_type.DEFINITION_TYPE_RELATIVE,
                    // fastener_definition_typeSpecified = true,
                    // reduction_area_factor = 0.5,
                    // reduction_area_factorSpecified = true,
                    // gross_area = new variant()
                    // {
                    //     type = 0,
                    //     value = "1.0"
                    // },

                    // net_area = new variant()
                    // {
                    //     type = 0,
                    //     value = "0.5"
                    // }

                }
            };
            aluminum_member_local_cross_section_reduction_components_row aluminumReductionComponentSection = new aluminum_member_local_cross_section_reduction_components_row()
            {
                no = 1,
                row = new aluminum_member_local_cross_section_reduction_components()
                {
                    // reduction_type = reduction_type.REDUCTION_COMPONENT_TYPE_SECTION_VALUES,
                    // reduction_typeSpecified = true,
                    // position = 1.0,
                    // positionSpecified = true,
                    // multiple = false,
                    // multipleSpecified = true,
                    // fastener_definition_type = fastener_definition_type.DEFINITION_TYPE_RELATIVE,
                    // fastener_definition_typeSpecified = true,
                    // sectional_area_factor = 0.5,
                    // sectional_area_factorSpecified = true,
                    // shear_area_y_factor = 0.5,
                    // shear_area_y_factorSpecified = true,
                    // shear_area_z_factor = 0.5,
                    // shear_area_z_factorSpecified = true,
                    // moment_of_inertia_y_factor = 0.5,
                    // moment_of_inertia_y_factorSpecified = true,
                    // moment_of_inertia_z_factor = 0.5,
                    // moment_of_inertia_z_factorSpecified = true,
                    // torsional_constant_factor = 0.5,
                    // torsional_constant_factorSpecified = true,


                }
            };
            aluminum_member_local_cross_section_reduction aluminumMemberLocalCrossSectionReduction = new aluminum_member_local_cross_section_reduction()
            {
                no = 1,
                user_defined_name_enabled = false,
                user_defined_name_enabledSpecified = true,
                name = "Aluminum member local cross_section reduction",
                comment = "Aluminum member local cross_section reduction",
                members = new int[] { 39 },
                components = new aluminum_member_local_cross_section_reduction_components_row[] { aluminumReductionComponentSection, aluminumReductionComponentDesignParams },
            };

            try
            {
                model.begin_modification("Set  Aluminum member local cross_section reduction");
                // model.set_aluminum_member_local_cross_section_reduction(aluminumMemberLocalCrossSectionReduction);
            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something happen when creation of  aluminum member local cross_section reduction: " + exception.Message);
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of  aluminum member local cross_section reduction\n" + exception.Message + "\n");
                }
            }
            #endregion

            #region Aluminum member_shear_panel
            member_shear_panel aluminumShearPanel = new member_shear_panel()
            {
                no = 3,
                user_defined_name_enabled = false,
                user_defined_name_enabledSpecified = true,
                name = "Aluminum  shear panel",
                comment = "Aluminum  shear panel",
                // members = new int[] { 39 },
                definition_type = member_shear_panel_definition_type.DEFINITION_TYPE_TRAPEZOIDAL_SHEETING,
                definition_typeSpecified = true,
                panel_length = 2.0,
                panel_lengthSpecified = true,
                beam_spacing = 1.0,
                beam_spacingSpecified = true,
                fastening_arrangement = member_shear_panel_fastening_arrangement.FASTENING_ARRANGEMENT_EVERY_RIB,
                coefficient_k1 = 0.00024,
                coefficient_k1Specified = true,
                coefficient_k2 = 10.43,
                coefficient_k2Specified = true,
                sheeting_name = "FI (+) 35/207 - 0.63 (b: 1) | DIN 18807 | Fischer Profil",//get returns b in mm - input is in meters

            };
            try
            {
                model.begin_modification("Set  Aluminum member local cross_section reduction");
                model.set_member_shear_panel(aluminumShearPanel);
            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something happen when creation of  aluminum member local cross_section reduction: " + exception.Message);
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of  aluminum member local cross_section reduction\n" + exception.Message + "\n");
                }
            }
            #endregion

            #region  Aluminum member rotational restraint
            member_rotational_restraint aluminumMemberRotationalRestraint = new member_rotational_restraint()
            {
                no = 3,
                name = "Scripted Continuous (Members : 39)",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                // members = new int[] { 39 },
                type = member_rotational_restraint_type.TYPE_CONTINUOUS,
                typeSpecified = true,
                sheeting_name = "Hoesch (-) T35.1 - 0.63 (b: 1) | DIN 18807 | ThyssenKrupp Hoesch",
                material_name = "EN AW-3004 H14",
                position_of_sheeting = member_rotational_restraint_position_of_sheeting.SHEETING_POSITION_NEGATIVE,
                position_of_sheetingSpecified = true,
                continuous_beam_effect = member_rotational_restraint_continuous_beam_effect.CONTINUOUS_BEAM_EFFECT_END_PANEL,
                continuous_beam_effectSpecified = true,
                section_deformation_cdb = true,
                section_deformation_cdbSpecified = true,
                beam_spacing = 3.0,
                beam_spacingSpecified = true,
                spring_stiffness = 5.20,
                spring_stiffnessSpecified = true,
                different_spring_stiffness = false,
                different_spring_stiffnessSpecified = true,
            };
            try
            {
                model.begin_modification("Set  Aluminum member local cross_section reduction");
                model.set_member_rotational_restraint(aluminumMemberRotationalRestraint);
            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something happen when creation of  aluminum member local cross_section reduction: " + exception.Message);
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of  aluminum member local cross_section reduction\n" + exception.Message + "\n");
                }
            }
            #endregion


            #region aluminumTransverseWeld

            aluminum_member_transverse_weld_components_row buttWeld = new aluminum_member_transverse_weld_components_row()
            {
                no = 1,
                row = new aluminum_member_transverse_weld_components()
                {
                    weld_type = weld_type.WELD_COMPONENT_TYPE_BUTT,
                    weld_typeSpecified = true,
                    multiple = false,
                    multipleSpecified = true,
                    position = 1.0,
                    positionSpecified = true,
                    note = " Weld one",
                    size = 0.005,
                    sizeSpecified = true,
                    method_ec_or_adm_type = method_ec_or_adm_type.WELDING_METHOD_TIG,
                    method_ec_or_adm_typeSpecified = true,
                    number_of_heat_paths = 2,
                    number_of_heat_pathsSpecified = true,
                    //temperature_of_material_between_welding_cycles = 2000,
                    //temperature_of_material_between_welding_cyclesSpecified = true,
                }
            };
            aluminum_member_transverse_weld_components_row filletWeld = new aluminum_member_transverse_weld_components_row()
            {
                no = 1,
                row = new aluminum_member_transverse_weld_components()
                {
                    weld_type = weld_type.WELD_COMPONENT_TYPE_FILLET,
                    weld_typeSpecified = true,
                    multiple = false,
                    multipleSpecified = true,
                    position = 2.0,
                    positionSpecified = true,
                    note = " Weld two",
                    size = 0.005,
                    sizeSpecified = true,
                    method_ec_or_adm_type = method_ec_or_adm_type.WELDING_METHOD_TIG,
                    method_ec_or_adm_typeSpecified = true,
                    number_of_heat_paths = 2,
                    number_of_heat_pathsSpecified = true,
                    //temperature_of_material_between_welding_cycles = 2000,
                    //temperature_of_material_between_welding_cyclesSpecified = true,
                }
            };
            aluminum_member_transverse_weld aluminumTransverseWeld = new aluminum_member_transverse_weld()
            {
                no = 1,
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                name = "2 Welds | Butt, Fillet scripted",
                members = new int[] { 39 },
                components = new aluminum_member_transverse_weld_components_row[] { buttWeld, filletWeld },
                comment = "Scripted welds"
            };
            try
            {
                model.begin_modification("Set  Aluminum member local cross_section reduction");
                model.set_aluminum_member_transverse_weld(aluminumTransverseWeld);
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Something happen when creation of  aluminum member local cross_section reduction: " + exception.Message);
                throw;
            }
            finally
            {
                model.finish_modification();
            }
            #endregion

            #region Aluminum Effective Length
            aluminum_effective_lengths_nodal_supports_row aluminumNodalSupportsStart = new aluminum_effective_lengths_nodal_supports_row()
            {
                no = 1,
                row = new aluminum_effective_lengths_nodal_supports()
                {
                    support_type = support_type.SUPPORT_TYPE_FIXED_IN_Z_Y_AND_TORSION,
                    support_typeSpecified = true,
                    support_in_z = true,
                    support_in_zSpecified = true,
                    eccentricity_type = eccentricity_type.ECCENTRICITY_TYPE_NONE,
                    eccentricity_typeSpecified = true,
                    support_in_y_type = support_in_y_type.SUPPORT_STATUS_YES,
                    support_in_y_typeSpecified = true,
                    restraint_about_x_type = restraint_about_x_type.SUPPORT_STATUS_YES,
                    restraint_about_z_type = restraint_about_z_type.SUPPORT_STATUS_NO,
                    restraint_warping_type = restraint_warping_type.SUPPORT_STATUS_NO,
                    nodes = new int[] { 3 },
                }
            };
            aluminum_effective_lengths_nodal_supports_row aluminumNodalSupportsEnd = new aluminum_effective_lengths_nodal_supports_row()
            {
                no = 2,
                row = new aluminum_effective_lengths_nodal_supports()
                {
                    support_type = support_type.SUPPORT_TYPE_FIXED_IN_Z_Y_AND_TORSION,
                    support_typeSpecified = true,
                    support_in_z = true,
                    support_in_zSpecified = true,
                    eccentricity_type = eccentricity_type.ECCENTRICITY_TYPE_NONE,
                    eccentricity_typeSpecified = true,
                    support_in_y_type = support_in_y_type.SUPPORT_STATUS_YES,
                    support_in_y_typeSpecified = true,
                    restraint_about_x_type = restraint_about_x_type.SUPPORT_STATUS_YES,
                    restraint_about_z_type = restraint_about_z_type.SUPPORT_STATUS_NO,
                    restraint_warping_type = restraint_warping_type.SUPPORT_STATUS_NO,
                    nodes = new int[] { 24 },
                }
            };
            aluminum_effective_lengths_factors_row aluminumEffectiveLengthsFactors = new aluminum_effective_lengths_factors_row()
            {
                no = 1,
                row = new aluminum_effective_lengths_factors()
                {
                    flexural_buckling_u = 1,
                    flexural_buckling_uSpecified = true,
                    flexural_buckling_v = 1,
                    flexural_buckling_vSpecified = true,
                    torsional_buckling = 1,
                    torsional_bucklingSpecified = true,
                }
            };
            aluminum_effective_lengths aluminumEffectiveLengths = new aluminum_effective_lengths()
            {
                no = 1,
                user_defined_name_enabled = true,
                name = "Aluminum scripted Effective Lengths",
                comment = "Scripted effective lengths",
                members = new int[] { 39 },
                flexural_buckling_about_y = true,
                flexural_buckling_about_ySpecified = true,
                flexural_buckling_about_z = true,
                flexural_buckling_about_zSpecified = true,
                torsional_buckling = true,
                torsional_bucklingSpecified = true,
                lateral_torsional_buckling = true,
                lateral_torsional_bucklingSpecified = true,
                principal_section_axes = true,
                principal_section_axesSpecified = true,
                geometric_section_axes = false,
                geometric_section_axesSpecified = true,
                intermediate_nodes = false,
                different_properties = true,
                factors_definition_absolute = false,
                import_from_stability_analysis_enabled = false,
                determination_mcr_europe = aluminum_effective_lengths_determination_mcr_europe.DETERMINATION_EUROPE_EIGENVALUE,
                determination_mcr_europeSpecified = true,
                nodal_supports = new aluminum_effective_lengths_nodal_supports_row[] { aluminumNodalSupportsStart, aluminumNodalSupportsEnd },
                factors = new aluminum_effective_lengths_factors_row[] { aluminumEffectiveLengthsFactors },
            };

            try
            {
                model.begin_modification("Set  Aluminum effective length");
                model.set_aluminum_effective_lengths(aluminumEffectiveLengths);
            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something happen when creation of Aluminum effective length: " + exception.Message);
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of  Aluminum effective length\n" + exception.Message + "\n");

                }
            }
            #endregion
            #region  aluminum design configurations
            aluminum_design_uls_configuration aluminumDesignUlsConfiguration = new aluminum_design_uls_configuration()
            {
                no = 2,
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                name = "aluminum ULS config",
                comment = "Scripted configuration",
                assigned_to_all_members = true,
                assigned_to_all_membersSpecified = true,
            };
            aluminum_design_sls_configuration aluminumDesignSlsConfiguration = new aluminum_design_sls_configuration()
            {
                no = 2,
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                name = "aluminum SLS config",
                comment = "Scripted configuration",
                assigned_to_all_members = true,
                assigned_to_all_membersSpecified = true,
            };

            try
            {
                model.begin_modification("Set  aluminum configurations");
                model.set_aluminum_design_uls_configuration(aluminumDesignUlsConfiguration);
                model.set_aluminum_design_sls_configuration(aluminumDesignSlsConfiguration);
            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something happen when creation of  aluminum_member_design configurations: " + exception.Message);
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of  steel_member_design configurations\n" + exception.Message + "\n");

                }
            }

            #endregion


            #region Loads
            #region Load case and analysis settings
            // create analysis settings
            #region  Static analysis settings
            static_analysis_settings analysis = new()
            {
                no = 1,
                analysis_type = static_analysis_settings_analysis_type.GEOMETRICALLY_LINEAR,
                analysis_typeSpecified = true,
                name = "MySettings",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,

            };
            static_analysis_settings analysisSecondOrder = new()
            {
                no = 1,
                analysis_type = static_analysis_settings_analysis_type.SECOND_ORDER_P_DELTA,
                analysis_typeSpecified = true,
                name = "MySettingsSecondOrder",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,

            };
            try
            {
                model.begin_modification("Static analysis settings");
                model.set_static_analysis_settings(analysis);
                model.set_static_analysis_settings(analysisSecondOrder);
            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something happen when creation of static analysis settings" + exception.Message);
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of load cases and static analysis settings\n" + exception.Message + "\n");

                }
            }
            #endregion

            #region Stability settings
            stability_analysis_settings stabilityAnalysisSettings = new stability_analysis_settings()
            {
                no = 101,
                comment = "Script genarated settings",
                name = "My SAS",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                analysis_type = stability_analysis_settings_analysis_type.EIGENVALUE_METHOD,
                analysis_typeSpecified = true,
                number_of_lowest_eigenvalues = 4,
                number_of_lowest_eigenvaluesSpecified = true,
                considered_favored_effect = true,
                considered_favored_effectSpecified = true,
                activate_minimum_initial_prestress = true,
                activate_minimum_initial_prestressSpecified = true,
                eigenvalue_method = stability_analysis_settings_eigenvalue_method.EIGENVALUE_METHOD_LANCZOS,
                matrix_type = stability_analysis_settings_matrix_type.MATRIX_TYPE_STANDARD,
                matrix_typeSpecified = true,
            };
            try
            {
                model.begin_modification("Set SAS");
                model.set_stability_analysis_settings(stabilityAnalysisSettings);
            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something happen when creation of stability analysis settings" + exception.Message);
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of load\n" + exception.Message + "\n");

                }
            }


            #endregion


            load_case selfWeightLC = new()
            {
                no = 1,
                name = "SelfWeight",
                static_analysis_settings = analysis.no,
                analysis_type = load_case_analysis_type.ANALYSIS_TYPE_STATIC,
                analysis_typeSpecified = true,
                static_analysis_settingsSpecified = true,
                self_weight_active = true,
                self_weight_activeSpecified = true,
                self_weight_factor_z = 1.0,
                self_weight_factor_zSpecified = true,
                action_category = "ACTION_CATEGORY_PERMANENT_G",
                calculate_critical_load = true,
                calculate_critical_loadSpecified = true,
                stability_analysis_settings = stabilityAnalysisSettings.no,
                stability_analysis_settingsSpecified = true,
            };

            // create load case
            load_case lcData = new()
            {
                no = 2,
                name = "My load case",
                self_weight_active = false,
                self_weight_activeSpecified = true,
                static_analysis_settings = analysis.no,
                static_analysis_settingsSpecified = true,
                analysis_type = load_case_analysis_type.ANALYSIS_TYPE_STATIC,
                analysis_typeSpecified = true,
                action_category = "ACTION_CATEGORY_PERMANENT_IMPOSED_GQ",
            };


            //Snow / Ice loads - H &lt;= 1000 m | Qs
            load_case lcSnow1 = new()
            {
                no = 3,
                name = "My load case snow",
                self_weight_active = false,
                self_weight_activeSpecified = true,
                static_analysis_settings = analysis.no,
                static_analysis_settingsSpecified = true,
                analysis_type = load_case_analysis_type.ANALYSIS_TYPE_STATIC,
                analysis_typeSpecified = true,
                action_category = "ACTION_CATEGORY_SNOW_ICE_LOADS_H_GREATER_THAN_1000_M_QS",
            };

            imperfection_case imperfectionCase = new imperfection_case()
            {
                no = 1,
                type = imperfection_case_type.IMPERFECTION_TYPE_LOCAL_IMPERFECTIONS,
                typeSpecified = true,
            };

            load_case lcWithImperfection = new()
            {
                no = 4,
                name = "My load case",
                self_weight_active = false,
                self_weight_activeSpecified = true,
                static_analysis_settings = analysisSecondOrder.no,
                static_analysis_settingsSpecified = true,
                analysis_type = load_case_analysis_type.ANALYSIS_TYPE_STATIC,
                analysis_typeSpecified = true,
                consider_imperfection = true,
                consider_imperfectionSpecified = true,
                imperfection_case = imperfectionCase.no,
                imperfection_caseSpecified = true,
                action_category = "ACTION_CATEGORY_SNOW_ICE_LOADS_H_GREATER_THAN_1000_M_QS",
            };
            load_case lcwWind = new()
            {
                no = 5,
                name = "My load case wind",
                self_weight_active = false,
                self_weight_activeSpecified = true,
                static_analysis_settings = analysis.no,
                static_analysis_settingsSpecified = true,
                analysis_type = load_case_analysis_type.ANALYSIS_TYPE_STATIC,
                analysis_typeSpecified = true,
                action_category = "ACTION_CATEGORY_WIND_QW",
            };

            load_case lcStability = new()
            {
                no = 6,
                name = "Stability",
                self_weight_active = true,
                self_weight_activeSpecified = true,
                static_analysis_settings = analysis.no,
                static_analysis_settingsSpecified = true,
                analysis_type = load_case_analysis_type.ANALYSIS_TYPE_STATIC,
                analysis_typeSpecified = true,
                calculate_critical_load = true,
                calculate_critical_loadSpecified = true,
                stability_analysis_settings = stabilityAnalysisSettings.no,
                stability_analysis_settingsSpecified = true,
                action_category = "ACTION_CATEGORY_PERMANENT_G",
            };

            relationship_between_load_cases_inclusive_load_cases_row relationshipBetweenLCInclusive = new relationship_between_load_cases_inclusive_load_cases_row()
            {
                no = 1,
                row = new relationship_between_load_cases_inclusive_load_cases()
                {
                    selected_load_cases = new int[] { selfWeightLC.no },
                    assigned_load_cases = new int[] { lcSnow1.no, lcWithImperfection.no, lcwWind.no, lcData.no },
                    comment = "",
                }
            };
            relationship_between_load_cases_exclusive_load_cases_row relationshipBetweenLCExclusive = new relationship_between_load_cases_exclusive_load_cases_row()
            {
                no = 1,
                row = new relationship_between_load_cases_exclusive_load_cases()
                {
                    selected_load_cases = new int[] { lcSnow1.no },
                    assigned_load_cases = new int[] { lcWithImperfection.no, lcwWind.no },
                    comment = "",
                }
            };
            relationship_between_load_cases relationshipBetweenLC = new relationship_between_load_cases()
            {
                no = 1,
                name = "Relationship between load cases scripted",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                inclusive_load_cases = new relationship_between_load_cases_inclusive_load_cases_row[] { relationshipBetweenLCInclusive },
                exclusive_load_cases = new relationship_between_load_cases_exclusive_load_cases_row[] { relationshipBetweenLCExclusive },
                comment = ""
            };

            design_situation design_Situation = new design_situation()
            {
                no = 1,
                name = "ScriptedDS",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                design_situation_type = "DESIGN_SITUATION_TYPE_EQU_PERMANENT_AND_TRANSIENT",
                is_generated = false,
                is_generatedSpecified = true,
                consider_inclusive_exclusive_load_cases = true,
                consider_inclusive_exclusive_load_casesSpecified = true,
                relationship_between_load_cases = relationshipBetweenLC.no,
                relationship_between_load_casesSpecified = true,

            };



            load_combination_items_row load_Combination_SW = new load_combination_items_row()
            {
                no = 1,
                row = new load_combination_items()
                {
                    load_case = 1,
                    load_caseSpecified = true,
                    factor = 1.35,
                    factorSpecified = true,
                }

            };

            load_combination_items_row load_Combination_lcData = new load_combination_items_row()
            {
                no = 2,
                row = new load_combination_items()
                {
                    load_case = 2,
                    load_caseSpecified = true,
                    factor = 1.5,
                    factorSpecified = true,
                }

            };
            load_combination_items_row[] loadCombinationItems = new load_combination_items_row[] { load_Combination_SW, load_Combination_lcData };



            load_combination load_Combination = new load_combination()
            {
                no = 1,
                name = "ScriptedCombination",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                to_solve = true,
                to_solveSpecified = true,
                analysis_type = load_combination_analysis_type.ANALYSIS_TYPE_STATIC,
                analysis_typeSpecified = true,
                items = loadCombinationItems,
                design_situation = 1,
                design_situationSpecified = true,
            };

            try
            {
                model.begin_modification("Set load cases and static analysis settings");
                model.set_load_case(selfWeightLC);
                model.set_load_case(lcData);
                model.set_load_case(lcSnow1);
                model.set_imperfection_case(imperfectionCase);
                model.set_load_case(lcStability);
                model.set_load_case(lcWithImperfection);
                model.set_load_case(lcwWind);
                model.set_relationship_between_load_cases(relationshipBetweenLC);
                model.set_design_situation(design_Situation);
                model.set_load_combination(load_Combination);



            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something happen when creation of load cases and static analysis settings" + exception.Message);
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of load cases and static analysis settings\n" + exception.Message + "\n");

                }
            }
            #endregion

            #endregion

            #region ModalAnalysisSetting
            modal_analysis_settings modal_Analysis_Settings = new modal_analysis_settings()
            {
                no = 1,
                name = "Generated via Script",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                comment = "ModalSettingsTest",
                number_of_modes_method = modal_analysis_settings_number_of_modes_method.NUMBER_OF_MODES_METHOD_USER_DEFINED,
                number_of_modes_methodSpecified = true,
                number_of_modes = 5,
                number_of_modesSpecified = true,
                solution_method = modal_analysis_settings_solution_method.METHOD_ROOT_OF_CHARACTERISTIC_POLYNOMIAL,
                solution_methodSpecified = true,
                mass_matrix_type = modal_analysis_settings_mass_matrix_type.MASS_MATRIX_TYPE_CONSISTENT,
                mass_conversion_typeSpecified = true,
                acting_masses_about_axis_x_enabled = true,
                acting_masses_about_axis_x_enabledSpecified = true,
                acting_masses_about_axis_y_enabled = true,
                acting_masses_about_axis_y_enabledSpecified = true,
                acting_masses_about_axis_z_enabled = true,
                acting_masses_about_axis_z_enabledSpecified = true,
                acting_masses_in_direction_x_enabled = true,
                acting_masses_in_direction_x_enabledSpecified = true,
                acting_masses_in_direction_y_enabled = true,
                acting_masses_in_direction_y_enabledSpecified = true,
                acting_masses_in_direction_z_enabled = true,
                acting_masses_in_direction_z_enabledSpecified = true,
                mass_conversion_type = modal_analysis_settings_mass_conversion_type.MASS_CONVERSION_TYPE_Z_COMPONENTS_OF_LOADS,

            };
            load_case lcModalAnalysis = new()
            {
                no = 10,
                name = "My load case for modal analysis",

                self_weight_active = false,
                self_weight_activeSpecified = true,
                modal_analysis_settings = modal_Analysis_Settings.no,
                modal_analysis_settingsSpecified = true,
                analysis_type = load_case_analysis_type.ANALYSIS_TYPE_MODAL,
                analysis_typeSpecified = true,
                import_masses_from = lcData.no,
                import_masses_fromSpecified = true,

            };
            try
            {
                model.begin_modification("Set MOS");
                model.set_modal_analysis_settings(modal_Analysis_Settings);
                model.set_load_case(lcModalAnalysis);
            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something happen when creation of load" + exception.Message);
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of load\n" + exception.Message + "\n");

                }
            }
            #endregion

            #region Wind load

            #region Wind profile
            wind_profile_user_defined_wind_profile_row wpOne = new wind_profile_user_defined_wind_profile_row()
            {
                no = 1,
                row = new wind_profile_user_defined_wind_profile()
                {
                    height = 0.0,
                    heightSpecified = true,
                    velocity = 22.0,
                    velocitySpecified = true,
                    turbulence_intensity = 0.01,
                    turbulence_intensitySpecified = true,
                }
            };
            wind_profile_user_defined_wind_profile_row wpTwo = new wind_profile_user_defined_wind_profile_row()
            {
                no = 2,
                row = new wind_profile_user_defined_wind_profile()
                {
                    height = 10.0,
                    heightSpecified = true,
                    velocity = 22.0,
                    velocitySpecified = true,
                    turbulence_intensity = 0.01,
                    turbulence_intensitySpecified = true,
                }
            };
            wind_profile windProfile = new wind_profile()
            {
                no = 1,
                name = "My wind profile",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                type = wind_profile_type.USER_DEFINED,
                typeSpecified = true,
                user_defined_wind_profile = new wind_profile_user_defined_wind_profile_row[] { wpOne, wpTwo },
                user_defined_wind_profile_sorted = true,
            };
            #endregion

            #region wind simulation analysis settings
            wind_simulation_analysis_settings WSAS = new wind_simulation_analysis_settings()
            {
                no = 1,
                name = "wind simulation analysis settings",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                kinematic_viscosity = 0.000015,
                kinematic_viscositySpecified = true,
                numerical_solver = wind_simulation_analysis_settings_numerical_solver.OPEN_FOAM,
                finite_volume_mesh_density = 0.2,
                finite_volume_mesh_densitySpecified = true,
                maximum_number_of_iterations = 500,
                maximum_number_of_iterationsSpecified = true,
                mesh_refinement_type = wind_simulation_analysis_settings_mesh_refinement_type.DISTANCE_FROM_SURFACE,
                mesh_refinement_typeSpecified = true,
                snap_to_model_edges = true,
                snap_to_model_edgesSpecified = true,
                boundary_layers_checked = false,
                boundary_layers_checkedSpecified = true,
                consider_turbulence = true,
                consider_turbulenceSpecified = true,
                slip_boundary_condition_on_bottom_boundary = false,
                slip_boundary_condition_on_bottom_boundarySpecified = true,
                use_second_order_numerical_scheme = false,
                use_second_order_numerical_schemeSpecified = true,
                user_defined_dimensions_of_wind_tunnel = false,
                user_defined_dimensions_of_wind_tunnelSpecified = true,
                member_load_distribution = wind_simulation_analysis_settings_member_load_distribution.CONCENTRATED,
                member_load_distributionSpecified = true,
                comment = "my wind",
            };
            #endregion

            #region Wind simulations

            wind_simulation WS = new wind_simulation()
            {
                no = 1,
                name = "wind simulation",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                comment = "my wind simulation settings",
                active = false,
                activeSpecified = true,
                wind_profile = 1,
                wind_profileSpecified = true,
                wind_simulation_analysis_settings = 1,
                wind_simulation_analysis_settingsSpecified = true,
                consider_initial_state = false,
                consider_initial_stateSpecified = true,
                wind_direction_type = wind_simulation_wind_direction_type.UNIFORM,
                wind_direction_typeSpecified = true,
                uniform_wind_direction_step = Math.PI,
                uniform_wind_direction_stepSpecified = true,
                uniform_wind_direction_range_start = 0.0,
                uniform_wind_direction_range_startSpecified = true,
                uniform_wind_direction_range_end = 2 * Math.PI,
                uniform_wind_direction_range_endSpecified = true,
            };
            #endregion

            try
            {
                model.begin_modification("Set wind properties");
                model.set_wind_profile(windProfile);
                model.set_wind_simulation_analysis_settings(WSAS);
                model.set_wind_simulation(WS);
            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something happen when creating wind properties" + exception.Message);
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of creating wind properties\n" + exception.Message + "\n");

                }
            }
            #endregion



            #region  loads
            List<member_load> memberLoad = new();
            // create member load
            foreach (var member in members)
            {
                member_load load = new()
                {
                    no = member.Value.no,
                    members_string = member.Value.no.ToString(),
                    members = new int[] { member.Value.no },
                    load_distribution = member_load_load_distribution.LOAD_DISTRIBUTION_TRAPEZOIDAL,
                    load_distributionSpecified = true,
                    magnitude = 30000,
                    magnitudeSpecified = true,
                    magnitude_1 = 10000,
                    magnitude_1Specified = true,
                    magnitude_2 = 20000,
                    magnitude_2Specified = true,
                    load_is_over_total_length = true,
                    load_is_over_total_lengthSpecified = true,
                };
                memberLoad.Add(load);
            }



            try
            {
                model.begin_modification("Set loads");
                memberLoad.ForEach(memberLoadItem => model.set_member_load(lcData.no, memberLoadItem));
            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something happen when creation of load" + exception.Message);
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of load\n" + exception.Message + "\n");

                }
            }

            #endregion


            // object info - information about "columns" in table in Web services - name / units ....
            // object_information objectInfo = model.get_object_information(object_types.E_OBJECT_TYPE_NODE);




            #region Spectral analysis settings
            spectral_analysis_settings spectral_Analysis_Settings = new spectral_analysis_settings()
            {
                no = 1,
                name = "MySRSS",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                combination_rule_for_periodic_responses = spectral_analysis_settings_combination_rule_for_periodic_responses.SRSS,
                combination_rule_for_periodic_responsesSpecified = true,
                combination_rule_for_directional_components = spectral_analysis_settings_combination_rule_for_directional_components.SRSS,
                combination_rule_for_directional_componentsSpecified = true,
                use_equivalent_linear_combination = true,
                use_equivalent_linear_combinationSpecified = true,
                comment = "My scripted settings"
            };
            try
            {
                model.begin_modification("Set SAS");
                model.set_spectral_analysis_settings(spectral_Analysis_Settings);
            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something happen when creation of SAS" + exception.Message);
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of SAS\n" + exception.Message + "\n");

                }
            }
            #endregion


            #region Accelerogram & Response Spectrum
            response_spectrum Response_Spectrum_Standard = new response_spectrum()
            {
                no = 1,
                definition_type = response_spectrum_definition_type.ACCORDING_TO_STANDARD,
                definition_typeSpecified = true,
                is_generated = false,
                is_generatedSpecified = true,

            };

            response_spectrum_user_defined_response_spectrum_row[] user_Defined_Response_Spectrum = new response_spectrum_user_defined_response_spectrum_row[10];
            for (int i = 0; i < 10; i++)
            {
                user_Defined_Response_Spectrum[i] = new response_spectrum_user_defined_response_spectrum_row()
                {
                    no = i + 1,
                    row = new response_spectrum_user_defined_response_spectrum()
                    {
                        acceleration = 0.03 + i * 0.01, // as test example
                        accelerationSpecified = true,
                        frequency = 100 - 5 * i, // as test example
                        frequencySpecified = true,
                        period = 0.01 + 0.006 * i, // as test example
                        periodSpecified = true,
                    }
                };

            }

            response_spectrum Response_Spectrum_User = new response_spectrum()
            {
                no = 2,
                definition_type = response_spectrum_definition_type.USER_DEFINED,
                definition_typeSpecified = true,
                user_defined_response_spectrum = user_Defined_Response_Spectrum,
                is_generated = false,
                is_generatedSpecified = true,

            };

            accelerogram Accelerogram = new accelerogram()
            {
                no = 1,
                definition_type = accelerogram_definition_type.FROM_LIBRARY,
                definition_typeSpecified = true,
                library_id = 674,
                library_idSpecified = true,
                name = "Wutoschingen, Germany - station: Zurich-Degenried",
            };
            response_spectrum Response_Spectrum_Accelerogram = new response_spectrum()
            {
                no = 3,
                definition_type = response_spectrum_definition_type.GENERATED_FROM_ACCELEROGRAM,
                definition_typeSpecified = true,
                accelerogram = Accelerogram.no,
                accelerogramSpecified = true,
                damping = 0.05,
                dampingSpecified = true,
                direction = response_spectrum_direction.DIRECTION_X,
                directionSpecified = true,
                max_t = 10,
                max_tSpecified = true,
                min_t = 0.01,
                min_tSpecified = true,
                sample_count = 120,
                sample_countSpecified = true,
            };

            accelerogram_user_defined_accelerogram_row[] accelerogrampoints = new accelerogram_user_defined_accelerogram_row[10];
            for (int i = 0; i < 10; i++)
            {
                accelerogrampoints[i] = new accelerogram_user_defined_accelerogram_row()
                {
                    no = i + 1,
                    row = new accelerogram_user_defined_accelerogram()
                    {
                        acceleration_x = 0.0 + i * 0.01, //test data
                        acceleration_xSpecified = true,
                        acceleration_y = 0.0 + i * 0.05, //test data
                        acceleration_ySpecified = true,
                        acceleration_z = 0.0 + i * 0.06, //test data
                        time = 0.0 + 0.001 * i,
                        timeSpecified = true,
                    }
                };
            }
            accelerogram AccelerogramUserDefined = new accelerogram()
            {
                no = 2,
                definition_type = accelerogram_definition_type.USER_DEFINED,
                definition_typeSpecified = true,
                user_defined_accelerogram = accelerogrampoints,
            };

            // load case for spectral analysis
            load_case LCSpectral = new load_case()
            {
                no = 20,
                name = "Spectral",
                static_analysis_settings = analysis.no,
                analysis_type = load_case_analysis_type.ANALYSIS_TYPE_RESPONSE_SPECTRUM,
                analysis_typeSpecified = true,
                spectral_analysis_settings = spectral_Analysis_Settings.no,
                spectral_analysis_settingsSpecified = true,
                import_modal_analysis_from = lcModalAnalysis.no,
                import_modal_analysis_fromSpecified = true,
                response_spectrum_is_enabled_in_any_direction = true,
                response_spectrum_is_enabled_in_any_directionSpecified = true,
                response_spectrum_is_enabled_in_direction_x = false,
                response_spectrum_is_enabled_in_direction_xSpecified = true,
                response_spectrum_is_enabled_in_direction_y = false,
                response_spectrum_is_enabled_in_direction_ySpecified = true,
                response_spectrum_is_enabled_in_direction_z = true,
                response_spectrum_is_enabled_in_direction_zSpecified = true,
                response_spectrum_in_direction_z = Response_Spectrum_Standard.no,
                response_spectrum_in_direction_zSpecified = true,
                response_spectrum_scale_factor_in_direction_x = 1,
                response_spectrum_scale_factor_in_direction_xSpecified = true,
                response_spectrum_scale_factor_in_direction_y = 1,
                response_spectrum_scale_factor_in_direction_ySpecified = true,
                response_spectrum_scale_factor_in_direction_z = 1,
                response_spectrum_scale_factor_in_direction_zSpecified = true,
                response_spectrum_rotation_angle = 0,
                response_spectrum_rotation_angleSpecified = true,
                response_spectrum_and_equivalent_load_consider_accidental_torsion = false,
                response_spectrum_and_equivalent_load_consider_accidental_torsionSpecified = true,
            };

            try
            {
                model.begin_modification("Set response spectrum and accelerogram");
                model.set_response_spectrum(Response_Spectrum_Standard);
                model.set_response_spectrum(Response_Spectrum_User);
                model.set_accelerogram(Accelerogram);
                model.set_response_spectrum(Response_Spectrum_Accelerogram);
                model.set_accelerogram(AccelerogramUserDefined);
                model.set_load_case(LCSpectral);
            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something happen when creation of spectrum and accelerogram" + exception.Message);
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something happen when creation of spectrum and accelerogram\n" + exception.Message + "\n");

                }
            }
            #endregion

            #region Mesh settings
            //meshConfig meshConfigTest = model.get_mesh_settings();
            meshConfig meshConfig = new meshConfig()
            {
                SurfacesMeshQualityConfig = new SurfacesMeshQualityConfig()
                {
                    mesh_quality_color_indicator_warning_color = "#00ff00",//"#00ff00"
                    mesh_quality_color_indicator_failure_color = "#00ff00",
                    mesh_quality_color_indicator_ok_color = "#00ff00",
                    QualityCriteriaConfigForSurfaces = new QualityCriteriaConfigForSurfaces()
                    {
                        quality_criterion_check_aspect_ratio = true,
                        quality_criterion_check_aspect_ratio_warning = 20,
                        quality_criterion_check_aspect_ratio_failure = 100,
                        quality_criterion_parallel_deviations = true,
                        quality_criterion_parallel_deviations_warning = 1.7453292519943295,
                        quality_criterion_parallel_deviations_failure = 2.792526803190927,
                        quality_criterion_corner_angles_of_triangle_elements = true,
                        quality_criterion_corner_angles_of_triangle_elements_warning = 1.8325957145940461,
                        quality_criterion_corner_angles_of_triangle_elements_failure = 3.0543261909900767,
                        quality_criterion_corner_angles_of_quadrangle_elements = true,
                        quality_criterion_corner_angles_of_quadrangle_elements_warning = 2.705260340591211,
                        quality_criterion_corner_angles_of_quadrangle_elements_failure = 3.0543261909900767,
                        quality_criterion_warping_of_membrane_elements = true,
                        quality_criterion_warping_of_membrane_elements_warning = 0.01,
                        quality_criterion_warping_of_membrane_elements_failure = 0.2,
                        quality_criterion_warping_of_non_membrane_elements = true,
                        quality_criterion_warping_of_non_membrane_elements_warning = 1,
                        quality_criterion_warping_of_non_membrane_elements_failure = 4,
                        quality_criterion_jacobian_ratio = true,
                        quality_criterion_jacobian_ratio_warning = 30,
                        quality_criterion_jacobian_ratio_failure = 800,
                    },
                },
                // always set this:
                general_target_length_of_fe = 0.5,
                members_number_of_divisions_for_special_types = 10,
                members_number_of_divisions_for_result_diagram = 10,
                members_number_of_divisions_for_min_max_values = 10,
                members_number_of_divisions_for_concrete_members = 10,
                surfaces_maximum_ratio_of_fe = 1.8,
                solids_maximum_number_of_elements = 200,
                solids_target_length_of_fe_for_type_soil = 1,
            };
            try
            {
                model.begin_modification("Set mesh settings");
                model.set_mesh_settings(meshConfig);
            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something happen when creation of mesh settings: " + exception.Message);
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of  mesh settings: \n" + exception.Message + "\n");
                    // 
                }
            }
            #endregion

            #region Calculation diagrams
            calculation_diagram calcDiagram = new calculation_diagram()
            {
                no = 1,
                name = "MyScriptedDiagram",
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                comment = "My scripted diagram",
                diagram_type = calculation_diagram_diagram_type.TYPE_2D_BASIC,
                diagram_typeSpecified = true,
                analysis_type = calculation_diagram_analysis_type.ANALYSIS_TYPE_STATIC,
                analysis_typeSpecified = true,
                @case = 1, // loadcase
                caseSpecified = true,
                horizontal_axis_result_type = calculation_diagram_horizontal_axis_result_type.SUPPORT_REACTIONS_NODAL_SUPPORTS,
                horizontal_axis_result_typeSpecified = true,
                horizontal_axis_value = calculation_diagram_horizontal_axis_value.SUPPORT_REACTIONS_NODAL_SUPPORTS_GLOBAL_P_Z,
                horizontal_axis_valueSpecified = true,
                horizontal_axis_object = 19,
                horizontal_axis_objectSpecified = true,
                vertical_axis_result_type = calculation_diagram_vertical_axis_result_type.SUPPORT_REACTIONS_NODAL_SUPPORTS,
                vertical_axis_result_typeSpecified = true,
                vertical_axis_value = calculation_diagram_vertical_axis_value.SUPPORT_REACTIONS_NODAL_SUPPORTS_GLOBAL_M_Y,
                vertical_axis_valueSpecified = true,
                vertical_axis_object = 19,
                vertical_axis_objectSpecified = true,

            };
            try
            {
                model.begin_modification("Calcualtion diagram");
                model.set_calculation_diagram(calcDiagram);

            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something happen when creation of Calcualtion diagram" + exception.Message);
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of Calcualtion diagram" + exception.Message + "\n");

                }
            }
            #endregion

            load_cases_and_combinations loadCasesAndCombinations = model.get_load_cases_and_combinations();
            loadCasesAndCombinations.result_combinations_active = true;
            loadCasesAndCombinations.result_combinations_activeSpecified = true;
            try
            {
                model.begin_modification("Result combinations");
                model.set_load_cases_and_combinations(loadCasesAndCombinations);

            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something happen when creation of Result combinations" + exception.Message);
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of Result combinations" + exception.Message + "\n");

                }
            }
            result_combination resultCombination = new result_combination()
            {
                no = 1,
                design_situation = design_Situation.no,
                design_situationSpecified = true,
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                name = "LC1 + LC2 + LC3 + LC4 + LC5 + LC6 + LC20 + CO1",
                to_solve = true,
                to_solveSpecified = true,
                comment = "scripted result combination",
                combination_type = result_combination_combination_type.COMBINATION_TYPE_GENERAL,
                combination_typeSpecified = true,
                srss_combination = false,
                srss_combinationSpecified = true,
                items = new result_combination_items_row[]{
                    new result_combination_items_row(){
                        no = 1,
                        description = "",
                        row = new  result_combination_items(){
                             case_object_item = 1,
                            case_object_itemSpecified = true,
                            operator_type = operator_type.OPERATOR_AND,
                            operator_typeSpecified = true,
                            case_object_factor= 1,
                            case_object_load_type = case_object_load_type.LOAD_TYPE_TRANSIENT,
                            case_object_load_typeSpecified = true,
                        }
                    },
                    new result_combination_items_row(){
                        no = 2,
                        description = "",
                        row = new  result_combination_items(){
                            case_object_item = 2,
                            case_object_itemSpecified = true,
                            operator_type = operator_type.OPERATOR_AND,
                            operator_typeSpecified = true,
                            case_object_factor= 1,
                            case_object_load_type = case_object_load_type.LOAD_TYPE_TRANSIENT,
                            case_object_load_typeSpecified = true,
                        }
                    },
                    new result_combination_items_row(){
                        no = 3,
                        description = "",
                        row = new  result_combination_items(){
                            case_object_item = 3,
                            case_object_itemSpecified = true,
                            operator_type = operator_type.OPERATOR_AND,
                            operator_typeSpecified = true,
                            case_object_factor= 1,
                            case_object_load_type = case_object_load_type.LOAD_TYPE_TRANSIENT,
                            case_object_load_typeSpecified = true,
                         }
                    },
                    new result_combination_items_row(){
                        no = 4,
                        description = "",
                        row = new  result_combination_items(){
                             case_object_item = 4,
                            case_object_itemSpecified = true,
                            operator_type = operator_type.OPERATOR_AND,
                            operator_typeSpecified = true,
                            case_object_factor= 1,
                            case_object_load_type = case_object_load_type.LOAD_TYPE_TRANSIENT,
                            case_object_load_typeSpecified = true,
                        }
                   },
                    new result_combination_items_row(){
                        no = 5,
                        description = "",
                        row = new  result_combination_items(){
                             case_object_item = 5,
                            case_object_itemSpecified = true,
                            operator_type = operator_type.OPERATOR_AND,
                            operator_typeSpecified = true,
                            case_object_factor= 1,
                            case_object_load_type = case_object_load_type.LOAD_TYPE_TRANSIENT,
                            case_object_load_typeSpecified = true,
                         }
                   },
                    new result_combination_items_row(){
                        no = 6,
                        description = "",
                        row = new  result_combination_items(){
                           case_object_item = 6,
                            case_object_itemSpecified = true,
                            operator_type = operator_type.OPERATOR_AND,
                            operator_typeSpecified = true,
                            case_object_factor= 1,
                            case_object_load_type = case_object_load_type.LOAD_TYPE_TRANSIENT,
                            case_object_load_typeSpecified = true,
                         }
                   },
                    new result_combination_items_row(){
                        no = 7,
                        description = "",
                        row = new  result_combination_items(){
                            case_object_item = 20,
                            case_object_itemSpecified = true,
                            operator_type = operator_type.OPERATOR_AND,
                            operator_typeSpecified = true,
                            case_object_factor= 1,
                            case_object_load_type = case_object_load_type.LOAD_TYPE_TRANSIENT,
                            case_object_load_typeSpecified = true,
                         }
                   },
                    new result_combination_items_row(){
                        no = 8,
                        description = "",
                        row = new  result_combination_items(){
                             case_object_item = 1,
                            case_object_itemSpecified = true,
                            operator_type = operator_type.OPERATOR_NONE,
                            operator_typeSpecified = true,
                            case_object_factor= 1,
                            case_object_load_type = case_object_load_type.LOAD_TYPE_TRANSIENT,
                            case_object_load_typeSpecified = true,
                        }
                    }
                },
                generate_subcombinations = false,
                generate_subcombinationsSpecified = true,
                load_duration = "LOAD_DURATION_PERMANENT",
            };
            try
            {
                model.begin_modification("Set Result combinations");
                model.set_result_combination(resultCombination);

            }
            catch (Exception exception)
            {
                model.cancel_modification();
                logger.Error(exception, "Something happen when creation of Set Result combinations" + exception.Message);
                throw;
            }
            finally
            {
                try
                {
                    model.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in finish modification of Set Result combinations" + exception.Message + "\n");

                }
            }

            // calculation_message[] calculationMesagesAll = model.calculate_all(false);

            // if (calculationMesagesAll.Length != 0)
            // {
            // }
            // else
            // {
            //     Console.WriteLine("Calculation finished successfully");
            // }


            #region generate mesh and get mesh statistics
            calculation_result meshGenerationMessage = model.generate_mesh(true);
            mesh_statistics_type mesh_Statistics = model.get_mesh_statistics();
            Console.WriteLine("Number of mesh nodes: " + mesh_Statistics.node_elements);
            Console.WriteLine("Number of 1D elements: " + mesh_Statistics.member_1D_finite_elements);
            Console.WriteLine("Number of surface element: " + mesh_Statistics.surface_2D_finite_elements);
            Console.WriteLine("Number of volume elements: " + mesh_Statistics.solid_3D_finite_elements);

            #endregion

            #region  Calculate specific cases
            // calculate_specific_objects_element
            calculate_specific_loading element_sw = new calculate_specific_loading()
            {
                type = case_object_types.E_OBJECT_TYPE_LOAD_CASE,
                no = selfWeightLC.no,
            };
            calculate_specific_loading element_lc2 = new calculate_specific_loading()
            {
                type = case_object_types.E_OBJECT_TYPE_LOAD_CASE,
                no = lcData.no,
            };
            calculate_specific_loading element_modal = new calculate_specific_loading()
            {
                type = case_object_types.E_OBJECT_TYPE_LOAD_CASE,
                no = lcModalAnalysis.no,
            };
            calculate_specific_loading element_spectral = new calculate_specific_loading()
            {
                type = case_object_types.E_OBJECT_TYPE_LOAD_CASE,
                no = LCSpectral.no,
            };
            calculate_specific_loading element_stability = new calculate_specific_loading()
            {
                type = case_object_types.E_OBJECT_TYPE_LOAD_CASE,
                no = lcStability.no,
            };
            calculate_specific_loading element_imperfections = new calculate_specific_loading()
            {
                type = case_object_types.E_OBJECT_TYPE_LOAD_CASE,
                no = lcWithImperfection.no,
            };
            calculate_specific_loading element_loadCase = new calculate_specific_loading()
            {
                type = case_object_types.E_OBJECT_TYPE_LOAD_COMBINATION,
                no = 1,
            };


            calculate_specific_loading[] elementsToCalculate = new calculate_specific_loading[] { element_sw, element_lc2, element_loadCase, element_modal, element_spectral, element_stability, element_imperfections };

            calculation_result calculationResult = model.calculate_specific(elementsToCalculate, true);

            if (!calculationResult.succeeded || !String.IsNullOrEmpty(calculationResult.messages) || calculationResult.errors_and_warnings.Any())
            {
                Console.Write("Calculation is not finished successfully");
                if (!String.IsNullOrEmpty(calculationResult.messages))
                {
                    Console.Write(calculationResult.messages);
                }
                if (calculationResult.errors_and_warnings.Any())
                {
                    foreach (calculation_message message in calculationResult.errors_and_warnings)
                    {
                        Console.Write($"{message.message_type.ToString()}  {message.message} {(message.input_field != null ? message.input_field : "")} {(message.@object != null ? message.@object : "")} {(message.current_value != null ? message.current_value : "")} {message.result.ToString()}");
                    }

                }
                if (!String.IsNullOrEmpty(calculationResult.messages) && !calculationResult.errors_and_warnings.Any())
                {
                    errors_row[] errors = model.get_calculation_errors();
                    if (errors.Any())
                    {
                        foreach (errors_row error in errors)
                        {
                            Console.Write($"{error.no} {error.description}  {error.row.analysis_type} {error.row.description} {error.row.error_or_warning_number} {error.row.@object}");
                        }
                    }
                }
            }

            #endregion


            #region Results
            bool modelHasAnyResults = model.has_any_results();

            if (modelHasAnyResults)
            {
                Console.WriteLine("Model has results");
            }
            else
            {
                Console.WriteLine("Model has no results");
            }

            bool modelHasLC2Calculated = model.has_results(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lcData.no);
            if (modelHasLC2Calculated)
            {
                Console.WriteLine("Model has LC2 results");
            }
            else
            {
                Console.WriteLine("Model has no LC2 results");
            }

            #region Calculation diagram
            calculation_diagram resultcalcDiagram = model.get_calculation_diagram(calcDiagram.no);
            Console.WriteLine("Calculation diagram 1");
            foreach (var tableRow in resultcalcDiagram.table)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t", tableRow.row.iteration_no.ToString(), tableRow.row.load_factor.ToString(), tableRow.row.horizontal_axis_value.ToString(), tableRow.row.vertical_axis_value.ToString());
            }
            #endregion


            model.use_detailed_member_results(true); // results along the length of the member, by default false -> results just at the begingign and end of the member + exteremes


            summary_row[] staticAnalysisSummary = model.get_results_for_summary(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lcData.no);
            Console.WriteLine("Summary static analysis");
            foreach (var item in staticAnalysisSummary)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", item.row.description, item.row.value != null ? item.row.value.value : "NAN", item.row.units, item.row.notes);
            }

            var object_location = model.get_all_selected_objects();

            members_internal_forces_row[] internalForcesMember1 = model.get_results_for_members_internal_forces(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lcData.no, object_location, axes_type.MEMBER_AXES); // 1 is the no of the member
            Console.WriteLine("Internal forces for member");

            foreach (var item in internalForcesMember1)
            {
                Console.WriteLine("Row no {0}\t Description {1}", item.no, item.description);
                Console.WriteLine("Node {0}\t Location {1}\t Location flags {2}\t Internal force label {3}\t Specification {4}", item.row.node_no != 0, item.row.location, item.row.location_flags, item.row.internal_force_label, item.row.specification);
                Console.WriteLine("N {0}\t Vy {1}\t Vz {2}\t Mx {3}\t My {4}\t Mz {5}\t", item.row.internal_force_n.ToString(), item.row.internal_force_vy.ToString(), item.row.internal_force_vz.ToString(), item.row.internal_force_mt.ToString(), item.row.internal_force_my.ToString(), item.row.internal_force_mz.ToString());

            }
            Console.WriteLine("Global deformations for member");
            members_global_deformations_row[] globalDeformationsMember1 = model.get_results_for_members_global_deformations(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lcData.no, object_location); // 1 is the no of the member

            foreach (var item in globalDeformationsMember1)
            {
                Console.WriteLine("Row no {0}\t Description {1}", item.no, item.description);
                Console.WriteLine("Node {0}\t Location {1}\t Location flags {2}\t Deformation label {3}\t Specification {4}", item.row.node_no != 0, item.row.location, item.row.location_flags, item.row.deformation_label, item.row.specification);
                Console.WriteLine("ux {0}\t uy {1}\t uz {2}\t utot {3}\t rx {4}\t ry {5}\t rz {6}\t warping {6}\t", item.row.displacement_x.ToString(), item.row.displacement_y.ToString(), item.row.displacement_z.ToString(), item.row.displacement_absolute.ToString(), item.row.rotation_x.ToString(), item.row.rotation_y.ToString(), item.row.rotation_z.ToString(), item.row.warping.ToString());

            }

            Console.WriteLine("Internal forces for surface");
            surfaces_basic_internal_forces_row[] internalForcesSurface1 = model.get_results_for_surfaces_basic_internal_forces(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lcData.no, object_location); // 1 is the no of the surface
            foreach (var item in internalForcesSurface1)
            {
                Console.WriteLine("Row no {0}\t Description {1}", item.no, item.description);
                Console.WriteLine("Grid point {0}\t Grid point X {1}\t Grid point Y {2}\t  Grid point Z {3}\t layer {4}\t Internal force label {5}\t Specification {6}", item.row.grid_point != 0,
                item.row.grid_point_coordinate_x != 0.0, item.row.grid_point_coordinate_y,
                item.row.grid_point_coordinate_z != 0.0, item.row.layer, item.row.basic_internal_force_label, item.row.specification);
                Console.WriteLine("nx {0}\t ny {1}\t nxy {2}\t  vx {3}\t vy {4}\t mx {5}\t my {6}\t mxy {7}", item.row.axial_force_nx, item.row.axial_force_ny, item.row.axial_force_nxy,
                item.row.shear_force_vx, item.row.shear_force_vy, item.row.moment_mx, item.row.moment_my, item.row.moment_mxy);

            }
            nodes_deformations_row[] nodeDeformations = model.get_results_for_nodes_deformations(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lcData.no, object_location);//all nodes -> 0
            Console.WriteLine("Node deformations");
            foreach (var item in nodeDeformations)
            {
                Console.WriteLine("Row no {0}\t Description {1} node comment {2}", item.no, item.description, item.row.specification);
                Console.WriteLine("ux {0}\t uy {1}\t uz {2}\t utot {3}\t rx {4}\t ry {5}\t rz {6}\t", item.row.displacement_x.ToString(), item.row.displacement_y.ToString(), item.row.displacement_z.ToString(), item.row.displacement_absolute.ToString(), item.row.rotation_x.ToString(), item.row.rotation_y.ToString(), item.row.rotation_z.ToString());

            }
            nodes_support_forces_row[] nodeReactions = model.get_results_for_nodes_support_forces(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lcData.no, object_location); //all nodes -> 0
            Console.WriteLine("Node reactions");
            foreach (var item in nodeReactions)
            {
                Console.WriteLine("Row no {0}\t Description {1}", item.no, item.description);
                Console.WriteLine("note corresponding loading {0}\t px {1}\t py {2}\t pz {3}\t mx {4}\t my {5}\t mz {6}\t label {7}\t", item.row.node_no, item.row.support_force_p_x.ToString(), item.row.support_force_p_y.ToString(), item.row.support_force_p_z.ToString(), item.row.support_moment_m_x.ToString(), item.row.support_moment_m_y.ToString(), item.row.support_moment_m_z.ToString(), item.row.support_forces_label);

            }
            #endregion

            //modal analysis
            bool modelHasModalCalculated = model.has_results(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lcModalAnalysis.no);
            if (modelHasModalCalculated)
            {
                Console.WriteLine("Model has modal results");
            }
            else
            {
                Console.WriteLine("Model has no modal results");
            }
            modal_analysis_natural_frequencies_row[] modalFrequencies = model.get_results_for_modal_analysis_natural_frequencies(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lcModalAnalysis.no, object_location);

            modal_analysis_effective_modal_masses_row[] effectiveModalMass = model.get_results_for_modal_analysis_effective_modal_masses(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lcModalAnalysis.no, object_location);

            modal_analysis_participation_factors_row[] participationFactors = model.get_results_for_modal_analysis_participation_factors(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lcModalAnalysis.no, object_location);

            modal_analysis_masses_in_locations_row[] massesInMeshPoints = model.get_results_for_modal_analysis_masses_in_locations(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lcModalAnalysis.no, object_location);

            modal_analysis_mode_shapes_by_node_row[] modalAnalysisModeShapesByNode = model.get_results_for_modal_analysis_mode_shapes_by_node(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lcModalAnalysis.no, object_location);//node number
            Console.WriteLine("Mode shapes for nodes");
            foreach (var item in modalAnalysisModeShapesByNode)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}", item.no, item.description, item.row.mode_no, item.row.displacement_x, item.row.displacement_y, item.row.displacement_z, item.row.rotation_x, item.row.rotation_y, item.row.rotation_z, item.row.comment);
            }
            modal_analysis_nodes_by_mode_shape_row[] modalAnalysisNodesByModalShape = model.get_results_for_modal_analysis_nodes_by_mode_shape(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lcModalAnalysis.no, object_location);//??
            foreach (var item in modalAnalysisNodesByModalShape)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}", item.no, item.description, item.row.node_no, item.row.displacement_x, item.row.displacement_y, item.row.displacement_z, item.row.rotation_x, item.row.rotation_y, item.row.rotation_z, item.row.comment);
            }

            modal_analysis_members_by_mode_shape_row[] modalAnalysismembersByModalShape = model.get_results_for_modal_analysis_members_by_mode_shape(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lcModalAnalysis.no, object_location);//??

            //spectral analysis
            bool modelHasSpectralCalculated = model.has_results(case_object_types.E_OBJECT_TYPE_LOAD_CASE, LCSpectral.no);
            if (modelHasSpectralCalculated)
            {
                Console.WriteLine("Model has spectral results");
            }
            else
            {
                Console.WriteLine("Model has no spectral results");
            }
            Console.WriteLine("Summary spectral analysis");
            /*
            var envelope = new spectral_analysis_envelope
            {
                envelope_type = spectral_analysis_envelope_type.SPECTRAL_ANALYSIS_SRSS,
                envelope_typeSpecified = true,
            };
            
            spectral_analysis_summary_row[] spectralAnalysisOverviewSummary = model.get_results_for_spectral_analysis_summary(case_object_types.E_OBJECT_TYPE_LOAD_CASE, LCSpectral.no, object_location, envelope);
            foreach (var item in spectralAnalysisOverviewSummary)
            {
                Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", item.row.description, item.row.value != null ? item.row.value.value : "NAN", item.row.units, item.row.notes);
            }

            spectral_analysis_nodes_deformations_row[] spectralAnalysisDeformations = model.get_results_for_spectral_analysis_nodes_deformations(case_object_types.E_OBJECT_TYPE_LOAD_CASE, LCSpectral.no, object_location, envelope);
            foreach (var item in spectralAnalysisDeformations)
            {
                Console.WriteLine("Row no {0}\t Description {1}", item.no, item.description);
                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}", item.row.displacement_x, item.row.displacement_y, item.row.displacement_z, item.row.displacement_absolute, item.row.rotation_x, item.row.rotation_y, item.row.rotation_z, item.row.specification);
            }
            spectral_analysis_nodes_pseudo_accelerations_row[] spectralAnalysisPseudoAccNodes = model.get_results_for_spectral_analysis_nodes_pseudo_accelerations(case_object_types.E_OBJECT_TYPE_LOAD_CASE, LCSpectral.no, object_location, envelope);
            foreach (var item in spectralAnalysisPseudoAccNodes)
            {
                Console.WriteLine("Row no {0}\t Description {1}", item.no, item.description);
                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}", item.row.acceleration_absolute, item.row.acceleration_x, item.row.acceleration_y, item.row.acceleration_z, item.row.angular_acceleration_x, item.row.angular_acceleration_y, item.row.angular_acceleration_z, item.row.specification);
            }
            spectral_analysis_nodes_pseudo_velocities_row[] spectralAnalysisPseudoVeloNodes = model.get_results_for_spectral_analysis_nodes_pseudo_velocities(case_object_types.E_OBJECT_TYPE_LOAD_CASE, LCSpectral.no, object_location, envelope);
            foreach (var item in spectralAnalysisPseudoVeloNodes)
            {
                Console.WriteLine("Row no {0}\t Description {1}", item.no, item.description);
                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}", item.row.velocity_absolute, item.row.velocity_x, item.row.velocity_y, item.row.velocity_z, item.row.angular_velocity_x, item.row.angular_velocity_y, item.row.angular_velocity_z, item.row.specification);
            }
            */
            //stability
            bool modelHasStabilityCalculated = model.has_results(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lcStability.no);
            if (modelHasStabilityCalculated)
            {
                Console.WriteLine("Model has stability results");
            }
            else
            {
                Console.WriteLine("Model has no stability results");
            }
            stability_incremental_analysis_summary_row[] stabilitySummary = model.get_results_for_stability_incremental_analysis_summary(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lcStability.no); //??

            critical_load_factors_row[] criticalLoadFactors = model.get_results_for_critical_load_factors(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lcStability.no, object_location); //??

            stability_incremental_analysis_nodes_deformations_row[] stabilityShapes = model.get_results_for_stability_incremental_analysis_nodes_deformations(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lcStability.no, object_location);

            // stability

            #region model info statuses
            modelInfo modelInfo = model.get_model_info();
            Console.WriteLine("Number of lines in model: {0}", modelInfo.property_line_count.ToString());
            Console.WriteLine("Number of nodes in model: {0}", modelInfo.property_node_count.ToString());
            Console.WriteLine("Number of members in model: {0}", modelInfo.property_member_count.ToString());
            Console.WriteLine("Number of surfaces in model: {0}", modelInfo.property_surface_count.ToString());
            Console.WriteLine("Number of solids in model: {0}", modelInfo.property_solid_count.ToString());
            Console.WriteLine("Weight of model: {0}", modelInfo.property_weight.ToString());
            Console.WriteLine("Number of load cases: {0}", modelInfo.property_lc_count.ToString());
            Console.WriteLine("Number of load cases: {0}", modelInfo.property_co_count.ToString());
            Console.WriteLine("Number of result classes: {0}", modelInfo.property_rc_count.ToString());
            Console.WriteLine("Is model calculated?: {0}", modelInfo.property_has_results ? "Yes" : "No");
            Console.WriteLine("Has model printout report?: {0}", modelInfo.property_has_printout_report ? "Yes" : "No");
            Console.WriteLine("Dimensions: {0} , {1}, {2}", modelInfo.property_dimensions.x.ToString(), modelInfo.property_dimensions.y.ToString(), modelInfo.property_dimensions.z.ToString());
            #endregion

            #region Generate part list
            model.generate_parts_lists();
            parts_list_all_by_material_row[] partListByAllMaterial = model.get_parts_list_all_by_material();
            foreach (var item in partListByAllMaterial)
            {
                if (!item.description.Contains("Total"))
                {//material no
                    Console.WriteLine("Material no: {0}\t Material name: {1}\t object type: {2}\t coating:{3}\t volume: {4}\t mass: {5}", item.description, item.row.material_name, item.row.object_type, item.row.total_coating, item.row.volume, item.row.mass);
                }
                else
                {//material no total
                    Console.WriteLine("Material total\t \t \t coating:{0}\t volume: {1}\t mass: {2}", item.row.total_coating, item.row.volume, item.row.mass);
                }

            }
            Console.WriteLine("Members: ");
            parts_list_members_by_material_row[] partListMemberByMaterial = model.get_parts_list_members_by_material();
            foreach (var item in partListMemberByMaterial)
            {
                if (!item.description.Contains("Total"))
                {
                    Console.WriteLine("Material no: {0}\t Material name: {1}\t cross_section: {2}\t members no:{3}\t quantity: {4}\t length: {5}\t unit surface area: {6}\t volume: {7}\t unit mass: {8}\t member mass: {9}\t total length: {10}\t total surface area: {11}\t total volume:{12}\t total mass:{13}",
                    item.description, item.row.material_name, item.row.section_name, item.row.members_no, item.row.quantity, item.row.length, item.row.unit_surface_area, item.row.volume, item.row.unit_mass, item.row.member_mass, item.row.total_length, item.row.total_surface_area, item.row.total_volume, item.row.total_mass);
                }
                else
                {
                    Console.WriteLine("Total \t \t \t \t quantity: {4}\t length: {5}\t unit surface area: {6}\t volume: {7}\t unit mass: {8}\t member mass: {9}\t total length: {10}\t total surface area: {11}\t total volume:{12}\t total mass:{13}",
                 item.description, item.row.material_name, item.row.section_name, item.row.members_no, item.row.quantity, item.row.length, item.row.unit_surface_area, item.row.volume, item.row.unit_mass, item.row.member_mass, item.row.total_length, item.row.total_surface_area, item.row.total_volume, item.row.total_mass);

                }

            }
            parts_list_member_representatives_by_material_row[] partListMemberRepresentativesByMaterial = model.get_parts_list_member_representatives_by_material();
            foreach (var item in partListMemberRepresentativesByMaterial)
            {
                Console.WriteLine("Row: {0}\t Material: {1}\t member representative no: {2}\t quantity: {3}\t length:{4}\t volume: {5}\t mass: {6}", item.description, item.row.material_name, item.row.member_representative_no, item.row.quantity, item.row.length, item.row.volume, item.row.member_mass);
            }
            parts_list_member_sets_by_material_row[] partListMemberSetsByMaterial = model.get_parts_list_member_sets_by_material();
            parts_list_member_set_representatives_by_material_row[] partListMemberSetRepresentativesByMaterial = model.get_parts_list_member_set_representatives_by_material();
            parts_list_solids_by_material_row[] partListSolidsByMaterial = model.get_parts_list_solids_by_material();
            parts_list_surfaces_by_material_row[] partListSurfacesByMaterial = model.get_parts_list_surfaces_by_material();
            Console.WriteLine("Surfaces: ");
            foreach (var item in partListSurfacesByMaterial)
            {
                if (!item.description.Contains("Total"))
                {
                    Console.WriteLine("Material no: {0}\t Material: {1}\t depthField: {2}\t surfaces no:{3}\t quantity: {4}\t surface: {5}\t coating: {6}\t volume: {7}\t unit mass: {8}\t surface mass: {9}\t total coating: {10}\t total surface: {11}\t total volume:{12}\t total mass:{13} ",
                        item.description, item.row.material_name, item.row.thickness_name, item.row.surfaces_no, item.row.quantity, item.row.surface_area, item.row.coating, item.row.volume, item.row.unit_mass, item.row.surface_mass, item.row.total_coating, item.row.total_surface_area, item.row.total_volume, item.row.total_mass);
                }
                else
                {
                    Console.WriteLine("Total \t \t t quantity: {4}\t surface: {5}\t coating: {6}\t volume: {7}\t unit mass: {8}\t surface mass: {9}\t total coating: {10}\t total surface: {11}\t total volume:{12}\t total mass:{13} ",
                        item.description, item.row.material_name, item.row.thickness_name, item.row.surfaces_no, item.row.quantity, item.row.surface_area, item.row.coating, item.row.volume, item.row.unit_mass, item.row.surface_mass, item.row.total_coating, item.row.total_surface_area, item.row.total_volume, item.row.total_mass);

                }
            }
            #endregion

            #region report
            //model.export_printout_report_to_pdf(1, ExamplesDirectory + @"printout.pdf");
            // new Process
            // {
            //     StartInfo = new ProcessStartInfo(ExamplesDirectory + @"\printout.pdf")
            //     {
            //         UseShellExecute = true
            //     }
            // }.Start();
            //model.export_printout_report_to_html(1, ExamplesDirectory + @"printout.html");
            // new Process
            // {
            //     StartInfo = new ProcessStartInfo(ExamplesDirectory + @"\printout.html")
            //     {
            //         UseShellExecute = true
            //     }
            // }.Start();
            #endregion



            #region Export Result XML / CSV

            // model.export_result_tables_with_detailed_members_results_to_csv(ExamplesDirectory);

            // model.export_result_tables_with_detailed_members_results_to_xml(ExamplesDirectory + @"\Test.xml");

            #endregion

            #region Exports to files
            // string export = model.export_to(ExamplesDirectory + @"\Mymodel.gltf");
            // Console.WriteLine(export);
            // export = model.export_to(ExamplesDirectory + @"\Mymodel.glb");
            // Console.WriteLine(export);
            // export = model.export_to(ExamplesDirectory + @"\Mymodel.vtm");
            // Console.WriteLine(export);
            // export = model.export_to(ExamplesDirectory + @"\Mymodel.xml");
            // Console.WriteLine(export);

            // SafConfiguration safConfiguration = application.get_saf_settings();

            // model.export_to(ExamplesDirectory + @"\Mymodel.saf");
            // model.export_to(ExamplesDirectory + @"\Mymodel.xlsx");
            // export_to_ifc_object_location[] ifcLocation = null; // whole model will be exported
            // export_to_ifc_settings ifcSettings = new export_to_ifc_settings()
            // {
            //     axis_rotation_sequence = export_to_ifc_axis_rotation_sequence_type.X_Y_Z,
            //     mirror_axis_x = false,
            //     mirror_axis_y = false,
            //     mirror_axis_z = true,
            //     origin_coordinate_x = 0.0,
            //     origin_coordinate_y = 0.0,
            //     origin_coordinate_z = 0.0,
            //     export_type = export_to_ifc_export_type.E_EXPORT_IFC4_REFERENCE_VIEW,
            //     rotation_angle_0 = 0.0,
            //     rotation_angle_1 = 0.0,
            //     rotation_angle_2 = 0.0,
            //     switch_axis_x = export_to_ifc_axis_type.X,
            //     switch_axis_y = export_to_ifc_axis_type.Y,
            //     switch_axis_z = export_to_ifc_axis_type.Z,
            //     remove_accents = false,
            // };
            // model.export_to_ifc(ExamplesDirectory + @"\Mymodel.ifc", ifcSettings, ifcLocation);

            //TableExportConfigManager tableConfigManager = model.get_table_export_config_manager();
            // tableConfigManager.configs[0].TableExportConfigBase.TableExportMainConfig.property_export_target = TableExportMainConfig_property_export_target_type.E_EXPORT_TARGET_XLSX_FILE;
            // tableConfigManager.configs[0].TableExportConfigBase.TableExportMainConfig.property_rewrite_existing_worksheet = false;
            // tableConfigManager.configs[0].TableExportConfigBase.TableExportMainConfig.property_export_table_header = true;
            // tableConfigManager.configs[0].TableExportConfigBase.TableExportMainConfig.property_export_table_to_active_workbook = false;
            // tableConfigManager.configs[0].TableExportConfigBase.TableExportMainConfig.property_export_table_to_active_worksheet = false;
            // tableConfigManager.configs[0].TableExportConfigBase.TableExportMainConfig.property_export_as_plain_text = false;
            // tableConfigManager.configs[0].TableExportConfigBase.TableExportMainConfig.property_export_filled_rows_only = true;
            // tableConfigManager.configs[0].TableExportConfigBase.TableExportMainConfig.property_export_filled_tables_only = true;
            // tableConfigManager.configs[0].TableExportConfigBase.TableExportMainConfig.property_export_selected_objects_only = false;

            // var inputTableDef = tableConfigManager.configs[0].TableExportConfigBase.TableExportInputTablesConfig.property_check_state_of_items_input_table;
            // foreach (var item in inputTableDef)
            // {
            //     Console.WriteLine("{0} is {1}", item.first.ToString(), item.second.ToString());
            //     item.second = false;
            // }
            // inputTableDef[0].second = true;// crash because  inputTableDef is null
            // var resultTableDef = tableConfigManager.configs[0].TableExportConfigBase.TableExportResultTablesConfig.property_export_import_check_state_of_items_result_table;
            // foreach (var item in resultTableDef)
            // {
            //     Console.WriteLine("{0} is {1}", item.first.ToString(), item.second.ToString());
            //     item.second = false;
            // }
            // resultTableDef[5].second = true;
            //model.set_table_export_config_manager(tableConfigManager);
            //model.export_to_tables(ExamplesDirectory + @"Mymodel-tables.xlsx");
            #endregion
            // model.save(@"D:\TEMP\Mymodel.rf6", saveSettingsAsVersion);
            // application.close_model(0, false);
        }


        static void WebServicesExamples()
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            LogManager.Configuration = config;
            var logger = LogManager.GetCurrentClassLogger();
            string CurrentDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            string ExamplesDirectory = CurrentDirectory + @"\ExampleFiles\";
            try
            {
                // check if RFEM6 is running
                // Process[] RFEM6Process = Process.GetProcessesByName("RFEM6");
                // if (RFEM6Process.Length == 0)
                // {
                //     ProcessStartInfo start = new ProcessStartInfo();
                //     // start.Arguments = arguments;
                //     start.FileName = @"D:\RFEM-TestingVersions\master\bin\RFEM6.exe";// hardcoded
                //     start.WindowStyle = ProcessWindowStyle.Hidden;
                //     start.CreateNoWindow = true;
                //     int exitCode;
                //     Process proc = Process.Start(start);
                // }

                #region RFEM Settings
                application_information RFEMInfo;
                try
                {
                    // connects to RFEM6 application
                    application = new RfemApplicationClient(Binding, Address);

                }
                catch (Exception exception)
                {
                    if (application != null)
                    {
                        if (application.State != CommunicationState.Faulted)
                        {
                            application.Close();
                            logger.Error(exception, "Something happen:" + exception.Message);
                        }
                        else
                        {
                            application.Abort();
                            logger.Error(exception, "Communication with RFEM faulted:" + exception.Message);
                        }

                        application = null;
                    }
                }
                finally
                {
                    RFEMInfo = application.get_information();
                    logger.Info("Name: {0}, Version:{1}, Type: {2}, language: {3} ", RFEMInfo.name, RFEMInfo.version, RFEMInfo.type, RFEMInfo.language_name);
                    Console.WriteLine("Name: {0}, Version:{1}, Type: {2}, language: {3} ", RFEMInfo.name, RFEMInfo.version, RFEMInfo.type, RFEMInfo.language_name);
                }

                #endregion


                #region new model
                // creates new model
                // string modelName = "MyTestModel";
                // string modelUrl = application.new_model(modelName);
                string modelUrl = application.open_model(ExamplesDirectory + @"EmptyWithReport.rf6");
                //string modelUrl = application.get_active_model();
                // connects to RFEM6 model
                RfemModelClient model = new RfemModelClient(Binding, new EndpointAddress(modelUrl));


                #endregion

                TestingExample(model, logger);


                // TestingGlobalParameters(model, logger);


                // string[] models = application.get_model_list();
                //var query = from item in models where item == modelName select item;
                // int indexOfModel = Array.IndexOf(models, modelName);
                // application.close_model(indexOfModel, false);//close model
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                logger.Error(ex, "Stopped program because of exception :" + ex.Message);
            }
        }


        static void Main(string[] args)
        {
            WebServicesExamples();
        }
    }
}
