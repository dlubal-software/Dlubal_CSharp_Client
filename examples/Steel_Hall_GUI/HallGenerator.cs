using System;
using System.Linq;
using System.IO;
using System.Globalization;
using System.ServiceModel;
using NLog;

#if RFEM
using Dlubal.WS.Rfem6.Application;
using ApplicationClient = Dlubal.WS.Rfem6.Application.RfemApplicationClient;
using Dlubal.WS.Rfem6.Model;
using ModelClient = Dlubal.WS.Rfem6.Model.RfemModelClient;
#elif RSTAB

#endif

namespace Steel_Hall_GUI
{
    public class HallGenerator
    {
        public static EndpointAddress Address { get; set; } = new EndpointAddress("http://localhost:8081");
        public ModelClient Model;

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
                double doubleValue = Convert.ToDouble(input.Replace(".", ","), CultureInfo.CurrentCulture);

                if (input == "0" || doubleValue < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
            catch (ArgumentNullException)
            {
                //Console.WriteLine("Please input a value!");
                doubleCheck = false;
            }
            catch (FormatException)
            {
               // Console.WriteLine("Please input a number!");
                doubleCheck = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                //Console.WriteLine("Please input a value greater than 0!");
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
                //Console.WriteLine("Please input a value!");
                integerCheck = false;
            }
            catch (FormatException)
            {
                //Console.WriteLine("Please input an integer number!");
                integerCheck = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                //Console.WriteLine("Please input a frame number greater 1!");
                integerCheck = false;
            }
            return integerCheck;
        }

        public static bool CheckBracingInput(ref string input)
        {
            bool bracingCheck = true;
            try
            {
                if (string.IsNullOrEmpty(input))
                {
                    throw new ArgumentNullException();
                }
                else if (!(input.ToLower() == "y" || input.ToLower() == "n"))
                {
                    throw new ArgumentOutOfRangeException();
                }
                input.ToLower();
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("Please input Y or N!");
                bracingCheck = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Please input Y or N!");
                bracingCheck = false;
            }

            return bracingCheck;
        }

        public static Bracing GetBracingType(string input, int frameNumber)
        {
            Bracing bracing = new Bracing();

            switch (input)
            {
                case "1":
                    bracing.BracingType = 1;
                    bracing.BracingNumber = 2 * ((frameNumber * 2) - 2);
                    bracing.LoopCount = bracing.BracingNumber / 2;
                    bracing.Increment = 2;
                    break;
                case "2":
                    bracing.BracingType = 2;
                    bracing.BracingNumber = (frameNumber * 2) - 2;
                    if (frameNumber % 2 == 0)
                    {
                        bracing.LoopCount = (bracing.BracingNumber / 2) + 1;
                    }
                    else
                    {
                        bracing.LoopCount = bracing.BracingNumber / 2;
                    }
                    bracing.Increment = 4;
                    break;
                case "3":
                    bracing.BracingType = 3;
                    bracing.BracingNumber = 8;
                    bracing.LoopCount = 4;
                    bracing.Increment = (frameNumber * 2) - 4;
                    break;
                default:
                    // value is neither 1 nor 2 nor 3
                    throw new ArgumentOutOfRangeException();
            }
            return bracing;
        }
        public static Bracing GetBracingInput(string message, int frameNumber)
        {
            bool check = false;
            bool numberCheck = false;
            string? inputValue = string.Empty;
            string? inputNumber = string.Empty;
            Bracing bracing = new Bracing();

            while (check == false)
            {
                Console.Write(message);
                inputValue = Console.ReadLine();
                check = CheckBracingInput(ref inputValue);
            }

            if (inputValue == "y")
            {
                while (!numberCheck)
                {
                    Console.Write("Do you want vertical bracings in every field (1), in every second field (2) or only in the end fields (3): ");
                    inputNumber = Console.ReadLine();
                    numberCheck = (inputNumber == "1" || inputNumber == "2" || inputNumber == "3") ? true : false;
                    if (!numberCheck)
                    {
                        Console.WriteLine("Please input either 1, 2 or 3!");
                    }
                }
                bracing = GetBracingType(inputNumber, frameNumber);
            }
            return bracing;
        }

        public static double GetDoubleInput(string inputValue)
        {
            bool check = false;
            //string inputValue = string.Empty;

            while (check == false)
            {
                //Console.Write(message);
                //inputValue = ;
                check = CheckDouble(inputValue);
            }
            double doubleValue = Convert.ToDouble(inputValue.Replace(".", ","), CultureInfo.CurrentCulture);
            return doubleValue;
        }

        public static int GetIntegerInput(string inputValue)
        {
            bool check = false;
            //string? inputValue = string.Empty;
            while (check == false)
            {
                //Console.Write(message);
                //inputValue = Console.ReadLine();
                check = CheckInteger(inputValue);
            }
            int integerValue = int.Parse(inputValue);
            return integerValue;
        }

        public static string ModelOpenedCheck(string modelName)
        {
            bool noModelOpened = false;
            string modelUrl = string.Empty;

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
                    Console.WriteLine("Close the model and press any key to try again");
                    Console.ReadKey();
                    Console.ReadLine();
                }
            }
            while (noModelOpened);

            return modelUrl;
        }

        private static ApplicationClient? application = null;
        string currentDirectory = Directory.GetCurrentDirectory();

        public void GenerateHall(double frameHeight, double frameSpan, double frameDistance, int frameNumber, double roofAngle, Bracing bracing)
        {
            Console.WriteLine("Steel Hall Generator for RFEM6 and RSTAB9");

            //get user-input
            //double frameHeight = GetDoubleInput("Height of frame [m]: ");
            //double frameSpan = GetDoubleInput("Frame span [m]: ");
            //double frameDistance = GetDoubleInput("Distance between frames [m]: ");
            //int frameNumber = GetIntegerInput("Number of frames: ");
            //Bracing bracing = GetBracingInput("Do you want to include vertical bracing (Y/N): ", frameNumber);

            //connection to RFEM/RSTAB
            //Logger logger = LogManager.GetCurrentClassLogger();
            

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
                            //logger.Error(exception, "Something happened:" + exception.Message);
                        }
                        else
                        {
                            application.Abort();
                            //logger.Error(exception, "Communication with RFEM faulted:" + exception.Message);
                        }
                        application = null;
                    }
                }
                finally
                {
                    application_information ApplicationInfo = application.get_information();
                    //logger.Info("Name: {0}, Version:{1}, Type: {2}, language: {3} ", ApplicationInfo.name, ApplicationInfo.version, ApplicationInfo.type, ApplicationInfo.language_name);
                    Console.WriteLine("Name: {0}, Version:{1}, Type: {2}, language: {3} ", ApplicationInfo.name, ApplicationInfo.version, ApplicationInfo.type, ApplicationInfo.language_name);
                }
                #endregion

                string modelName = "SteelHall";
                //check if model with same name is already opened
                string modelUrl = ModelOpenedCheck(modelName);

                #region new model
                this.Model = new ModelClient(Binding, new EndpointAddress(modelUrl));
                this.Model.reset();
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
                    name = "HEA 240",
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
                section section4 = new section
                {
                    no = 4,
                    material = steel.no,
                    materialSpecified = true,
                    name = "HEA 400",
                    comment = "section for tapered member"
                };

                sections.Add(section1);
                sections.Add(section2);
                sections.Add(section3);
                sections.Add(section4);

                //Create nodes
                SortedList<int, node> nodes = new SortedList<int, node>();
                int[] lineDefinitionNodes = new int[frameNumber * 5];

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
                //middle nodes
                xVector = 0.5 * frameSpan;
                yVector = 0.0;
                double frameHeightRooftop = frameHeight + (Math.Tan(roofAngle) * frameSpan / 2);
                
                for (int j = 0; j < frameNumber; j++)
                {
                    node middleNode = new()
                    {
                        no = nodeId,
                        coordinates = new vector_3d
                        {
                            x = xVector,
                            y = yVector,
                            z = 0 - frameHeightRooftop
                        },
                        coordinate_system_type = node_coordinate_system_type.COORDINATE_SYSTEM_CARTESIAN,
                        coordinate_system_typeSpecified = true
                    };                    
                    yVector -= frameDistance;
                    nodes.Add(nodeId, middleNode);
                    lineDefinitionNodes[nodeId - 1] = nodeId;
                    nodeId++;
                }

                //create lines
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
                        definition_nodes = new int[] { lineDefinitionNodes[m], lineDefinitionNodes[m + 1] },
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
                int nodePositionMiddle = nodeId - frameNumber - 1;

                for (int k = 0; k < frameNumber * 2; k++)
                {
                    line newLine = new()
                    {
                        no = lineId,
                        definition_nodes = new int[] { lineDefinitionNodes[nodePositionX], lineDefinitionNodes[nodePositionMiddle] },
                        comment = "lines for beams",
                        type = line_type.TYPE_POLYLINE,
                        typeSpecified = true,
                    };
                    xLines.Add(lineId, newLine);
                    nodePositionX += 2;
                    nodePositionMiddle ++;
                    if (nodePositionMiddle == nodeId - 1)
                    {
                        nodePositionMiddle = nodeId - frameNumber - 1;
                    }
                    lineId++;
                };

                //lines in y - direction
                SortedList<int, line> yLines = new SortedList<int, line>();
                int nodePositionY = 1;
                int secondNode = nodePositionY + 2;

                for (int k = 0; k < frameNumber * 3 - 3; k++)
                {
                    if (k == frameNumber - 1)
                    {
                        nodePositionY += 2;
                        secondNode += 2;
                    }
                    else if (k == frameNumber * 2 -2)
                    {
                        nodePositionY += 1;
                        //secondNode = nodePositionY + 1;
                    }

                    line newLine = new()
                    {
                        no = lineId,
                        definition_nodes = new int[] { lineDefinitionNodes[nodePositionY], lineDefinitionNodes[secondNode] },
                        comment = "lines for beams",
                        type = line_type.TYPE_POLYLINE,
                        typeSpecified = true,
                    };
                    yLines.Add(lineId, newLine);
                    if (k > frameNumber * 2 - 3)
                    {
                        nodePositionY ++;
                        secondNode++;
                    }
                    else
                    {
                        nodePositionY += 2;
                        secondNode += 2;
                    }                    
                    lineId++;
                };

                //lines for bracing
                SortedList<int, line> bracingLines = new SortedList<int, line>();
                int nodePositionB = 0;

                for (int k = 0; k < bracing.LoopCount; k++)
                {
                    if ((bracing.BracingType == 1 || bracing.BracingType == 2) && nodePositionB == frameNumber * 2 - 2)
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

                    if (bracing.BracingType == 3 && nodePositionB == frameNumber * 2 - 4)
                    {
                        nodePositionB += 4;
                    }
                    else
                    {
                        nodePositionB += bracing.Increment;
                    }
                    lineId += 2;
                }
#endif
                //create members
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
                        comment = "column"
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
                        section_distribution_type = member_section_distribution_type.SECTION_DISTRIBUTION_TYPE_TAPERED_AT_START_OF_MEMBER,
                        section_distribution_typeSpecified = true,
                        section_distance_from_start_relative = 0.25,
                        section_distance_from_start_relativeSpecified = true,
                        section_distance_from_end_relative = 0.75,
                        section_distance_from_end_relativeSpecified = true,
                        section_distance_from_start_is_defined_as_relative = true,
                        section_distance_from_end_is_defined_as_relativeSpecified = true,
                        section_distance_from_end_is_defined_as_relative = true,
                        section_distance_from_start_is_defined_as_relativeSpecified = true,
                        section_alignment = member_section_alignment.SECTION_ALIGNMENT_TOP,
                        reference_type = member_reference_type.REFERENCE_TYPE_L,
                        reference_typeSpecified = true,
                        section_alignmentSpecified = true,
                        section_start = section4.no,
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
                        typeSpecified = true,
                        comment = "bracing member"
                    };
                    bracingMembers.Add(memberId, newMember);
                    memberId++;
                }
#elif RSTAB
                int nodePositionB = 0;
                for (int i = 0; i < bracing.BracingNumber; i++)
			    {
                    if ((bracing.BracingType == 1 || bracing.BracingType == 2) && nodePositionB == frameNumber * 2 - 2) 
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
                    if (bracing.BracingType == 3 && nodePositionB == frameNumber * 2 - 4)
                    {
                        nodePositionB += 4;
                    }
                    else
                    {
                        nodePositionB += bracing.Increment;
                    }
			    }
#endif
                //create supports
                List<int> supportedNodes = new List<int>();

                foreach (KeyValuePair<int, node> nodeItem in nodes)
                {
                    if (nodeItem.Key < nodeId - frameNumber)
                    {
                        if (nodeItem.Key % 2 != 0)
                        {
                            supportedNodes.Add(nodeItem.Key);
                        }
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
                    this.Model.begin_modification("Geometry");
                    this.Model.set_material(steel);

                    foreach (section section in sections)
                    {
                        this.Model.set_section(section);
                    }
                    foreach (KeyValuePair<int, node> nodeItem in nodes)
                    {
                        this.Model.set_node(nodeItem.Value);
                    }
#if RFEM
                    foreach (KeyValuePair<int, line> lineItem in zLines)
                    {
                        this.Model.set_line(lineItem.Value);
                    }
                    foreach (KeyValuePair<int, line> lineItem in xLines)
                    {
                        this.Model.set_line(lineItem.Value);
                    }
                    foreach (KeyValuePair<int, line> lineItem in yLines)
                    {
                        this.Model.set_line(lineItem.Value);
                    }
                    foreach (KeyValuePair<int, line> lineItem in bracingLines)
                    {
                        this.Model.set_line(lineItem.Value);
                    }
#endif
                    foreach (KeyValuePair<int, member> memberItem in zMembers)
                    {
                        this.Model.set_member(memberItem.Value);
                    }
                    foreach (KeyValuePair<int, member> memberItem in xMembers)
                    {
                        this.Model.set_member(memberItem.Value);
                    }
                    foreach (KeyValuePair<int, member> memberItem in yMembers)
                    {
                        this.Model.set_member(memberItem.Value);
                    }
                    foreach (KeyValuePair<int, member> memberItem in bracingMembers)
                    {
                        this.Model.set_member(memberItem.Value);
                    }
                    this.Model.set_nodal_support(support);
                }
                catch (Exception exception)
                {
                    this.Model.cancel_modification();
                    //logger.Error(exception, "Something happened while creation of geometry" + exception.Message);
                    throw;
                }
                finally
                {
                    try
                    {
                        this.Model.finish_modification();
                    }
                    catch (Exception exception)
                    {
                        //logger.Error(exception, "Something went wrong while finishing modification of geometry\n" + exception.Message + "\n");
                        this.Model.reset();
                    }
                }

                static_analysis_settings analysis = new static_analysis_settings()
                {
                    no = 1,
                    analysis_type = static_analysis_settings_analysis_type.GEOMETRICALLY_LINEAR,
                    analysis_typeSpecified = true
                };
                load_case selfWeightLC = new load_case()
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
                load_case liveLoad = new load_case()
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
                load_case windX = new load_case()
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
                load_case windY = new load_case()
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

                load_case snow = new load_case()
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
                    consider_inclusive_exclusive_load_casesSpecified = true
                };
                load_combination_items_row load_Combination_SW = new load_combination_items_row()
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
                load_combination_items_row load_Combination_liveLoad = new load_combination_items_row()
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
                load_combination_items_row load_Combination_windX = new load_combination_items_row()
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
                load_combination_items_row load_Combination_windY = new load_combination_items_row()
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
                load_combination_items_row load_Combination_snow = new load_combination_items_row()
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

                load_combination load_Combination1 = new load_combination()
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
                load_combination load_Combination2 = new load_combination()
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
                    this.Model.begin_modification("Load");
                    this.Model.set_static_analysis_settings(analysis);
                    foreach (var loadcase in loadCases)
                    {
                        this.Model.set_load_case(loadcase);
                    }
                    this.Model.set_design_situation(design_Situation);
                    this.Model.set_load_combination(load_Combination1);
                    this.Model.set_load_combination(load_Combination2);
                }
                catch (Exception exception4)
                {
                    this.Model.cancel_modification();
                    //logger.Error(exception4, "Something happened while creation of analysis settings" + exception4.Message);
                    throw;
                }
                finally
                {
                    try
                    {
                        this.Model.finish_modification();
                    }
                    catch (Exception exception3)
                    {
                        //logger.Error(exception3, "Something went wrong while finishing creation of analysis settings\n" + exception3.Message + "\n");
                        this.Model.reset();
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
                    this.Model.begin_modification("Set loads");
                    foreach (KeyValuePair<int, member_load> memberload in member_loads_LC2)
                    {
                        this.Model.set_member_load(liveLoad.no, memberload.Value);
                    }
                    foreach (KeyValuePair<int, member_load> memberload in member_loads_LC3)
                    {
                        this.Model.set_member_load(windX.no, memberload.Value);
                    }
                    foreach (KeyValuePair<int, member_load> memberload in member_loads_LC4)
                    {
                        this.Model.set_member_load(windY.no, memberload.Value);
                    }
                    foreach (KeyValuePair<int, member_load> memberload in member_loads_LC5)
                    {
                        this.Model.set_member_load(snow.no, memberload.Value);
                    }
                }
                catch (Exception exception2)
                {
                    this.Model.cancel_modification();
                    //logger.Error(exception2, "Something happened while load transfer" + exception2.Message);
                    throw;
                }
                finally
                {
                    try
                    {
                        this.Model.finish_modification();
                    }
                    catch (Exception exception)
                    {
                        //logger.Error(exception, "Something went wrong while finishing load transfer\n" + exception.Message + "\n");
                        this.Model.reset();
                    }
                }
#if RFEM
                #region generate mesh and get mesh statistics
                //calculation_message[] meshGenerationMessage = this.Model.generate_mesh(true);
                //if (meshGenerationMessage.Length != 0)
                //{
                //    Console.WriteLine(calculationMessage);
                //}
                //mesh_statistics_type mesh_Statistics = this.Model.get_mesh_statistics();
                //Console.WriteLine("Number of mesh nodes: " + mesh_Statistics.node_elements);
                //Console.WriteLine("Number of 1D elements: " + mesh_Statistics.member_1D_finite_elements);
                //Console.WriteLine("Number of surface element: " + mesh_Statistics.surface_2D_finite_elements);
                //Console.WriteLine("Number of volume elements: " + mesh_Statistics.solid_3D_finite_elements);
                #endregion
#endif
                calculation_message[] calculationMessages = this.Model.calculate_all(true);
                //if (calculationMessages.Length != 0)
                //{
                //}
                //else
                //{
                //    Console.WriteLine("Calculation finished successfully");
                //}

                // printout result messages
                #region Results
                //bool modelHasAnyResults = this.Model.has_any_results();

                //if (modelHasAnyResults)
                //{
                //    Console.WriteLine("Model has results.");
                //}
                //else
                //{
                //    Console.WriteLine("Model has no results.");
                //}

                //bool modelHasLC2Calculated = this.Model.has_results(case_object_types.E_OBJECT_TYPE_LOAD_CASE, liveLoad.no);
                //if (modelHasLC2Calculated)
                //{
                //    Console.WriteLine("Model has LC2 results.");
                //}
                //else
                //{
                //    Console.WriteLine("Model has no LC2 results.");
                //}

                //bool modelHasLC3Calculated = this.Model.has_results(case_object_types.E_OBJECT_TYPE_LOAD_CASE, windX.no);
                //if (modelHasLC3Calculated)
                //{
                //    Console.WriteLine("Model has LC3 results.");
                //}
                //else
                //{
                //    Console.WriteLine("Model has no LC3 results.");
                //}

                //bool modelHasLC4Calculated = this.Model.has_results(case_object_types.E_OBJECT_TYPE_LOAD_CASE, windY.no);
                //if (modelHasLC4Calculated)
                //{
                //    Console.WriteLine("Model has LC4 results.");
                //}
                //else
                //{
                //    Console.WriteLine("Model has no LC4 results.");
                //}

                //bool modelHasLC5Calculated = this.Model.has_results(case_object_types.E_OBJECT_TYPE_LOAD_CASE, snow.no);
                //if (modelHasLC5Calculated)
                //{
                //    Console.WriteLine("Model has LC5 results.");
                //}
                //else
                //{
                //    Console.WriteLine("Model has no LC5 results.");
                //}
                #endregion

                #region export results
                //this.Model.export_result_tables_with_detailed_members_results_to_csv("C:\\Users\\GoebelR\\Documents\\Webservices\\testmodels\\CSV");
                //Console.WriteLine("Results have been exported as CSV-files to C:\\Users\\GoebelR\\Documents\\Webservices\\testmodels\\CSV.");
                #endregion

                this.Model.save(currentDirectory + @"\testmodels\");
                //Console.WriteLine("Model has been saved to C:\\Users\\GoebelR\\Documents\\Webservices\\testmodels\\SteelHall.");

                //Console.Write("Press enter to close the this.Model.");
                //if (Console.ReadKey().Key == ConsoleKey.Enter)
                //{
                //    application.close_model(0, true);
                //}
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                //logger.Error(ex, "Stopped program because of exception :" + ex.Message);                
            }
        }
        
        public void CloseModel()
        {
            application.close_model(0, false);
        }

        public string ExportCsv()
        {
            this.Model.export_result_tables_with_detailed_members_results_to_csv(currentDirectory + @"\CSV\");
            return (currentDirectory + @"\CSV\");
        }

        public string CreateResultMessage()
        {
            string resultMessages = string.Empty;
            string messageResults = string.Empty;

            bool modelHasAnyResults = this.Model.has_any_results();
            if (modelHasAnyResults)
            {
                messageResults = "Model has results!";
            }
            else
            {
                messageResults = "Model has no results!";
            }
            //bool modelHasLC1Calculated = this.Model.has_results(case_object_types.E_OBJECT_TYPE_LOAD_CASE, selfWeightLC.no);
            //bool modelHasLC2Calculated = this.Model.has_results(case_object_types.E_OBJECT_TYPE_LOAD_CASE, liveLoad.no);
            resultMessages = messageResults;

            return resultMessages;
        }
    }

}
