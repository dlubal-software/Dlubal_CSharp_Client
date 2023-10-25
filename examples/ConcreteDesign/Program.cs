namespace Cantilever
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.ServiceModel;
    using Dlubal.WS.Rfem6.Application;
    using Dlubal.WS.Rfem6.Model;
    using ApplicationClient = Dlubal.WS.Rfem6.Application.RfemApplicationClient;
    using ModelClient = Dlubal.WS.Rfem6.Model.RfemModelClient;

    internal class Program
    {
        public static EndpointAddress Address { get; set; } = new EndpointAddress("http://localhost:8081");

        private static BasicHttpBinding Binding
        {
            get
            {
                BasicHttpBinding binding = new BasicHttpBinding
                {
                    SendTimeout = new TimeSpan(0, 0, 180),
                    UseDefaultWebProxy = true,
                    MaxReceivedMessageSize = 1000000,
                };

                return binding;
            }
        }

        private static ApplicationClient application = null;

        public static void Main(string[] args)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            #region Application Settings
            try
            {
                application_information ApplicationInfo;
                try
                {
                    // connects to RFEM6 or RSTAB9 application
                    application = new ApplicationClient(Binding, Address);
                }
                catch (Exception exception)
                {
                    if (application != null)
                    {
                        if (application.State != CommunicationState.Faulted)
                        {
                            application.Close();
                        }
                        else
                        {
                            application.Abort();
                        }

                        application = null;
                    }
                }
                finally
                {
                    ApplicationInfo = application.get_information();
                    Console.WriteLine("Name: {0}, Version:{1}, Type: {2}, language: {3} ", ApplicationInfo.name, ApplicationInfo.version, ApplicationInfo.type, ApplicationInfo.language_name);
                }
                #endregion

                #region new model

                // creates new model
                string modelName = "MyConcreteModel";
                string modelUrl = application.new_model(modelName);

                // connects to RFEM6/RSTAB9 model
                ModelClient model = new ModelClient(Binding, new EndpointAddress(modelUrl));
                model.reset();
                #endregion

                addon_list_type addon = model.get_addon_statuses();

                addon.design_addons.concrete_design_active = true;

                material materialConcrete = new material
                {
                    no = 1,
                    name = "C35/45 | EN 1992-1-1:2004/A1:2014",
                };

                material materialReinforcement = new material()
                {
                    no = 2,
                    name = "B500S(B)",
                };

                thickness surfaceThickness = new thickness()
                {
                    no = 1,
                    thickness_1 = 200,
                    thickness_1Specified = true,
                    type = thickness_type.TYPE_UNIFORM,
                    typeSpecified = true,
                    material = 1,
                    materialSpecified = true,
                };

                section sectionRectangle = new section
                {
                    no = 1,
                    material = materialConcrete.no,
                    materialSpecified = true,
                    type = section_type.TYPE_PARAMETRIC_MASSIVE_I,
                    typeSpecified = true,
                    parametrization_type = section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_RECTANGLE__R_M1,
                    parametrization_typeSpecified = true,
                    b = 0.3,
                    bSpecified = true,
                    h = 0.4,
                    hSpecified = true,
                };

                int nodeNumber = 1;
                double zCoordinate = 0.0;

                List<node> nodes = new List<node>();
                List<int> lineDefinitionNodes = new List<int>();
                List<line> lines = new List<line>();
                List<surface> surfaces = new List<surface>();
                List<member> members = new List<member>();

                for (int i = 0; i < 2; i++)
                {
                    node cornerNode1 = new ()
                    {
                        no = nodeNumber,
                        coordinates = new vector_3d() { x = 0.0, y = 0.0, z = zCoordinate },
                        coordinate_system_type = node_coordinate_system_type.COORDINATE_SYSTEM_CARTESIAN,
                        coordinate_system_typeSpecified = true,
                    };
                    nodes.Add(cornerNode1);
                    lineDefinitionNodes.Add(nodeNumber);
                    nodeNumber++;

                    node cornerNode2 = new ()
                    {
                        no = nodeNumber,
                        coordinates = new vector_3d() { x = 15.0, y = 0.0, z = zCoordinate },
                        coordinate_system_type = node_coordinate_system_type.COORDINATE_SYSTEM_CARTESIAN,
                        coordinate_system_typeSpecified = true,
                    };

                    nodes.Add(cornerNode2);
                    lineDefinitionNodes.Add(nodeNumber);
                    nodeNumber++;

                    node cornerNode3 = new ()
                    {
                        no = nodeNumber,
                        coordinates = new vector_3d() { x = 15.0, y = 15.0, z = zCoordinate },
                        coordinate_system_type = node_coordinate_system_type.COORDINATE_SYSTEM_CARTESIAN,
                        coordinate_system_typeSpecified = true,
                    };

                    nodes.Add(cornerNode3);
                    lineDefinitionNodes.Add(nodeNumber);
                    nodeNumber++;

                    node cornerNode4 = new ()
                    {
                        no = nodeNumber,
                        coordinates = new vector_3d() { x = 0.0, y = 15.0, z = zCoordinate },
                        coordinate_system_type = node_coordinate_system_type.COORDINATE_SYSTEM_CARTESIAN,
                        coordinate_system_typeSpecified = true,
                    };

                    nodes.Add(cornerNode4);
                    lineDefinitionNodes.Add(nodeNumber);
                    nodeNumber++;
                    zCoordinate = -5.5;
                }

                // nodes on line
                zCoordinate = 0.0;
                int referenceLineNumber = 1;

                for (int i = 0; i < 2; i++)
                {
                    node lineNode1 = new ()
                    {
                        no = nodeNumber,
                        coordinates = new vector_3d() { x = 7.5, y = 0.0, z = zCoordinate },
                        coordinate_system_type = node_coordinate_system_type.COORDINATE_SYSTEM_CARTESIAN,
                        coordinate_system_typeSpecified = true,
                        type = node_type.TYPE_ON_LINE,
                        typeSpecified = true,
                        on_line_reference_line = referenceLineNumber,
                        on_line_reference_lineSpecified = true,
                    };

                    nodes.Add(lineNode1);
                    nodeNumber++;
                    referenceLineNumber++;

                    node lineNode2 = new ()
                    {
                        no = nodeNumber,
                        coordinates = new vector_3d() { x = 15.0, y = 7.5, z = zCoordinate },
                        coordinate_system_type = node_coordinate_system_type.COORDINATE_SYSTEM_CARTESIAN,
                        coordinate_system_typeSpecified = true,
                        type = node_type.TYPE_ON_LINE,
                        typeSpecified = true,
                        on_line_reference_line = referenceLineNumber,
                        on_line_reference_lineSpecified = true,
                    };

                    nodes.Add(lineNode2);
                    nodeNumber++;
                    referenceLineNumber++;

                    node lineNode3 = new ()
                    {
                        no = nodeNumber,
                        coordinates = new vector_3d() { x = 7.5, y = 15.0, z = zCoordinate },
                        coordinate_system_type = node_coordinate_system_type.COORDINATE_SYSTEM_CARTESIAN,
                        coordinate_system_typeSpecified = true,
                        type = node_type.TYPE_ON_LINE,
                        typeSpecified = true,
                        on_line_reference_line = referenceLineNumber,
                        on_line_reference_lineSpecified = true,
                    };

                    nodes.Add(lineNode3);
                    nodeNumber++;
                    referenceLineNumber++;

                    node lineNode4 = new ()
                    {
                        no = nodeNumber,
                        coordinates = new vector_3d() { x = 0, y = 7.5, z = zCoordinate },
                        coordinate_system_type = node_coordinate_system_type.COORDINATE_SYSTEM_CARTESIAN,
                        coordinate_system_typeSpecified = true,
                        type = node_type.TYPE_ON_LINE,
                        typeSpecified = true,
                        on_line_reference_line = referenceLineNumber,
                        on_line_reference_lineSpecified = true,
                    };

                    zCoordinate = 5.5;
                    nodes.Add(lineNode4);
                    nodeNumber++;
                    referenceLineNumber++;
                }

                int lineNumber = 1;

                for (int i = 0; i < 8; i++)
                {
                    if (i == 3 || i == 7)
                    {
                        line newLine = new ()
                        {
                            no = lineNumber,
                            definition_nodes = new int[] { nodes[i].no, nodes[i - 3].no },
                            type = line_type.TYPE_POLYLINE,
                            typeSpecified = true,
                        };

                        lines.Add(newLine);
                    }
                    else
                    {
                        line newLine = new ()
                        {
                            no = lineNumber,
                            definition_nodes = new int[] { nodes[i].no, nodes[i + 1].no },
                            type = line_type.TYPE_POLYLINE,
                            typeSpecified = true,
                        };

                        lines.Add(newLine);
                    }

                    lineNumber++;
                }

                // lines for collumns
                int nodePosition = 0;

                for (int i = 0; i < 8; i++)
                {
                    if (i == 4)
                    {
                        nodePosition = 8;
                    }

                    line newLine = new ()
                    {
                        no = lineNumber,
                        definition_nodes = new int[] { nodes[nodePosition].no, nodes[nodePosition + 4].no },
                        type = line_type.TYPE_POLYLINE,
                        typeSpecified = true,
                    };

                    lineNumber++;
                    nodePosition++;
                    lines.Add(newLine);
                }

                surface surfaceBottom = new ()
                {
                    no = 1,
                    boundary_lines = new int[] { 1, 2, 3, 4 },
                    type = surface_type.TYPE_STANDARD,
                    typeSpecified = true,
                    surface_reinforcements = new int[] { 1 },
                    thickness = 1,
                    thicknessSpecified = true,
                };

                surface surfaceTop = new ()
                {
                    no = 2,
                    boundary_lines = new int[] { 5, 6, 7, 8 },
                    type = surface_type.TYPE_STANDARD,
                    typeSpecified = true,
                    thickness = 1,
                    thicknessSpecified = true,
                };

                surfaces.Add(surfaceBottom);
                surfaces.Add(surfaceTop);

                // create columns
                for (int i = 0; i < 8; i++)
                {
                    member newMember = new ()
                    {
                        no = i + 1,
                        line = lines[i + 8].no,
                        lineSpecified = true,
                        type = member_type.TYPE_BEAM,
                        typeSpecified = true,
                        section_start = sectionRectangle.no,
                        section_startSpecified = true,
                        section_end = sectionRectangle.no,
                        section_endSpecified = true,
                        section_alignment = member_section_alignment.SECTION_ALIGNMENT_CENTRIC,
                        section_alignmentSpecified = true,
                        section_distribution_type = member_section_distribution_type.SECTION_DISTRIBUTION_TYPE_UNIFORM,
                        section_distribution_typeSpecified = true,
                    };

                    members.Add(newMember);
                }

                surface_support support = new ()
                {
                    no = 1,
                    surfaces = new int[] { 1 },
                    translation = new vector_3d { x = 5000, y = 5000, z = 10000 },
                    shear_xz = double.PositiveInfinity,
                    shear_xzSpecified = true,
                    shear_yz = double.PositiveInfinity,
                    shear_yzSpecified = true,
                };

                try
                {
                    model.begin_modification("Geometry");
                    model.set_material(materialConcrete);
                    model.set_material(materialReinforcement);
                    model.set_thickness(surfaceThickness);
                    model.set_section(sectionRectangle);
                    model.set_addon_statuses(addon);
                    foreach (var node in nodes)
                    {
                        model.set_node(node);
                    }

                    foreach (var line in lines)
                    {
                        model.set_line(line);
                    }

                    foreach (var surface in surfaces)
                    {
                        model.set_surface(surface);
                    }

                    foreach (var member in members)
                    {
                        model.set_member(member);
                    }

                    model.set_surface_support(support);
                }
                catch (Exception exception)
                {
                    model.cancel_modification();
                    Console.WriteLine("Something happen when creation of geometry" + exception.Message);
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
                        Console.WriteLine("Something happen when finishing creation of geometry" + exception.Message);
                        model.reset();
                        throw;
                    }
                }

                #region concrete design
                //member_concrete_longitudinal_reinforcement_items_row longitudinalReeinforcementMember = new member_concrete_longitudinal_reinforcement_items_row()
                //{
                //    no = 1,
                //    row = new member_concrete_longitudinal_reinforcement_items()
                //    {
                //        rebar_type = rebar_type.REBAR_TYPE_SYMMETRICAL,
                //        rebar_typeSpecified = true,
                //        material = materialReinforcement.no,
                //        materialSpecified = true,
                //        bar_count_symmetrical = 4,
                //        bar_count_symmetricalSpecified = true,
                //        bar_diameter_symmetrical = 0.01,
                //        bar_diameter_symmetricalSpecified = true,
                //        span_position_reference_type = member_concrete_longitudinal_reinforcement_items_span_position_reference_type.LONGITUDINAL_REINFORCEMENT_ITEM_REFERENCE_START,
                //        span_position_reference_typeSpecified = true,
                //        span_position_definition_format_type = member_concrete_longitudinal_reinforcement_items_span_position_definition_format_type.LONGITUDINAL_REINFORCEMENT_SPAN_DEFINITION_FORMAT_RELATIVE,
                //        span_position_definition_format_typeSpecified = true,
                //        span_start_relative = 0.0,
                //        span_start_relativeSpecified = true,
                //        span_end_relative = 1.0,
                //        span_end_relativeSpecified = true,
                //        anchorage_start_anchor_type = anchorage_start_anchor_type.ANCHORAGE_TYPE_NONE,
                //        anchorage_end_anchor_type = anchorage_end_anchor_type.ANCHORAGE_TYPE_NONE,
                //    },
                //};

                //member_concrete_shear_reinforcement_spans_row shearReinforcement = new member_concrete_shear_reinforcement_spans_row()
                //{
                //    no = 1,
                //    row = new member_concrete_shear_reinforcement_spans()
                //    {
                //        material = materialReinforcement.no,
                //        stirrup_type = stirrup_type.STIRRUP_TYPE_FOUR_LEGGED_CLOSED_HOOK_135,
                //        stirrup_distances = 0.3,
                //        stirrup_diameter = 0.01,
                //        span_start_relative = 0.0,
                //        span_start_relativeSpecified = true,
                //        span_end_relative = 1.0,
                //        span_end_relativeSpecified = true,
                //        span_position_reference_type = span_position_reference_type.SHEAR_REINFORCEMENT_SPAN_REFERENCE_START,
                //        span_position_reference_typeSpecified = true,
                //        span_position_definition_format_type = span_position_definition_format_type.SHEAR_REINFORCEMENT_SPAN_DEFINITION_FORMAT_RELATIVE,
                //        span_position_definition_format_typeSpecified = true,
                //    },
                //};

                // define boundary conditions for concrete design
                concrete_durability concreteDurability = new concrete_durability()
                {
                    no = 1,
                    members = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 },
                    surfaces = new int[] { 1, 2 },
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

                reinforcement_direction reinforcementDirection = new reinforcement_direction()
                {
                    no = 1,
                    reinforcement_direction_type = reinforcement_direction_reinforcement_direction_type.REINFORCEMENT_DIRECTION_TYPE_FIRST_REINFORCEMENT_IN_X,
                    reinforcement_direction_typeSpecified = true,
                    surfaces = new int[] { 1, 2 },
                };

                surface_reinforcement surfaceReinforcement = new surface_reinforcement()
                {
                    no = 1,
                    location_type = surface_reinforcement_location_type.LOCATION_TYPE_ON_SURFACE,
                    location_typeSpecified = true,
                    surfaces = new[] { 1, 2 },
                    material = materialReinforcement.no,
                    materialSpecified = true,
                    reinforcement_type = surface_reinforcement_reinforcement_type.REINFORCEMENT_TYPE_MESH,
                    reinforcement_typeSpecified = true,
                    mesh_name = "Q424A",
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
                    model.set_concrete_durability(concreteDurability);

                    model.set_reinforcement_direction(reinforcementDirection);
                    model.set_surface_reinforcement(surfaceReinforcement);
                }
                catch (Exception exception)
                {
                    model.cancel_modification();
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
                    }
                }
                #endregion

                // create load cases
                static_analysis_settings analysis = new ()
                {
                    no = 1,
                    analysis_type = static_analysis_settings_analysis_type.GEOMETRICALLY_LINEAR,
                    analysis_typeSpecified = true,
                };

                load_case selfWeightLC = new ()
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
                    stability_analysis_settings = analysis.no,
                    stability_analysis_settingsSpecified = true,
                };

                load_case lcData = new ()
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
                    },
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
                    },
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
                    model.begin_modification("Load");
                    model.set_static_analysis_settings(analysis);
                    model.set_load_case(selfWeightLC);
                    model.set_load_case(lcData);

                    model.set_design_situation(design_Situation);
                    model.set_load_combination(load_Combination);

                }
                catch (Exception exception)
                {
                    model.cancel_modification();
                    Console.WriteLine("Something happen when creation of load" + exception.Message);
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
                        Console.WriteLine("Something wrong in finish modification of load\n" + exception.Message + "\n");
                        model.reset();
                    }
                }

                #region generate mesh and get mesh statistics
                calculation_message[] meshGenerationMessage = model.generate_mesh(true);

                #endregion

                model.calculate_all(true);

                #region Results

                // create object locations for surfaces
                object_location[] object_locations = new object_location[2];

                foreach (var surface in surfaces)
                {
                    object_location object_location = new object_location()
                    {
                        type = object_types.E_OBJECT_TYPE_SURFACE,
                        no = surface.no,
                        parent_no = 0,
                    };

                    object_locations.Append(object_location);
                }

                // get results for concrete design
                results_for_concrete_design_design_ratios_surfaces_by_surface_row[] designRatioResults = model.get_results_for_concrete_design_design_ratios_surfaces_by_surface(object_locations);

                // print results to console
                Console.WriteLine($"Design Ratios by Surface: ");
                int counter = 1;

                foreach (var item in designRatioResults.Skip(1))
                {
                    if (item.row.surface == null)
                    {
                        continue;
                    }

                    Console.WriteLine($"Row no.: {item.no}, Description: {item.description}");
                    Console.WriteLine($"Surface: {item.row.surface.value}\t Mesh Point: {item.row.mesh_node_or_grid_point_no.value}\t design ratio: {item.row.design_ratio.value}\t design check type: {item.row.design_check_type.value}\t design check formula: {item.row.design_check_formula.value}\t design check description: {item.row.design_check_description.value}");
                }

                #endregion
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                Console.WriteLine("Stopped program because of exception :" + ex.Message);
            }
        }
    }
}