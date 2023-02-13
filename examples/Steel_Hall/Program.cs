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

            Console.Write("Do you want vertical bracings in every field (1), in every second field (2) or only in the end fields (3): ");
            string bracingAnswer = Console.ReadLine();
            bool bracingInput = false;

            if (bracingAnswer == "1" || bracingAnswer == "2" || bracingAnswer == "3")
            {
                bracingInput = true;
            }
            while (bracingInput == false)
            {
                Console.WriteLine("Please enter 1, 2 or 3!");
                Console.WriteLine("Do you want vertical bracings in every field (1), in every second field (2) or only in the end fields (3): ");
                bracingAnswer = Console.ReadLine();

                if (bracingAnswer == "1" || bracingAnswer == "2" || bracingAnswer == "3")
                {
                    bracingInput = true;
                }
            }

            bool bracing1 = false;
            bool bracing2 = false;
            bool bracing3 = false;
            int  bracingNumber = 0;
            int loopCount = 0;
            int increment = 0;

            if (bracingAnswer == "1")
            {
                bracing1 = true;
                bracingNumber = 2 * (frameNumber * 2 - 2);
                loopCount = bracingNumber / 2;
                increment = 2;

            }
            else if (bracingAnswer == "2") 
            { 
                bracing2= true;
                bracingNumber = frameNumber * 2 - 2;
                if (frameNumber % 2 == 0)
                {
                    loopCount = bracingNumber / 2 + 1;
                }
                else
                {
                    loopCount = bracingNumber / 2;
                }                
                increment = 4;
            }
            else if (bracingAnswer == "3")
            {
                bracing3 = true;
                bracingNumber = 8;
                loopCount = 4;
                increment = frameNumber * 2 - 4;
            }

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
                    name = "S235JR"
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
                    name = "IPE 100",
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
                    for (int i = 0; i < frameNumber; i++)
                    {
                        node bottomNode = new()
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
                        node topNode = new()
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
                        lineDefinitionNodes[nodeId - 1] = nodeId;
                        lineDefinitionNodes[nodeId] = nodeId + 1;
                        yVector = (0.0 - frameDistance) * (i + 1);
                        nodeId += 2;
                    }
                    xVector += frameSpan;
                    yVector = 0.0;
                }
#if RFEM
                int lineId = 1;
                int m = 0;
                int numberOfLines = frameNumber * 4 + (frameNumber - 2);

                //lines in z-direction
                SortedList<int, line> zLines = new SortedList<int, line>();

                for (int j = 0; j < frameNumber * 2; j++) 
                {
                    line newLine = new()
                    {
                        no = lineId,
                        definition_nodes = new int[]{lineDefinitionNodes[m],lineDefinitionNodes[m + 1]},
                        comment = "lines for beams",
                        type = line_type.TYPE_POLYLINE,
                        typeSpecified = true,
                    };
                    zLines.Add(lineId, newLine);
                    m += 2;
                    lineId++;
                };

                //lines in x-direction
                SortedList<int, line> xLines = new SortedList<int, line>();
                int nodePositionX = 1;

                for (int k = 0; k < frameNumber; k++)
                {
                    line newLine = new()
                    {
                        no = lineId,
                        definition_nodes = new int[] { lineDefinitionNodes[nodePositionX], lineDefinitionNodes[nodePositionX + (frameNumber * 2)] },
                        comment = "lines for beams",
                        type = line_type.TYPE_POLYLINE,
                        typeSpecified = true,
                    };
                    xLines.Add(lineId, newLine);
                    nodePositionX += 2;
                    lineId++;
                };

                //lines in y - direction
                SortedList<int, line> yLines = new SortedList<int, line>();
                int nodePositionY = 1;

                for (int k = 0; k < frameNumber * 2 - 2; k++)
                {
                    if (k == frameNumber - 1)
                    {
                        nodePositionY += 2;
                    };

                    line newLine = new()
                    {
                        no = lineId,
                        definition_nodes = new int[] { lineDefinitionNodes[nodePositionY], lineDefinitionNodes[nodePositionY + 2] },
                        comment = "lines for beams",
                        type = line_type.TYPE_POLYLINE,
                        typeSpecified = true,
                    };
                    yLines.Add(lineId, newLine);
                    nodePositionY += 2;
                    lineId++;
                };
#endif

                //lines for bracing
                SortedList<int, line> bracingLines = new SortedList<int, line>();
                int nodePositionB = 0;

                for (int k = 0; k < loopCount; k++)
                {
                    if ((bracing1 == true || bracing2 == true) && nodePositionB == frameNumber * 2 - 2) 
                    {
                        nodePositionB += 2; 
                    };

                    line newLine = new()
                    {
                        no = lineId,
                        definition_nodes = new int[] { lineDefinitionNodes[nodePositionB + 1], lineDefinitionNodes[nodePositionB + 2] },
                        comment = "lines bracing",
                        type = line_type.TYPE_POLYLINE,
                        typeSpecified = true,
                    };

                    line newLine2 = new()
                    {
                        no = lineId + 1,
                        definition_nodes = new int[] { lineDefinitionNodes[nodePositionB], lineDefinitionNodes[nodePositionB + 3] },
                        comment = "lines bracing",
                        type = line_type.TYPE_POLYLINE,
                        typeSpecified = true,
                    };
                    bracingLines.Add(lineId, newLine);
                    bracingLines.Add(lineId + 1, newLine2);

                    if (bracing3 == true && nodePositionB == frameNumber * 2 - 4)
                    {
                        nodePositionB += 4;
                    }
                    else
                    {
                        nodePositionB += increment;
                    }
                    lineId += 2;
                }                

                int memberId = 1;

                //members in z-direction
                SortedList<int, member> zMembers = new SortedList<int, member>();
#if RFEM
                foreach (KeyValuePair<int, line> lineItem in zLines)
                {
                    member newMember = new()
                    {
                        no = memberId,
                        line = lineItem.Key,
                        lineSpecified = true,
                        section_start = section1.no,
                        section_startSpecified = true,
                        section_end = section1.no,
                        section_endSpecified = true,
                        comment = "coloumn"
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
                //members in x-direction
                SortedList<int, member> xMembers = new SortedList<int, member>();
#if RFEM
                foreach (KeyValuePair<int, line> lineItem in xLines)
                {
                    member newMember = new()
                    {
                        no = memberId,
                        line = lineItem.Key,
                        lineSpecified = true,
                        section_start = section1.no,
                        section_startSpecified = true,
                        section_end = section1.no,
                        section_endSpecified = true,
                        comment = "bar"
                    };
                    xMembers.Add(memberId, newMember);
                    memberId++;
                }
#elif RSTAB
                for (int i = 0; i < frameNumber; i++)
			    {
                     member newMember = new()
                    {
                        no = memberId,
                        node_start = lineDefinitionNodes[nodePositionX],
                        node_startSpecified = true,
                        node_end = lineDefinitionNodes[nodePositionX + 2],
                        node_endSpecified = true,
                        section_start = section1.no,
                        section_startSpecified = true,
                        section_end = section1.no,
                        section_endSpecified = true,
                        comment = "concrete beam"
                    };
                    zMembers.Add(memberId, newMember);
                    memberId++;
                    nodePosition += 4;
			    }
#endif

                //members in y - direction
                SortedList<int, member> yMembers = new SortedList<int, member>();
#if RFEM
                foreach (KeyValuePair<int, line> lineItem in yLines)
                {
                    member newMember = new()
                    {
                        no = memberId,
                        line = lineItem.Key,
                        lineSpecified = true,
                        section_start = section2.no,
                        section_startSpecified = true,
                        section_end = section2.no,
                        section_endSpecified = true,
                        comment = "side member"
                    };
                    yMembers.Add(memberId, newMember);
                    memberId++;
                }
#elif RSTAB
                for (int i = 0; i < frameNumber; i++)
			    {
                     member newMember = new()
                    {
                        no = memberId,
                        node_start = lineDefinitionNodes[nodePositionY],
                        node_startSpecified = true,
                        node_end = lineDefinitionNodes[nodePositionY + 2],
                        node_endSpecified = true,
                        section_start = section1.no,
                        section_startSpecified = true,
                        section_end = section1.no,
                        section_endSpecified = true,
                        comment = "concrete beam"
                    };
                    zMembers.Add(memberId, newMember);
                    memberId++;
                    nodePosition += 2;
			    }
#endif
                //members for bracing
                SortedList<int, member> bracingMembers = new SortedList<int, member>();
#if RFEM
                foreach (KeyValuePair<int, line> lineItem in bracingLines)
                {
                    member newMember = new()
                    {
                        no = memberId,
                        line = lineItem.Key,
                        lineSpecified = true,
                        section_start = section3.no,
                        section_startSpecified = true,
                        section_end = section3.no,
                        section_endSpecified = true,
                        type = member_type.TYPE_TENSION,
                        typeSpecified= true,
                        comment = "bracing member"
                    };
                    bracingMembers.Add(memberId, newMember);
                    memberId++;
                }
#elif RSTAB
                for (int i = 0; i < frameNumber; i++)
			    {
                     member newMember = new()
                    {
                        no = memberId,
                        node_start = lineDefinitionNodes[nodePositionY],
                        node_startSpecified = true,
                        node_end = lineDefinitionNodes[nodePositionY + 2],
                        node_endSpecified = true,
                        section_start = section1.no,
                        section_startSpecified = true,
                        section_end = section1.no,
                        section_endSpecified = true,
                        comment = "concrete beam"
                    };
                    zMembers.Add(memberId, newMember);
                    memberId++;
                    nodePosition += 2;
			    }
#endif
                List<int> supportedNodes = new List<int>();

                foreach (KeyValuePair<int, node> nodeItem in nodes)
                {
                    if (nodeItem.Key % 2 != 0)
                    {
                        supportedNodes.Add(nodeItem.Key);
                    }
                }

                nodal_support support = new()
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
                        x = 0.0,
                        y = 0.0,
                        z = double.PositiveInfinity
                    }
                };

                try
                {
                    model.begin_modification("Geometry");
                    model.set_material(steel);
                    //model.set_section(section1);

                    foreach (section section in sections)
                    {
                        model.set_section(section);
                    }
                    foreach (KeyValuePair<int, node> nodeItem in nodes)
                    {
                        model.set_node(nodeItem.Value);
                    }
                    foreach (KeyValuePair<int, line> lineItem in zLines)
                    {
                        model.set_line(lineItem.Value);
                    }
                    foreach (KeyValuePair<int, line> lineItem in xLines)
                    {
                        model.set_line(lineItem.Value);
                    }
                    foreach (KeyValuePair<int, line> lineItem in yLines)
                    {
                        model.set_line(lineItem.Value);
                    }
                    foreach (KeyValuePair<int, line> lineItem in bracingLines)
                    {
                        model.set_line(lineItem.Value);
                    }
                    foreach (KeyValuePair<int, member> memberItem in zMembers)
                    {
                        model.set_member(memberItem.Value);
                    }
                    foreach (KeyValuePair<int, member> memberItem in xMembers)
                    {
                        model.set_member(memberItem.Value);
                    }
                    foreach (KeyValuePair<int, member> memberItem in yMembers)
                    {
                        model.set_member(memberItem.Value);
                    }
                    foreach (KeyValuePair<int, member> memberItem in bracingMembers)
                    {
                        model.set_member(memberItem.Value);
                    }
                    model.set_nodal_support(support);
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
                    member_load newMemberLoad = new member_load()
                    {
                        no = member_load_id,
                        members_string = n.ToString(),
                        members = new int[1] { i + 1 },
                        load_distribution = member_load_load_distribution.LOAD_DISTRIBUTION_UNIFORM,
                        load_distributionSpecified = true,
                        magnitude = 3000.0,
                        magnitudeSpecified = true,
                        load_is_over_total_length = true,
                        load_is_over_total_lengthSpecified = true,
                    };
                    member_loads.Add(i + 1, newMemberLoad);
                    member_load_id++;
                    n += 2;
                }
                try
                {
                    model.begin_modification("Set loads");
                    foreach (KeyValuePair<int, member_load> memberload in member_loads)
                    {
                        model.set_member_load(lcData.no, memberload.Value);
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
                calculation_message[] calculationMessages = model.calculate_all(true);
                if (calculationMessages.Length != 0)
                {
                }
                else
                {
                    Console.WriteLine("Calculation finished successfully");
                }

                model.save("C:\\Users\\GoebelR\\Documents\\Webservices\\testmodels\\steelHall");
                //application.close_model(0, true);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                logger.Error(ex, "Stopped program because of exception :" + ex.Message);
            }
        }
    }
}

