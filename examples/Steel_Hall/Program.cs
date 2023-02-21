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
using System.Xml.Linq;

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

        public static bool CheckDouble(string? input)
        {
            bool doubleCheck = true;
            try
            {
                if (string.IsNullOrEmpty(input))
                {
                    throw new ArgumentNullException(nameof(input));
                }
                Convert.ToDouble(input.Replace(".", ","), CultureInfo.CurrentCulture);

                if (input == "0")
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("Please input a value!");
                doubleCheck = false;
            }
            catch(FormatException)
            {
                Console.WriteLine("Please input a number!");
                doubleCheck = false;               
            }
            catch(ArgumentOutOfRangeException)
            {
                Console.WriteLine("Please input a value greater 0!");
                doubleCheck = false;
            }
            return doubleCheck;
        }

        public static bool CheckInteger(string? input)
        {
            bool integerCheck = true;
            try
            {
                if (string.IsNullOrEmpty(input))
                {
                    throw new ArgumentNullException(nameof(input));
                }
                int.TryParse(input, out int intInput);

                if (intInput < 2)
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("Please input a value!");
                integerCheck = false;
            }
            catch (FormatException)
            {
                Console.WriteLine("Please input an integer number!");
                integerCheck = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Please input a frame number greater 1!");
                integerCheck = false;
            }
            return integerCheck;
        }

        public static double GetDoubleInput(string message)
        {
            bool check = false;
            string inputValue = string.Empty;

            while (check == false)
            {
                Console.Write(message);
                inputValue = Console.ReadLine();
                check = CheckDouble(inputValue);
            }
            double doubleValue = Convert.ToDouble(inputValue.Replace(".", ","), CultureInfo.CurrentCulture);
            return doubleValue;
        }

        public static int GetIntegerInput(string message)
        {
            bool check = false;
            string inputValue = string.Empty;
            while (check == false)
            {
                Console.Write(message);
                inputValue = Console.ReadLine();
                check = CheckInteger(inputValue);
            }
            int integerValue = int.Parse(inputValue);
            return integerValue;
        }

        private static ApplicationClient? application = null;

        static void Main(string[] args)
        {
            Console.WriteLine("Steel Hall Generator for RFEM6 and RSTAB9");
            //Console.Write("Height of frame [m]: ");          
           /* string frameHeightString = string.Empty; */// = Console.ReadLine();
            ////bool check = CheckDouble(frameHeightString);
            //bool check = false;

            double frameHeight = GetDoubleInput("Height of frame [m]: ");
            double frameSpan = GetDoubleInput("Frame span [m]: ");
            double frameDistance = GetDoubleInput("Distance between frames [m]: ");
            int frameNumber = GetIntegerInput("Number of frames: ");



            //while (check == false)
            //{
            //    Console.Write("Height of frame [m]: ");
            //    frameHeightString = Console.ReadLine();
            //    check = CheckDouble(frameHeightString);
            //}
            //double frameHeight = Convert.ToDouble(frameHeightString.Replace(".", ","), CultureInfo.CurrentCulture);

            //Console.Write("Frame span [m]: ");
            //string? frameSpanString = Console.ReadLine();
            //while (CheckDouble(frameSpanString) == false)
            //{
            //    Console.Write("Frame span [m]: ");
            //    frameSpanString = Console.ReadLine();
            //}
            //double frameSpan = Convert.ToDouble(frameSpanString.Replace(".", ","), CultureInfo.CurrentCulture);



            //int frameNumber = 0;
            //Console.Write("Number of frames: ");
            //string? frameNumberString = Console.ReadLine();

            //while (CheckInteger(frameNumberString) == false)
            //{
            //    Console.Write("Number of frames:");
            //    frameNumberString = Console.ReadLine();
            //}

            //Console.Write("Distance between frames [m]: ");
            //string? frameDistanceString = Console.ReadLine();
            //while (CheckDouble(frameDistanceString) == false)
            //{
            //    Console.Write("Distance between frames [m]: ");
            //    frameDistanceString = Console.ReadLine();
            //}
            //double frameDistance = Convert.ToDouble(frameDistanceString.Replace(".", ","), CultureInfo.CurrentCulture);

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
                bracing2 = true;
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
                    application = new ApplicationClient(Binding, Address);
                }
                catch (Exception exception)
                {
                    if (application != null)
                    {
                        if (application.State != CommunicationState.Faulted)
                        {
                            application.Close();
                            logger.Error(exception, "Something happened:" + exception.Message);
                        }
                        else
                        {
                            application.Abort();
                            logger.Error(exception, "Communication with RFEM faulted:" + exception.Message);
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
                ModelClient model = new ModelClient(Binding, new EndpointAddress(modelUrl));
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
                    name = "ROUND 0.016/H",
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
#endif
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
                int nodePositionX = 0;
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
                    nodePositionX += 4;
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
                int nodePositionY = 0;

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
                    nodePositionY += 2;
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
                int nodePositionB = 0;
                for (int i = 0; i < bracingNumber; i++)
			    {
                    if ((bracing1 == true || bracing2 == true) && nodePositionB == frameNumber * 2 - 2) 
                    {
                        nodePositionB += 2; 
                    };

                     member newMember = new()
                    {
                        no = memberId,
                        node_start = lineDefinitionNodes[nodePositionB + 1],
                        node_startSpecified = true,
                        node_end = lineDefinitionNodes[nodePositionB + 2],
                        node_endSpecified = true,
                        section_start = section3.no,
                        section_startSpecified = true,
                        section_end = section3.no,
                        section_endSpecified = true,
                        type = member_type.TYPE_TENSION,
                        typeSpecified = true,
                        comment = "bracing"
                    };
                    member newMember2 = new()
                    {
                        no = memberId + 1,
                        node_start = lineDefinitionNodes[nodePositionB],
                        node_startSpecified = true,
                        node_end = lineDefinitionNodes[nodePositionB + 3],
                        node_endSpecified = true,
                        section_start = section3.no,
                        section_startSpecified = true,
                        section_end = section3.no,
                        section_endSpecified = true,
                        type = member_type.TYPE_TENSION,
                        typeSpecified = true,
                        comment = "bracing"
                    };
                    bracingMembers.Add(memberId, newMember);
                    memberId += 2;
                    if (bracing3 == true && nodePositionB == frameNumber * 2 - 4)
                    {
                        nodePositionB += 4;
                    }
                    else
                    {
                        nodePositionB += increment;
                    }
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
#if RFEM
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
#endif
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
                load_case liveLoad = new load_case
                {
                    no = 2,
                    name = "Live Load",
                    self_weight_active = false,
                    self_weight_activeSpecified = true,
                    static_analysis_settings = analysis.no,
                    static_analysis_settingsSpecified = true,
                    analysis_type = load_case_analysis_type.ANALYSIS_TYPE_STATIC,
                    analysis_typeSpecified = true,
                    action_category = "ACTION_CATEGORY_IMPOSED_LOADS_CATEGORY_A_DOMESTIC_RESIDENTIAL_AREAS_QI_A"
                };
                load_case windX = new load_case
                {
                    no = 3,
                    name = "Wind in X",
                    self_weight_active = false,
                    self_weight_activeSpecified = true,
                    static_analysis_settings = analysis.no,
                    static_analysis_settingsSpecified = true,
                    analysis_type = load_case_analysis_type.ANALYSIS_TYPE_STATIC,
                    analysis_typeSpecified = true,
                    action_category = "ACTION_CATEGORY_WIND_QW"
                };
                load_case windY = new load_case
                {
                    no = 4,
                    name = "Wind in Y",
                    self_weight_active = false,
                    self_weight_activeSpecified = true,
                    static_analysis_settings = analysis.no,
                    static_analysis_settingsSpecified = true,
                    analysis_type = load_case_analysis_type.ANALYSIS_TYPE_STATIC,
                    analysis_typeSpecified = true,
                    action_category = "ACTION_CATEGORY_WIND_QW"
                };

                load_case snow = new load_case
                {
                    no = 5,
                    name = "Snow",
                    self_weight_active = false,
                    self_weight_activeSpecified = true,
                    static_analysis_settings = analysis.no,
                    static_analysis_settingsSpecified = true,
                    analysis_type = load_case_analysis_type.ANALYSIS_TYPE_STATIC,
                    analysis_typeSpecified = true,
                    action_category = "ACTION_CATEGORY_SNOW_ICE_LOADS_H_LESS_OR_EQUAL_TO_1000_M_QS"
                };
                List<load_case> loadCases = new List<load_case>();
                loadCases.Add(selfWeightLC);
                loadCases.Add(liveLoad);
                loadCases.Add(windX);
                loadCases.Add(windY);
                loadCases.Add(snow);

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
                load_combination_items_row load_Combination_liveLoad = new load_combination_items_row
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
                load_combination_items_row load_Combination_windX = new load_combination_items_row
                {
                    no = 3,
                    row = new load_combination_items
                    {
                        load_case = 3,
                        load_caseSpecified = true,
                        factor = 1.5,
                        factorSpecified = true
                    }
                };
                load_combination_items_row load_Combination_windY = new load_combination_items_row
                {
                    no = 4,
                    row = new load_combination_items
                    {
                        load_case = 4,
                        load_caseSpecified = true,
                        factor = 1.5,
                        factorSpecified = true
                    }
                };
                load_combination_items_row load_Combination_snow = new load_combination_items_row
                {
                    no = 5,
                    row = new load_combination_items
                    {
                        load_case = 5,
                        load_caseSpecified = true,
                        factor = 1.5,
                        factorSpecified = true
                    }
                };
                load_combination_items_row[] loadCombinationItems1 = new load_combination_items_row[4] { load_Combination_SW, load_Combination_liveLoad, load_Combination_windX, load_Combination_snow };
                load_combination_items_row[] loadCombinationItems2 = new load_combination_items_row[4] { load_Combination_SW, load_Combination_liveLoad, load_Combination_windY, load_Combination_snow };

                load_combination load_Combination1 = new load_combination
                {
                    no = 1,
                    name = "Combination 1",
                    user_defined_name_enabled = true,
                    user_defined_name_enabledSpecified = true,
                    to_solve = true,
                    to_solveSpecified = true,
                    analysis_type = load_combination_analysis_type.ANALYSIS_TYPE_STATIC,
                    analysis_typeSpecified = true,
                    items = loadCombinationItems1,
                    design_situation = 1,
                    design_situationSpecified = true
                };
                load_combination load_Combination2 = new load_combination
                {
                    no = 2,
                    name = "Combination 2",
                    user_defined_name_enabled = true,
                    user_defined_name_enabledSpecified = true,
                    to_solve = true,
                    to_solveSpecified = true,
                    analysis_type = load_combination_analysis_type.ANALYSIS_TYPE_STATIC,
                    analysis_typeSpecified = true,
                    items = loadCombinationItems2,
                    design_situation = 1,
                    design_situationSpecified = true
                };
                try
                {
                    model.begin_modification("Load");
                    model.set_static_analysis_settings(analysis);
                    foreach (var loadcase in loadCases)
                    {
                        model.set_load_case(loadcase);
                    }
                    model.set_design_situation(design_Situation);
                    model.set_load_combination(load_Combination1);
                    model.set_load_combination(load_Combination2);
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

                SortedList<int, member_load> member_loads_LC2 = new SortedList<int, member_load>();
                SortedList<int, member_load> member_loads_LC5 = new SortedList<int, member_load>();
                int member_load_id = 1;

                //member loads load case 2/5
                foreach (var memberItem in xMembers)
                {
                    member_load newMemberLoad = new member_load()
                    {
                        no = member_load_id,
                        members_string = memberItem.Key.ToString(),
                        members = new int[1] { memberItem.Key },
                        load_distribution = member_load_load_distribution.LOAD_DISTRIBUTION_UNIFORM,
                        load_distributionSpecified = true,
                        magnitude = 3000.0,
                        magnitudeSpecified = true,
                        load_is_over_total_length = true,
                        load_is_over_total_lengthSpecified = true,
                    };
                    member_loads_LC2.Add(member_load_id, newMemberLoad);
                    member_load_id++;

                    member_load newMemberLoad2 = new member_load()
                    {
                        no = member_load_id,
                        members_string = memberItem.Key.ToString(),
                        members = new int[1] { memberItem.Key },
                        load_distribution = member_load_load_distribution.LOAD_DISTRIBUTION_TRAPEZOIDAL,
                        load_distributionSpecified = true,
                        magnitude_1 = 1000.0,
                        magnitude_1Specified = true,
                        magnitude_2 = 750.0,
                        magnitude_2Specified = true,
                        load_is_over_total_length = true,
                        load_is_over_total_lengthSpecified = true,
                    };
                    member_loads_LC5.Add(member_load_id, newMemberLoad2);
                    member_load_id++;
                }

                //member loads load case 3
                SortedList<int, member_load> member_loads_LC3 = new SortedList<int, member_load>();
                                
                foreach (var memberItem in zMembers)
                {
                    if (memberItem.Key < frameNumber + 1)
                    {
                        member_load newMemberLoad = new member_load()
                        {
                            no = member_load_id,
                            members_string = memberItem.Key.ToString(),
                            members = new int[1] { memberItem.Key },
                            load_distribution = member_load_load_distribution.LOAD_DISTRIBUTION_UNIFORM,
                            load_distributionSpecified = true,
                            load_direction = member_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_TRUE,
                            load_directionSpecified = true,
                            magnitude = 1500.0,
                            magnitudeSpecified = true,
                            load_is_over_total_length = true,
                            load_is_over_total_lengthSpecified = true,
                        };
                        member_loads_LC3.Add(member_load_id, newMemberLoad);
                        member_load_id++;
                    }
                    else
                    {
                        member_load newMemberLoad = new member_load()
                        {
                            no = member_load_id,
                            members_string = memberItem.Key.ToString(),
                            members = new int[1] { memberItem.Key },
                            load_distribution = member_load_load_distribution.LOAD_DISTRIBUTION_UNIFORM,
                            load_distributionSpecified = true,
                            load_direction = member_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_TRUE,
                            load_directionSpecified = true,
                            magnitude = 700.0,
                            magnitudeSpecified = true,
                            load_is_over_total_length = true,
                            load_is_over_total_lengthSpecified = true,
                        };
                        member_loads_LC3.Add(member_load_id, newMemberLoad);
                        member_load_id++;
                    }
                }

                //member loads load case 4
                SortedList<int, member_load> member_loads_LC4 = new SortedList<int, member_load>();

                foreach (var memberItem in zMembers)
                {
                    if (memberItem.Key == frameNumber || memberItem.Key == frameNumber * 2)
                    {
                        member_load newMemberLoad = new member_load()
                        {
                            no = member_load_id,
                            members_string = memberItem.Key.ToString(),
                            members = new int[1] { memberItem.Key },
                            load_distribution = member_load_load_distribution.LOAD_DISTRIBUTION_UNIFORM,
                            load_distributionSpecified = true,
                            load_direction = member_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_TRUE,
                            load_directionSpecified = true,
                            magnitude = 800.0,
                            magnitudeSpecified = true,
                            load_is_over_total_length = true,
                            load_is_over_total_lengthSpecified = true,
                        };
                        member_loads_LC4.Add(member_load_id, newMemberLoad);
                        member_load_id++;
                    }
                    else if (memberItem.Key == 1 || memberItem.Key == frameNumber + 1)
                    {
                        member_load newMemberLoad = new member_load()
                        {
                            no = member_load_id,
                            members_string = memberItem.Key.ToString(),
                            members = new int[1] { memberItem.Key },
                            load_distribution = member_load_load_distribution.LOAD_DISTRIBUTION_UNIFORM,
                            load_distributionSpecified = true,
                            load_direction = member_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_TRUE,
                            load_directionSpecified = true,
                            magnitude = 400.0,
                            magnitudeSpecified = true,
                            load_is_over_total_length = true,
                            load_is_over_total_lengthSpecified = true,
                        };
                        member_loads_LC4.Add(member_load_id, newMemberLoad);
                        member_load_id++;
                    }
                }

                try
                {
                    model.begin_modification("Set loads");
                    foreach (KeyValuePair<int, member_load> memberload in member_loads_LC2)
                    {
                        model.set_member_load(liveLoad.no, memberload.Value);
                    }
                    foreach (KeyValuePair<int, member_load> memberload in member_loads_LC3)
                    {
                        model.set_member_load(windX.no, memberload.Value);
                    }
                    foreach (KeyValuePair<int, member_load> memberload in member_loads_LC4)
                    {
                        model.set_member_load(windY.no, memberload.Value);
                    }
                    foreach (KeyValuePair<int, member_load> memberload in member_loads_LC5)
                    {
                        model.set_member_load(snow.no, memberload.Value);
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

                bool modelHasLC2Calculated = model.has_results(case_object_types.E_OBJECT_TYPE_LOAD_CASE, liveLoad.no);
                if (modelHasLC2Calculated)
                {
                    Console.WriteLine("Model has LC2 results");
                }
                else
                {
                    Console.WriteLine("Model has no LC2 results");
                }

                bool modelHasLC3Calculated = model.has_results(case_object_types.E_OBJECT_TYPE_LOAD_CASE, windX.no);
                if (modelHasLC3Calculated)
                {
                    Console.WriteLine("Model has LC3 results");
                }
                else
                {
                    Console.WriteLine("Model has no LC3 results");
                }

                bool modelHasLC4Calculated = model.has_results(case_object_types.E_OBJECT_TYPE_LOAD_CASE, windY.no);
                if (modelHasLC4Calculated)
                {
                    Console.WriteLine("Model has LC4 results");
                }
                else
                {
                    Console.WriteLine("Model has no LC4 results");
                }

                bool modelHasLC5Calculated = model.has_results(case_object_types.E_OBJECT_TYPE_LOAD_CASE, snow.no);
                if (modelHasLC5Calculated)
                {
                    Console.WriteLine("Model has LC5 results");
                }
                else
                {
                    Console.WriteLine("Model has no LC5 results");
                }

                // activate display of results along the length of the member, by default false -> results just at the beginning and end of the member + extremes
                model.use_detailed_member_results(true);

                // printout internal forces for every member and every load case
                List<members_internal_forces_row[]> internalForcesMember_List = new();
                foreach (var loadcase in loadCases)
                {
                    foreach (KeyValuePair<int, member> memberItem in xMembers)
                    {
                        members_internal_forces_row[] internalForcesMember = model.get_results_for_members_internal_forces(case_object_types.E_OBJECT_TYPE_LOAD_CASE, loadcase.no, memberItem.Key);
                        internalForcesMember_List.Add(internalForcesMember);
                    }

                    foreach (KeyValuePair<int, member> memberItem in yMembers)
                    {
                        members_internal_forces_row[] internalForcesMember = model.get_results_for_members_internal_forces(case_object_types.E_OBJECT_TYPE_LOAD_CASE, loadcase.no, memberItem.Key);
                        internalForcesMember_List.Add(internalForcesMember);
                    }

                    foreach (KeyValuePair<int, member> memberItem in zMembers)
                    {
                        members_internal_forces_row[] internalForcesMember = model.get_results_for_members_internal_forces(case_object_types.E_OBJECT_TYPE_LOAD_CASE, loadcase.no, memberItem.Key);
                        internalForcesMember_List.Add(internalForcesMember);
                    }
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
                //Console.WriteLine("Global deformations for member:");

                //List<members_global_deformations_row[]> globalDeformationsMember_List = new();

                //foreach (KeyValuePair<int, member> memberItem in xMembers)
                //{
                //    members_global_deformations_row[] globalDeformationsMember = model.get_results_for_members_global_deformations(case_object_types.E_OBJECT_TYPE_LOAD_CASE, liveLoad.no, memberItem.Key);
                //    globalDeformationsMember_List.Add(globalDeformationsMember);
                //}

                //foreach (var member in globalDeformationsMember_List)
                //{
                //    foreach (var row in member)
                //    {
                //        Console.WriteLine("Row no {0}\t Description {1}", row.no, row.description);
                //        Console.WriteLine("Node {0}\t Location {1}\t Location flags {2}\t Deformation label {3}\t Specification {4}", row.row.node_number != null ? row.row.node_number.value : "NAN", row.row.location, row.row.location_flags, row.row.deformation_label, row.row.section);
                //        Console.WriteLine("ux {0}\t uy {1}\t uz {2}\t utot {3}\t rx {4}\t ry {5}\t rz {6}\t warping {6}\t", row.row.displacement_x.ToString(), row.row.displacement_y.ToString(), row.row.displacement_z.ToString(), row.row.displacement_absolute.ToString(), row.row.rotation_x.ToString(), row.row.rotation_y.ToString(), row.row.rotation_z.ToString(), row.row.warping.ToString());
                //    }
                //}

                //// printout node deformations
                //nodes_deformations_row[] nodeDeformations = model.get_results_for_nodes_deformations(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lcData.no, 0);//all nodes -> 0
                //Console.WriteLine("Node deformations:");
                //foreach (var item in nodeDeformations)
                //{
                //    Console.WriteLine("Row no {0}\t Description {1} node comment {2}", item.no, item.description, item.row.specification);
                //    Console.WriteLine("ux {0}\t uy {1}\t uz {2}\t utot {3}\t rx {4}\t ry {5}\t rz {6}\t", item.row.displacement_x.ToString(), item.row.displacement_y.ToString(), item.row.displacement_z.ToString(), item.row.displacement_absolute.ToString(), item.row.rotation_x.ToString(), item.row.rotation_y.ToString(), item.row.rotation_z.ToString());
                //}

                //// printout support forces
                //nodes_support_forces_row[] nodeReactions = model.get_results_for_nodes_support_forces(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lcData.no, 0);//all nodes -> 0
                //Console.WriteLine("Node reactions:");
                //foreach (var item in nodeReactions)
                //{
                //    Console.WriteLine("Row no {0}\t Description {1}", item.no, item.description);
                //    Console.WriteLine("note corresponding loading {0}\t px {1}\t py {2}\t pz {3}\t mx {4}\t my {5}\t mz {6}\t label {7}\t", item.row.node_comment_corresponding_loading.ToString(), item.row.support_force_p_x.value.ToString(), item.row.support_force_p_y.value.ToString(), item.row.support_force_p_z.value.ToString(), item.row.support_moment_m_x.value.ToString(), item.row.support_moment_m_y.ToString(), item.row.support_moment_m_z.ToString(), item.row.support_forces_label);
                //}
                //#endregion

                //// printout parts list
                //#region Generate parts list
                //model.generate_parts_lists();
                //parts_list_all_by_material_row[] partListByAllMaterial = model.get_parts_list_all_by_material();
                //foreach (var item in partListByAllMaterial)
                //{
                //    if (!item.description.Contains("Total:"))
                //    {
                //        Console.WriteLine("Material no: {0}\t Material name: {1}\t object type: {2}\t coating:{3}\t volume: {4}\t mass: {5}", item.description, item.row.material_name, item.row.object_type, item.row.total_coating, item.row.volume, item.row.mass);
                //    }
                //    else
                //    {
                //        Console.WriteLine("Material total\t \t \t coating:{0}\t volume: {1}\t mass: {2}", item.row.total_coating, item.row.volume, item.row.mass);
                //    }
                //}

                //Console.WriteLine("Members: ");
                //parts_list_members_by_material_row[] partListMemberByMaterial = model.get_parts_list_members_by_material();
                //foreach (var item in partListMemberByMaterial)
                //{
                //    if (!item.description.Contains("Total"))
                //    {
                //        Console.WriteLine("Material no: {0}\t Material name: {1}\t section: {2}\t members no:{3}\t quantity: {4}\t length: {5}\t unit surface area: {6}\t volume: {7}\t unit mass: {8}\t member mass: {9}\t total length: {10}\t total surface area: {11}\t total volume:{12}\t total mass:{13}",
                //        item.description, item.row.material_name, item.row.section_name, item.row.members_no, item.row.quantity, item.row.length, item.row.unit_surface_area, item.row.volume, item.row.unit_mass, item.row.member_mass, item.row.total_length, item.row.total_surface_area, item.row.total_volume, item.row.total_mass);
                //    }
                //    else
                //    {
                //        Console.WriteLine("Total \t \t \t \t quantity: {4}\t length: {5}\t unit surface area: {6}\t volume: {7}\t unit mass: {8}\t member mass: {9}\t total length: {10}\t total surface area: {11}\t total volume:{12}\t total mass:{13}",
                //                            item.description, item.row.material_name, item.row.section_name, item.row.members_no, item.row.quantity, item.row.length, item.row.unit_surface_area, item.row.volume, item.row.unit_mass, item.row.member_mass, item.row.total_length, item.row.total_surface_area, item.row.total_volume, item.row.total_mass);
                //    }
                //}
#endregion

                model.save("C:\\Users\\GoebelR\\Documents\\Webservices\\testmodels\\steelHall");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                logger.Error(ex, "Stopped program because of exception :" + ex.Message);
            }
        }
    }
}

