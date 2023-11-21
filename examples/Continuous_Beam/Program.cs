#if RFEM
using Dlubal.WS.Rfem6.Application;
using ApplicationClient = Dlubal.WS.Rfem6.Application.RfemApplicationClient;
using Dlubal.WS.Rfem6.Model;
using ModelClient = Dlubal.WS.Rfem6.Model.RfemModelClient;
#elif RSTAB
using Dlubal.WS.Rstab9.Application;
using ApplicationClient = Dlubal.WS.Rstab9.Application.RstabApplicationClient;
using Dlubal.WS.Rstab9.Model;
using ModelClient = Dlubal.WS.Rstab9.Model.RstabModelClient;
#endif
using NLog;
using System.ServiceModel;
using System.Globalization;

namespace ContinuousBeam
{
    class Program
    {
        public static EndpointAddress Address { get; set; } = new EndpointAddress("http://localhost:8081");

        private static BasicHttpBinding Binding
        {
            get
            {
                BasicHttpBinding binding = new BasicHttpBinding
                {
                    // define timespan to send timeout

                    SendTimeout = new TimeSpan(0, 0, 180),
                    UseDefaultWebProxy = true,
                };

                return binding;
            }
        }

        private static ApplicationClient application = null;

        static void Main(string[] args)
        {
            // welcome message
            Console.WriteLine("Welcome to Continuous Beam Application for Dlubal Software");

            int spanNumber = 0;

            // get user input
            Console.Write("Number of spans: ");
            bool rightInput = int.TryParse(Console.ReadLine(), out spanNumber);

            while (rightInput == false)
            {
                Console.WriteLine("Please choose an integer number for the number of spans!");
                Console.Write("Number of spans:");
                rightInput = int.TryParse(Console.ReadLine(), out spanNumber);
            }

            Console.Write("Span [m]: ");
            double span = Convert.ToDouble(Console.ReadLine().Replace('.', ','), CultureInfo.CurrentCulture);

            Console.Write("Continuous member load [kN/m]: ");
            double memberLoad = Convert.ToDouble(Console.ReadLine().Replace('.', ','), CultureInfo.CurrentCulture);

            var logger = LogManager.GetCurrentClassLogger();
            string CurrentDirectory = Directory.GetCurrentDirectory();
            #region Application Settings
            try
            {
                application_information ApplicationInfo;
                try
                {
                    // connect to RFEM6 or RSTAB9 application
                    application = new ApplicationClient(Binding, Address);
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
                    ApplicationInfo = application.get_information();
                    logger.Info("Name: {0}, Version:{1}, Type: {2}, language: {3} ", ApplicationInfo.name, ApplicationInfo.version, ApplicationInfo.type, ApplicationInfo.language_name);
                    Console.WriteLine("Name: {0}, Version:{1}, Type: {2}, language: {3} ", ApplicationInfo.name, ApplicationInfo.version, ApplicationInfo.type, ApplicationInfo.language_name);
                }
                #endregion

                string modelName = "MyTestModel";
                string modelUrl ="";

                //check if model with same name already opened -> no program crash
                bool noModelOpened = false;
                do
                {
                    try
                    {
                        modelUrl = application.new_model(modelName);
                        noModelOpened = false;
                    }
                    catch (FaultException ex)
                    {
                        Console.WriteLine(ex.Message);
                        noModelOpened = true;
                        Console.WriteLine("Press any key to try again");
                        Console.ReadKey();
                    }
                }
                while (noModelOpened);

                #region new model
                // connect to RFEM6/RSTAB9 model
                ModelClient model = new ModelClient(Binding, new EndpointAddress(modelUrl));
                model.reset();
                #endregion

                material materialConcrete = new material
                {
                    no = 1,
                    name = "C20/25 | EN 1992-1-1:2004/A1:2014"
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
                    // width/height as in RFEM
                    name = "R_M1 0.5/1.0"
                };

                //create nodes
                SortedList<int, node> nodes = new SortedList<int, node>();
                int[] lineDefinitionNodes = new int[spanNumber + 1];
                int nodeId = 1;
                double xVector = 0.0;

                for (int i = 0; i < spanNumber + 1; i++)
                {
                    node newNode = new()
                    {
                        no = nodeId,
                        coordinates = new vector_3d() { x = xVector, y = 0.0, z = 0.0 },
                        coordinate_system_type = node_coordinate_system_type.COORDINATE_SYSTEM_CARTESIAN,
                        coordinate_system_typeSpecified = true,
                        comment = "concrete part"
                    };
                    nodes.Add(nodeId, newNode);
                    lineDefinitionNodes[i] = nodeId;
                    xVector = xVector + span;
                    nodeId++;
                }

#if RFEM        // create lines
                int lineId = 1;
                SortedList<int, line> lines = new SortedList<int, line>();

                for (int i = 0; i < spanNumber; i++)
                {
                    line newLine = new()
                    {
                        no = lineId,
                        definition_nodes = new int[] { lineDefinitionNodes[i], lineDefinitionNodes[i + 1] },
                        comment = "lines for beams",
                        type = line_type.TYPE_POLYLINE,
                        typeSpecified = true,
                    };
                    lines.Add(lineId, newLine);
                    lineId++;
                }
#endif
                // create members
                int memberId = 1;
                SortedList<int, member> members = new SortedList<int, member>();
#if RFEM
                foreach (KeyValuePair<int, line> lineItem in lines)
                {
                    member newMember = new()
                    {
                        no = memberId,
                        line = lineItem.Key,
                        lineSpecified = true,
                        section_start = sectionRectangle.no,
                        section_startSpecified = true,
                        section_end = sectionRectangle.no,
                        section_endSpecified = true,
                        comment = "concrete beam"
                    };
                    members.Add(memberId, newMember);
                    memberId++;
                }
#elif RSTAB
                for (int i = 0; i < spanNumber; i++)
			    {
                     member newMember = new()
                    {
                        no = memberId,
                        node_start = lineDefinitionNodes[i],
                        node_startSpecified = true,
                        node_end = lineDefinitionNodes[i + 1],
                        node_endSpecified = true,
                        section_start = sectionRectangle.no,
                        section_startSpecified = true,
                        section_end = sectionRectangle.no,
                        section_endSpecified = true,
                        comment = "concrete beam"
                    };
                    members.Add(memberId, newMember);
                    memberId++;
			    }
#endif                                
                //contains nodes with nodal support number 1
                List<int> supportedNodes1 = new();

                //contains nodes with nodal support number 2
                List<int> supportedNodes2 = new();
                List<nodal_support> nodalSupports = new();

                foreach (KeyValuePair<int, node> nodeItem in nodes)
                {
                    if (nodeItem.Key == 1)
                    {
                        supportedNodes1.Add(nodeItem.Key);
                    }
                    else
                    {
                        supportedNodes2.Add(nodeItem.Key);  
                    }
                }

                // create supports
                nodal_support support1 = new()
                {
                    no = 1,
                    nodes = supportedNodes1.ToArray(),
                    spring = new vector_3d() { x = double.PositiveInfinity, y = double.PositiveInfinity, z = double.PositiveInfinity },
                    rotational_restraint = new vector_3d() { x = double.PositiveInfinity, y = 0.0, z = double.PositiveInfinity }
                };

                nodal_support support2 = new()
                {
                    no = 2,
                    nodes = supportedNodes2.ToArray(),
                    spring = new vector_3d() { x = 0.0, y = double.PositiveInfinity, z = double.PositiveInfinity },
                    rotational_restraint = new vector_3d() { x = 0.0, y = 0.0, z = double.PositiveInfinity }
                };

                nodalSupports.Add(support1);
                nodalSupports.Add(support2);

                // transfer objects to RFEM
                try
                {
                    model.begin_modification("Geometry");
                    model.set_material(materialConcrete);
                    model.set_section(sectionRectangle);

                    foreach (KeyValuePair<int, node> nodeItem in nodes)
                    {
                        model.set_node(nodeItem.Value);
                    }
#if RFEM
                    foreach (KeyValuePair<int, line> lineItem in lines)
                    {
                        model.set_line(lineItem.Value);
                    }
#endif
                    foreach (KeyValuePair<int, member> memberItem in members)
                    {
                        model.set_member(memberItem.Value);
                    }

                    foreach (var nodalSupport in nodalSupports)
                    {
                        model.set_nodal_support(nodalSupport);
                    }
                }
                catch (Exception exception)
                {
                    model.cancel_modification();
                    logger.Error(exception, "Something happened while creation of geometry" + exception.Message);
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
                        logger.Error(exception, "Something went wrong while finishing modification of geometry\n" + exception.Message + "\n");
                        model.reset();
                    }
                }

                // define static analysis settings
                static_analysis_settings analysis = new()
                {
                    no = 1,
                    analysis_type = static_analysis_settings_analysis_type.GEOMETRICALLY_LINEAR,
                    analysis_typeSpecified = true,
                };

                // define load cases
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
                    stability_analysis_settings = analysis.no,
                    stability_analysis_settingsSpecified = true,
                };

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

                // define design-situation
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

                // define settings for load combination
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

                // define load combinations
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

                // transfer analysis settings to RFEM
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
                    logger.Error(exception, "Something happened while creation of analysis settings" + exception.Message);
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
                        logger.Error(exception, "Something went wrong while finishing creation of analysis settings\n" + exception.Message + "\n");
                        model.reset();
                    }
                }

                // define member load
                SortedList<int, member_load> member_loads = new SortedList<int, member_load>();
                int member_load_id = 1;

                for (int i = 0; i < spanNumber; i++)
                {
                    member_load newMemberLoad = new()
                    {
                        no = i + 1,
                        members_string = (i + 1).ToString(),
                        members = new int[] { i + 1 },
                        load_distribution = member_load_load_distribution.LOAD_DISTRIBUTION_UNIFORM,
                        load_distributionSpecified = true,
                        magnitude = memberLoad * 1000,
                        magnitudeSpecified = true,
                        load_is_over_total_length = true,
                        load_is_over_total_lengthSpecified = true,
                    };
                    member_loads.Add(i + 1, newMemberLoad);
                    member_load_id++;
                }   
                   
                // transfer loads to RFEM              
                try
                {
                    model.begin_modification("Set loads");
                    foreach (KeyValuePair<int, member_load> memberload in member_loads)
                    {
                        model.set_member_load(lcData.no, memberload.Value);
                    }
                }
                catch (Exception exception)
                {
                    model.cancel_modification();
                    logger.Error(exception, "Something happened while load transfer" + exception.Message);
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
                        logger.Error(exception, "Something went wrong while finishing load transfer\n" + exception.Message + "\n");
                        model.reset();
                    }
                }

#if RFEM
                #region generate mesh and get mesh statistics
                calculation_message[] meshGenerationMessage = model.generate_mesh(true);
                if (meshGenerationMessage.Length != 0)
                {
                }
                mesh_statistics_type mesh_Statistics = model.get_mesh_statistics();
                Console.WriteLine("Number of mesh nodes: " + mesh_Statistics.node_elements);
                Console.WriteLine("Number of 1D elements: " + mesh_Statistics.member_1D_finite_elements);
                Console.WriteLine("Number of surface element: " + mesh_Statistics.surface_2D_finite_elements);
                Console.WriteLine("Number of volume elements: " + mesh_Statistics.solid_3D_finite_elements);
                #endregion
#endif
                calculation_result calculationResult = model.calculate_all(true);

                if (calculationResult.succeeded == false)
                {
                }
                else
                {
                    Console.WriteLine("Calculation finished successfully");
                }

                // printout result messages
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

                // activate display of results along the length of the member, by default false -> results just at the beginning and end of the member + extremes
                model.use_detailed_member_results(true);

                // printout internal forces
                List<members_internal_forces_row[]> internalForcesMember_List = new();

                foreach (KeyValuePair<int, member> memberItem in members)
                {
                    members_internal_forces_row[] internalForcesMember = model.get_results_for_members_internal_forces(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lcData.no, memberItem.Key);
                    internalForcesMember_List.Add(internalForcesMember);
                }

                Console.WriteLine("Internal forces for member:");

                foreach (var member in internalForcesMember_List)
                {
                    foreach (var row in member)
                    {
                        Console.WriteLine("Row no {0}\t Description {1}", row.no, row.description);
                        Console.WriteLine("Node {0}\t Location {1}\t Location flags {2}\t Internal force label {3}\t Specification {4}", row.row.node_number != null ? row.row.node_number.value : "NAN", row.row.location, row.row.location_flags, row.row.internal_force_label, row.row.specification);
                        Console.WriteLine("N {0}\t Vy {1}\t Vz {2}\t Mx {3}\t My {4}\t Mz {5}\t", row.row.internal_force_n.ToString(), row.row.internal_force_vy.ToString(), row.row.internal_force_vz.ToString(), row.row.internal_force_mt.ToString(), row.row.internal_force_my.ToString(), row.row.internal_force_mz.ToString());
                    }
                }

                // printout member deformations
                Console.WriteLine("Global deformations for member:");

                List<members_global_deformations_row[]> globalDeformationsMember_List = new();

                foreach (KeyValuePair<int, member> memberItem in members)
                {
                    members_global_deformations_row[] globalDeformationsMember = model.get_results_for_members_global_deformations(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lcData.no, memberItem.Key);
                    globalDeformationsMember_List.Add(globalDeformationsMember);
                }

                foreach (var member in globalDeformationsMember_List)
                {
                    foreach (var row in member)
                    {
                        Console.WriteLine("Row no {0}\t Description {1}", row.no, row.description);
                        Console.WriteLine("Node {0}\t Location {1}\t Location flags {2}\t Deformation label {3}\t Specification {4}", row.row.node_number != null ? row.row.node_number.value : "NAN", row.row.location, row.row.location_flags, row.row.deformation_label, row.row.section);
                        Console.WriteLine("ux {0}\t uy {1}\t uz {2}\t utot {3}\t rx {4}\t ry {5}\t rz {6}\t warping {6}\t", row.row.displacement_x.ToString(), row.row.displacement_y.ToString(), row.row.displacement_z.ToString(), row.row.displacement_absolute.ToString(), row.row.rotation_x.ToString(), row.row.rotation_y.ToString(), row.row.rotation_z.ToString(), row.row.warping.ToString());
                    }
                }

                // printout node deformations
                nodes_deformations_row[] nodeDeformations = model.get_results_for_nodes_deformations(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lcData.no, 0);//all nodes -> 0
                Console.WriteLine("Node deformations:");
                foreach (var item in nodeDeformations)
                {
                    Console.WriteLine("Row no {0}\t Description {1} node comment {2}", item.no, item.description, item.row.specification);
                    Console.WriteLine("ux {0}\t uy {1}\t uz {2}\t utot {3}\t rx {4}\t ry {5}\t rz {6}\t", item.row.displacement_x.ToString(), item.row.displacement_y.ToString(), item.row.displacement_z.ToString(), item.row.displacement_absolute.ToString(), item.row.rotation_x.ToString(), item.row.rotation_y.ToString(), item.row.rotation_z.ToString());
                }

                // printout support forces
                nodes_support_forces_row[] nodeReactions = model.get_results_for_nodes_support_forces(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lcData.no, 0);//all nodes -> 0
                Console.WriteLine("Node reactions:");
                foreach (var item in nodeReactions)
                {
                    if (item.no == 6 | item.no == 20)
                    {
                        continue;
                    }
                    Console.WriteLine("Row no {0}\t Description {1}", item.no, item.description);
                    Console.WriteLine("node corresponding loading \t px {0}\t py {1}\t pz {2}\t mx {3}\t my {4}\t mz {5}\t label \t",item.row.support_force_p_x.value.ToString(), item.row.support_force_p_y.value.ToString(), item.row.support_force_p_z.value.ToString(), item.row.support_moment_m_x.value.ToString(), item.row.support_moment_m_y.ToString(), item.row.support_moment_m_z.ToString());
                }
                #endregion

                // printout parts list
                #region Generate parts list
                model.generate_parts_lists();
                parts_list_all_by_material_row[] partListByAllMaterial = model.get_parts_list_all_by_material();
                foreach (var item in partListByAllMaterial)
                {
                    if (!item.description.Contains("Total:"))
                    {
                        Console.WriteLine("Material no: {0}\t Material name: {1}\t object type: {2}\t coating:{3}\t volume: {4}\t mass: {5}", item.description, item.row.material_name, item.row.object_type, item.row.total_coating, item.row.volume, item.row.mass);
                    }
                    else
                    {
                        Console.WriteLine("Material total\t \t \t coating:{0}\t volume: {1}\t mass: {2}", item.row.total_coating, item.row.volume, item.row.mass);
                    }
                }

                Console.WriteLine("Members: ");
                parts_list_members_by_material_row[] partListMemberByMaterial = model.get_parts_list_members_by_material();
                foreach (var item in partListMemberByMaterial)
                {
                    if (!item.description.Contains("Total"))
                    {
                        Console.WriteLine("Material no: {0}\t Material name: {1}\t section: {2}\t members no:{3}\t quantity: {4}\t length: {5}\t unit surface area: {6}\t volume: {7}\t unit mass: {8}\t member mass: {9}\t total length: {10}\t total surface area: {11}\t total volume:{12}\t total mass:{13}",
                        item.description, item.row.material_name, item.row.section_name, item.row.members_no, item.row.quantity, item.row.length, item.row.unit_surface_area, item.row.volume, item.row.unit_mass, item.row.member_mass, item.row.total_length, item.row.total_surface_area, item.row.total_volume, item.row.total_mass);
                    }
                    else
                    {
                        Console.WriteLine("Total \t \t \t \t quantity: {4}\t length: {5}\t unit surface area: {6}\t volume: {7}\t unit mass: {8}\t member mass: {9}\t total length: {10}\t total surface area: {11}\t total volume:{12}\t total mass:{13}",
                                            item.description, item.row.material_name, item.row.section_name, item.row.members_no, item.row.quantity, item.row.length, item.row.unit_surface_area, item.row.volume, item.row.unit_mass, item.row.member_mass, item.row.total_length, item.row.total_surface_area, item.row.total_volume, item.row.total_mass);
                    }
                }
                #endregion

                //save the model before closing
                model.save(CurrentDirectory + @"\testmodels\");
                application.close_model(0, false);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                logger.Error(ex, "Stopped program because of exception :" + ex.Message);
            }
        }
    }
}