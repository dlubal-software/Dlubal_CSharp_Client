using Dlubal.WS.Rfem6.Application;
using ApplicationClient = Dlubal.WS.Rfem6.Application.RfemApplicationClient;
using Dlubal.WS.Rfem6.Model;
using ModelClient = Dlubal.WS.Rfem6.Model.RfemModelClient;
using System.ServiceModel;
using System.Reflection;

namespace SteelHall.MAUI
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
                      
        private static ApplicationClient? application = null;

        public void GenerateHall(double frameHeight, double frameSpan, double frameDistance, int frameNumber, double roofAngle, VerticalBracing verticalBracing, HorizontalBracing horizontalBracing)
        {
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
                    application_information ApplicationInfo = application.get_information();
                    Console.WriteLine("Name: {0}, Version:{1}, Type: {2}, language: {3} ", ApplicationInfo.name, ApplicationInfo.version, ApplicationInfo.type, ApplicationInfo.language_name);
                }
                #endregion

                string modelName = "SteelHall";

                string modelUrl = application.new_model(modelName);
                roofAngle = roofAngle * (Math.PI / 180);

                #region new model
                this.Model = new ModelClient(Binding, new EndpointAddress(modelUrl));
                this.Model.reset();
                #endregion

                // activate steel add-on
                addon_list_type addOn = Model.get_addon_statuses();
                addOn.design_addons.steel_design_active = true;

                try
                {
                    Model.begin_modification("Add-Ons");
                    Model.set_addon_statuses(addOn);
                }
                catch (Exception exception)
                {
                    Model.cancel_modification();
                    throw;
                }
                finally
                {
                    try
                    {
                        Model.finish_modification();
                    }
                    catch (Exception exception)
                    {
                    }
                }

                material steel = new material
                {
                    no = 1,
                    name = "S235JR"
                };

                List<cross_section> cross_sections = new List<cross_section>();
                cross_section section1 = new cross_section
                {
                    no = 1,
                    material = steel.no,
                    materialSpecified = true,
                    name = "HEA 240",
                    comment = "frame cross_section"
                };
                cross_section section2 = new cross_section
                {
                    no = 2,
                    material = steel.no,
                    materialSpecified = true,
                    name = "IPE 200",
                    comment = "side member cross_section"
                };
                cross_section section3 = new cross_section
                {
                    no = 3,
                    material = steel.no,
                    materialSpecified = true,
                    name = "ROUND 0.016/H",
                    comment = "bracing cross_section"
                };
                cross_section section4 = new cross_section
                {
                    no = 4,
                    material = steel.no,
                    materialSpecified = true,
                    name = "HEA 400",
                    comment = "cross_section for tapered member"
                };

                cross_sections.Add(section1);
                cross_sections.Add(section2);
                cross_sections.Add(section3);
                cross_sections.Add(section4);

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
                int lineId = 1;
                int m = 0;
                int numberOfLines = (frameNumber * 4) + (frameNumber - 2);

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
                }

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
                }

                //lines in y - direction
                SortedList<int, line> yLines = new SortedList<int, line>();
                int nodePositionY = 1;
                int secondNode = nodePositionY + 2;

                for (int k = 0; k < (frameNumber * 3) - 3; k++)
                {
                    if (k == frameNumber - 1)
                    {
                        nodePositionY += 2;
                        secondNode += 2;
                    }
                    else if (k == (frameNumber * 2) - 2)
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
                }

                //lines for vertical bracing
                SortedList<int, line> bracingLinesVertical = new SortedList<int, line>();
                int nodePositionB = 0;

                for (int k = 0; k < verticalBracing.LoopCount; k++)
                {
                    if ((verticalBracing.BracingType == 1 || verticalBracing.BracingType == 2) && nodePositionB == (frameNumber * 2) - 2)
                    {
                        nodePositionB += 2;
                    }

                    line newLine = new()
                    {
                        no = lineId,
                        definition_nodes = new int[] { lineDefinitionNodes[nodePositionB + 1], lineDefinitionNodes[nodePositionB + 2] },
                        comment = "lines vertical bracing",
                        type = line_type.TYPE_POLYLINE,
                        typeSpecified = true,
                    };

                    line newLine2 = new()
                    {
                        no = lineId + 1,
                        definition_nodes = new int[] { lineDefinitionNodes[nodePositionB], lineDefinitionNodes[nodePositionB + 3] },
                        comment = "lines vertical bracing",
                        type = line_type.TYPE_POLYLINE,
                        typeSpecified = true,
                    };
                    bracingLinesVertical.Add(lineId, newLine);
                    bracingLinesVertical.Add(lineId + 1, newLine2);

                    if (verticalBracing.BracingType == 3 && nodePositionB == (frameNumber * 2) - 4)
                    {
                        nodePositionB += 4;
                    }
                    else
                    {
                        nodePositionB += verticalBracing.Increment;
                    }
                    lineId += 2;
                }

                //lines for horizontal bracing
                SortedList<int, line> bracingLinesHorizontal = new SortedList<int, line>();
                int nodePositionBHorizontal1 = 1;
                int nodePositionBHorizontal2 = nodeId - frameNumber;

                for (int i = 0; i < horizontalBracing.LoopCount; i++)
                {
                    if (nodePositionBHorizontal1 == (frameNumber * 2) - 1)
                    {
                        nodePositionBHorizontal1 += 2;
                        nodePositionBHorizontal2 = nodeId - frameNumber;
                    }
                    else if (horizontalBracing.BracingType == 5 && nodePositionBHorizontal1 == (frameNumber * 2) + 1)
                    {
                        nodePositionBHorizontal2 = nodeId - frameNumber;
                    }

                    line newLine = new()
                    {
                        no = lineId,
                        definition_nodes = new int[] { lineDefinitionNodes[nodePositionBHorizontal1], lineDefinitionNodes[nodePositionBHorizontal2] },
                        comment = "lines horizontal bracing",
                        type = line_type.TYPE_POLYLINE,
                        typeSpecified = true
                    };

                    line newLine2 = new()
                    {
                        no = lineId + 1,
                        definition_nodes = new int[] { lineDefinitionNodes[nodePositionBHorizontal1 + 2], lineDefinitionNodes[nodePositionBHorizontal2 - 1] },
                        comment = "lines horizontal bracing",
                        type = line_type.TYPE_POLYLINE,
                        typeSpecified = true
                    };
                    bracingLinesHorizontal.Add(lineId, newLine);
                    bracingLinesHorizontal.Add(lineId + 1, newLine2);

                    if (horizontalBracing.BracingType == 6 && nodePositionBHorizontal1 == (frameNumber * 2) - 3)
                    {
                        nodePositionBHorizontal1 += 4;
                        nodePositionBHorizontal2 = nodeId - frameNumber;
                    }
                    else
                    {
                        nodePositionBHorizontal1 += horizontalBracing.Increment;
                        nodePositionBHorizontal2 += horizontalBracing.IncrementMiddleNode;
                    }
                    lineId += 2;
                }
                                          
                //create members
                int memberId = 1;

                //members in z-direction
                SortedList<int, member> zMembers = new SortedList<int, member>();

                foreach (KeyValuePair<int, line> lineItem in zLines)
                {
                    member newMember = new()
                    {
                        no = memberId,
                        line = lineItem.Key,
                        lineSpecified = true,
                        cross_section_start = section1.no,
                        cross_section_startSpecified = true,
                        cross_section_end = section1.no,
                        cross_section_endSpecified = true,                        
                        comment = "column"
                    };
                    zMembers.Add(memberId, newMember);
                    memberId++;
                }

                //members in x-direction
                SortedList<int, member> xMembers = new SortedList<int, member>();

                foreach (KeyValuePair<int, line> lineItem in xLines)
                {
                    member newMember = new()
                    {
                        no = memberId,
                        line = lineItem.Key,
                        lineSpecified = true,
                        cross_section_distribution_type = member_cross_section_distribution_type.SECTION_DISTRIBUTION_TYPE_TAPERED_AT_START_OF_MEMBER,
                        cross_section_distribution_typeSpecified = true,
                        section_distance_from_start_relative = 0.25,
                        section_distance_from_start_relativeSpecified = true,
                        cross_section_distance_from_end_relative = 0.75,
                        cross_section_distance_from_end_relativeSpecified = true,
                        cross_section_distance_from_start_is_defined_as_relative = true,
                        cross_section_distance_from_end_is_defined_as_relativeSpecified = true,
                        cross_section_distance_from_end_is_defined_as_relative = true,
                        cross_section_distance_from_start_is_defined_as_relativeSpecified = true,
                        cross_section_alignment = member_cross_section_alignment.SECTION_ALIGNMENT_TOP,
                        cross_section_alignmentSpecified = true,
                        reference_type = member_reference_type.REFERENCE_TYPE_L,
                        reference_typeSpecified = true,
                        design_properties_via_member = false,
                        design_properties_via_memberSpecified = true,
                        cross_section_start = section4.no,
                        cross_section_startSpecified = true,
                        cross_section_end = section1.no,
                        cross_section_endSpecified = true,
                        comment = "bar"
                    };
                    xMembers.Add(memberId, newMember);
                    memberId++;
                }

                //members in y - direction
                SortedList<int, member> yMembers = new SortedList<int, member>();

                foreach (KeyValuePair<int, line> lineItem in yLines)
                {
                    member newMember = new()
                    {
                        no = memberId,
                        line = lineItem.Key,
                        lineSpecified = true,
                        cross_section_start = section2.no,
                        cross_section_startSpecified = true,
                        cross_section_end = section2.no,
                        cross_section_endSpecified = true,
                        comment = "side member"
                    };
                    yMembers.Add(memberId, newMember);
                    memberId++;
                }

                //members for vertical bracing
                SortedList<int, member> verticalBracingMembers = new SortedList<int, member>();

                foreach (KeyValuePair<int, line> lineItem in bracingLinesVertical)
                {
                    member newMember = new()
                    {
                        no = memberId,
                        line = lineItem.Key,
                        lineSpecified = true,
                        cross_section_start = section3.no,
                        cross_section_startSpecified = true,
                        cross_section_end = section3.no,
                        cross_section_endSpecified = true,
                        type = member_type.TYPE_TENSION,
                        typeSpecified = true,
                        comment = "bracing member"
                    };
                    verticalBracingMembers.Add(memberId, newMember);
                    memberId++;
                }

                //members for horizontal bracing
                SortedList<int, member> horizontalBracingMembers = new SortedList<int, member>();

                foreach (KeyValuePair<int, line> lineItem in bracingLinesHorizontal)
                {
                    member newMember = new()
                    {
                        no = memberId,
                        line = lineItem.Key,
                        lineSpecified = true,
                        cross_section_start = section3.no,
                        cross_section_startSpecified = true,
                        cross_section_end = section3.no,
                        cross_section_endSpecified = true,
                        type = member_type.TYPE_TENSION,
                        typeSpecified = true,
                        comment = "bracing member"
                    };
                    horizontalBracingMembers.Add(memberId, newMember);
                    memberId++;
                }

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

                //transfer basic objects to RFEM
                try
                {
                    this.Model.begin_modification("Geometry");
                    this.Model.set_material(steel);

                    foreach (cross_section cross_section in cross_sections)
                    {
                        this.Model.set_cross_section(cross_section);
                    }
                    foreach (KeyValuePair<int, node> nodeItem in nodes)
                    {
                        this.Model.set_node(nodeItem.Value);
                    }
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
                    foreach (KeyValuePair<int, line> lineItem in bracingLinesVertical)
                    {
                        this.Model.set_line(lineItem.Value);
                    }
                    foreach (KeyValuePair<int,line> lineItem in bracingLinesHorizontal)
                    {
                        this.Model.set_line(lineItem.Value);
                    }
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
                    foreach (KeyValuePair<int, member> memberItem in verticalBracingMembers)
                    {
                        this.Model.set_member(memberItem.Value);
                    }
                    foreach (KeyValuePair<int, member> memberItem in horizontalBracingMembers)
                    {
                        this.Model.set_member(memberItem.Value);
                    }
                    this.Model.set_nodal_support(support);
                }
                catch (Exception exception)
                {
                    this.Model.cancel_modification();
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
                        this.Model.reset();
                    }
                }

                //define effective length parameters for steel design
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
                        restraint_warping_typeSpecified = true
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
                        restraint_warping_typeSpecified = true
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
                    members = zMembers.Keys.ToArray(),
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
                    this.Model.begin_modification("Set steel effective lengths");
                    this.Model.set_steel_effective_lengths(steelEffectiveLengths);
                }
                catch (Exception exception)
                {
                    this.Model.cancel_modification();
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
                    }
                }

                //define configurations for steel design
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
                    this.Model.begin_modification("Set  steel_member_design configurations");
                    this.Model.set_steel_design_uls_configuration(steelDesignUlsConfiguration);
                    this.Model.set_steel_design_sls_configuration(steelDesignSlsConfiguration);
                    this.Model.set_steel_design_fr_configuration(steelDesignFrConfiguration);
                }
                catch (Exception exception)
                {
                    this.Model.cancel_modification();
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
                    }
                }

                //define load cases and combinations
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

                //transfer load cases and combinations to RFEM
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
                        load_is_over_total_lengthSpecified = true
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
                        load_is_over_total_lengthSpecified = true
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
                            load_is_over_total_lengthSpecified = true
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
                            load_is_over_total_lengthSpecified = true
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
                            load_is_over_total_lengthSpecified = true
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
                            load_is_over_total_lengthSpecified = true
                        };
                        member_loads_LC4.Add(member_load_id, newMemberLoad);
                        member_load_id++;
                    }
                }

                //transfer loads to RFEM
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
                        this.Model.reset();
                    }
                }

                calculation_result calculationMessages = this.Model.calculate_all(true);
                var assemblyLocation = Assembly.GetEntryAssembly().Location;
                string currentDirectory = Path.GetDirectoryName(assemblyLocation);
                this.Model.save(currentDirectory + @"\testmodels\");                
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);          
            }
        }

        //method to close the current RFEM-model after calculation is finished
        public void CloseModel()
        {
            string[] models = application.get_model_list();
            int modelNumber = models.Length - 1;
            int indexOfModel = Array.IndexOf(models, models.Length);
            application.close_model(modelNumber, false);
        }

        //method to export the results as csv-file
        public string ExportCsv()
        {
            var assemblyLocation = Assembly.GetEntryAssembly().Location;
            string currentDirectory = Path.GetDirectoryName(assemblyLocation);
            this.Model.export_result_tables_with_detailed_members_results_to_csv(currentDirectory + @"\CSV\");
            return (currentDirectory + @"\CSV\");
        }

        //method that checks if calculation was successful
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
            resultMessages = messageResults;

            return resultMessages;
        }
    }
}
   
