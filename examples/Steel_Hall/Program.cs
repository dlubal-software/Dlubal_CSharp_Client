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

namespace Steel_Hall
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
            Console.WriteLine("Steel Hall Generator for RFEM6 and RSTAB9");
            Console.Write("Height of frame [m]: ");
            double frameHeight = Convert.ToDouble(Console.ReadLine()!.Replace(".", ","), CultureInfo.CurrentCulture);

            Console.Write("Frame span [m]: ");
            double frameSpan = Convert.ToDouble(Console.ReadLine()!.Replace(".", ","), CultureInfo.CurrentCulture);

            int frameNumber = 0;
            Console.Write("Number of frames: ");
            bool frameNumberInput = int.TryParse(Console.ReadLine(), out frameNumber);
            while (!frameNumberInput)
            {
                Console.WriteLine("Please choose an integer number for the number of frames!");
                Console.Write("Number of frames:");
                frameNumberInput = int.TryParse(Console.ReadLine(), out frameNumber);
            }
            if (frameNumber < 1)
            {
                Console.WriteLine("Please enter a frame number greater than 1!");
            }

            Console.Write("Distance between frames [m]: ");
            double frameDistance = Convert.ToDouble(Console.ReadLine()!.Replace(".", ","), CultureInfo.CurrentCulture);

            Logger logger = LogManager.GetCurrentClassLogger();
            Directory.GetCurrentDirectory();

            #region Application Settings
            try
            {
                try
                {
                    application = new RfemApplicationClient(Binding, Address);
                }
                catch (Exception exception7)
                {
                    if (application != null)
                    {
                        if (application.State != CommunicationState.Faulted)
                        {
                            application.Close();
                            logger.Error(exception7, "Something happen:" + exception7.Message);
                        }
                        else
                        {
                            application.Abort();
                            logger.Error(exception7, "Communication with RFEM faulted:" + exception7.Message);
                        }
                    }
                }
                finally
                {
                    application_information ApplicationInfo = application.get_information();
                    logger.Info("Name: {0}, Version:{1}, Type: {2}, language: {3} ", ApplicationInfo.name, ApplicationInfo.version, ApplicationInfo.type, ApplicationInfo.language_name);
                    Console.WriteLine("Name: {0}, Version:{1}, Type: {2}, language: {3} ", ApplicationInfo.name, ApplicationInfo.version, ApplicationInfo.type, ApplicationInfo.language_name);
                }
                #endregion

                string modelName = "SteelFrame";
                string modelUrl = application.new_model(modelName);

                #region new model
                RfemModelClient model = new RfemModelClient(Binding, new EndpointAddress(modelUrl));
                model.reset();
                #endregion

                material steel = new material
                {
                    no = 1,
                    name = "S235"
                };

                List<section> sections = new List<section>();
                section section1 = new section
                {
                    no = 1,
                    material = steel.no,
                    materialSpecified = true,
                    name = "HEA 200",
                    comment = "frame section"
                };
                section section2 = new section
                {
                    no = 2,
                    material = steel.no,
                    materialSpecified = true,
                    name = "IPE 200",
                    comment = "side member section"
                };
                section section3 = new section
                {
                    no = 3,
                    material = steel.no,
                    materialSpecified = true,
                    name = "RD 16",
                    comment = "bracing section"
                };

                sections.Add(section1);
                sections.Add(section2);
                sections.Add(section3);


                SortedList<int, node> nodes = new SortedList<int, node>();
                int[] lineDefinitionNodes = new int[frameNumber * 4];
                int nodeId = 1;
                double xVector = 0.0;
                double yVector = 0.0;

                for (int k = 0; k < 2; k++)
                {
                    for (int l = 0; l < frameNumber; l++)
                    {
                        node bottomNode = new node
                        {
                            no = nodeId,
                            coordinates = new vector_3d
                            {
                                x = xVector,
                                y = yVector,
                                z = 0.0
                            },
                            coordinate_system_type = node_coordinate_system_type.COORDINATE_SYSTEM_CARTESIAN,
                            coordinate_system_typeSpecified = true
                        };
                        node topNode = new node
                        {
                            no = nodeId + 1,
                            coordinates = new vector_3d
                            {
                                x = xVector,
                                y = yVector,
                                z = 0.0 - frameHeight
                            },
                            coordinate_system_type = node_coordinate_system_type.COORDINATE_SYSTEM_CARTESIAN,
                            coordinate_system_typeSpecified = true
                        };
                        nodes.Add(nodeId, bottomNode);
                        nodes.Add(nodeId + 1, topNode);
                        lineDefinitionNodes[k] = nodeId;
                        lineDefinitionNodes[k + 1] = nodeId + 1;
                        yVector = (0.0 - frameDistance) * (double)l;
                        nodeId += 2;
                    }
                    xVector += frameSpan;
                }
#if RFEM
                int lineId = 1;
                int m = 0;
                int numberOfLines = frameNumber * 4 + (frameNumber - 2);
                SortedList<int, line> zLines = new SortedList<int, line>();

                for (int j = 0; j < frameNumber * 2; j++)
                {
                    line line = new line();
                    line.no = lineId;
                    line.definition_nodes = new int[2]
                    {
                    lineDefinitionNodes[m],
                    lineDefinitionNodes[m + 1]
                    };
                    line.comment = "lines for beams";
                    line.type = line_type.TYPE_POLYLINE;
                    line.typeSpecified = true;
                    line newLine = line;
                    zLines.Add(lineId, newLine);
                    m += 2;
                    lineId++;
                }
#endif
                int memberId = 1;
                SortedList<int, member> zMembers = new SortedList<int, member>();
#if RFEM
                foreach (KeyValuePair<int, line> lineItem in zLines)
                {
                    member newMember = new member
                    {
                        no = memberId,
                        line = lineItem.Key,
                        lineSpecified = true,
                        section_start = section2.no,
                        section_startSpecified = true,
                        section_end = section2.no,
                        section_endSpecified = true,
                        comment = "concrete beam"
                    };
                    zMembers.Add(memberId, newMember);
                    memberId++;
                }

#elif RSTAB
                int j = 0;

                for (int i = 0; i < frameNumber; i++)
			    {
                     member newMember = new()
                    {
                        no = memberId,
                        node_start = lineDefinitionNodes[j],
                        node_startSpecified = true,
                        node_end = lineDefinitionNodes[j + 1],
                        node_endSpecified = true,
                        section_start = section1.no,
                        section_startSpecified = true,
                        section_end = section1.no,
                        section_endSpecified = true,
                        comment = "concrete beam"
                    };
                    zMembers.Add(memberId, newMember);
                    memberId++;
                    j += 2;
			    }
#endif
                List<int> supportedNodes = new List<int>();

                foreach (KeyValuePair<int, node> nodeItem in nodes)
                {
                    if (nodeItem.Key % 2 == 0)
                    {
                        supportedNodes.Add(nodeItem.Key);
                    }
                }

                nodal_support support = new nodal_support
                {
                    no = 1,
                    nodes = supportedNodes.ToArray(),
                    spring = new vector_3d
                    {
                        x = double.PositiveInfinity,
                        y = double.PositiveInfinity,
                        z = double.PositiveInfinity
                    },
                    rotational_restraint = new vector_3d
                    {
                        x = double.PositiveInfinity,
                        y = 0.0,
                        z = double.PositiveInfinity
                    }
                };

                try
                {
                    model.begin_modification("Geometry");
                    model.set_material(steel);

                    foreach (section section in sections)
                    {
                        model.set_section(section);
                    }
                    foreach (KeyValuePair<int, node> item in nodes)
                    {
                        model.set_node(item.Value);
                    }
                    foreach (KeyValuePair<int, line> item2 in zLines)
                    {
                        model.set_line(item2.Value);
                    }
                    foreach (KeyValuePair<int, member> item3 in zMembers)
                    {
                        model.set_member(item3.Value);
                    }
                    model.set_nodal_support(support);
                }
                catch (Exception exception6)
                {
                    model.cancel_modification();
                    logger.Error(exception6, "Something happened while creation of geometry" + exception6.Message);
                    throw;
                }
                finally
                {
                    try
                    {
                        model.finish_modification();
                    }
                    catch (Exception exception5)
                    {
                        logger.Error(exception5, "Something went wrong while finishing modification of geometry\n" + exception5.Message + "\n");
                        model.reset();
                    }
                }

                static_analysis_settings analysis = new static_analysis_settings
                {
                    no = 1,
                    analysis_type = static_analysis_settings_analysis_type.GEOMETRICALLY_LINEAR,
                    analysis_typeSpecified = true
                };
                load_case selfWeightLC = new load_case
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
                    stability_analysis_settingsSpecified = true
                };
                load_case lcData = new load_case
                {
                    no = 2,
                    name = "My load case",
                    self_weight_active = false,
                    self_weight_activeSpecified = true,
                    static_analysis_settings = analysis.no,
                    static_analysis_settingsSpecified = true,
                    analysis_type = load_case_analysis_type.ANALYSIS_TYPE_STATIC,
                    analysis_typeSpecified = true,
                    action_category = "ACTION_CATEGORY_PERMANENT_IMPOSED_GQ"
                };
                design_situation design_Situation = new design_situation
                {
                    no = 1,
                    name = "ScriptedDS",
                    user_defined_name_enabled = true,
                    user_defined_name_enabledSpecified = true,
                    design_situation_type = "DESIGN_SITUATION_TYPE_EQU_PERMANENT_AND_TRANSIENT",
                    is_generated = false,
                    is_generatedSpecified = true,
                    consider_inclusive_exclusive_load_cases = true,
                    consider_inclusive_exclusive_load_casesSpecified = true
                };
                load_combination_items_row load_Combination_SW = new load_combination_items_row
                {
                    no = 1,
                    row = new load_combination_items
                    {
                        load_case = 1,
                        load_caseSpecified = true,
                        factor = 1.35,
                        factorSpecified = true
                    }
                };
                load_combination_items_row load_Combination_lcData = new load_combination_items_row
                {
                    no = 2,
                    row = new load_combination_items
                    {
                        load_case = 2,
                        load_caseSpecified = true,
                        factor = 1.5,
                        factorSpecified = true
                    }
                };
                load_combination_items_row[] loadCombinationItems = new load_combination_items_row[2] { load_Combination_SW, load_Combination_lcData };
                load_combination load_Combination = new load_combination
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
                    design_situationSpecified = true
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
                catch (Exception exception4)
                {
                    model.cancel_modification();
                    logger.Error(exception4, "Something happened while creation of analysis settings" + exception4.Message);
                    throw;
                }
                finally
                {
                    try
                    {
                        model.finish_modification();
                    }
                    catch (Exception exception3)
                    {
                        logger.Error(exception3, "Something went wrong while finishing creation of analysis settings\n" + exception3.Message + "\n");
                        model.reset();
                    }
                }

                SortedList<int, member_load> member_loads = new SortedList<int, member_load>();
                int member_load_id = 1;
                int n = 1;

                for (int i = 0; i < frameNumber * 2; i++)
                {
                    member_load member_load = new member_load();
                    member_load.no = member_load_id;
                    member_load.members_string = n.ToString();
                    member_load.members = new int[1] { i + 1 };
                    member_load.load_distribution = member_load_load_distribution.LOAD_DISTRIBUTION_UNIFORM;
                    member_load.load_distributionSpecified = true;
                    member_load.magnitude = 3000.0;
                    member_load.magnitudeSpecified = true;
                    member_load.load_is_over_total_length = true;
                    member_load.load_is_over_total_lengthSpecified = true;
                    member_load newMemberLoad = member_load;
                    member_loads.Add(i + 1, newMemberLoad);
                    member_load_id++;
                    n += 2;
                }
                try
                {
                    model.begin_modification("Set loads");
                    using IEnumerator<KeyValuePair<int, member_load>> enumerator5 = member_loads.GetEnumerator();
                    while (enumerator5.MoveNext())
                    {
                        model.set_member_load(value: enumerator5.Current.Value, load_case_no: lcData.no);
                    }
                }
                catch (Exception exception2)
                {
                    model.cancel_modification();
                    logger.Error(exception2, "Something happened while load transfer" + exception2.Message);
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
                
                mesh_statistics_type mesh_Statistics = model.get_mesh_statistics();
                Console.WriteLine("Number of mesh nodes: " + mesh_Statistics.node_elements);
                Console.WriteLine("Number of 1D elements: " + mesh_Statistics.member_1D_finite_elements);
                Console.WriteLine("Number of surface element: " + mesh_Statistics.surface_2D_finite_elements);
                Console.WriteLine("Number of volume elements: " + mesh_Statistics.solid_3D_finite_elements);
                model.save("C:\\Users\\GoebelR\\Documents\\Webservices\\testmodels\\steelHall");
                application.close_model(0, save_changes: true);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                logger.Error(ex, "Stopped program because of exception :" + ex.Message);
            }
        }
    }
}

