using System;
using System.IO;
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

using System.Net.Http;
using NLog;
using System.ServiceModel;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Cantilever
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
                    // Send timeout is set to 180 seconds.
                    SendTimeout = new TimeSpan(0, 0, 180),
                    UseDefaultWebProxy = true,
                };

                return binding;
            }
        }

        //private static RfemApplicationClient application = null;
        private static ApplicationClient application = null;

        static void Main(string[] args)
        {

            var config = new NLog.Config.LoggingConfiguration();
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            LogManager.Configuration = config;
            var logger = LogManager.GetCurrentClassLogger();
            string CurrentDirectory = Directory.GetCurrentDirectory();
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

                // creates new model
                string modelName = "MyTestModel";
                string modelUrl = application.new_model(modelName);

                #region new model
                // connects to RFEM6/RSTAB9 model
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
                    name = "R_M1 500/1000", // width/height as in RFEM
                };

                node n1 = new()
                {
                    no = 1,
                    coordinates = new vector_3d() { x = 0.0, y = 0.0, z = 0.0 },
                    coordinate_system_type = node_coordinate_system_type.COORDINATE_SYSTEM_CARTESIAN,
                    coordinate_system_typeSpecified = true,
                    comment = "concrete part"
                };

                node n2 = new()
                {
                    no = 2,
                    coordinates = new vector_3d() { x = 5.0, y = 0.0, z = 0.0 },
                    coordinate_system_type = node_coordinate_system_type.COORDINATE_SYSTEM_CARTESIAN,
                    coordinate_system_typeSpecified = true,
                    comment = "concrete part"
                };

#if RFEM
                line line = new()
                {
                    no = 1,
                    definition_nodes = new int[] { n1.no, n2.no },
                    comment = "lines for beams",
                    type = line_type.TYPE_POLYLINE,
                    typeSpecified = true,
                };
#endif
                // create member
                member member = new()
                {
                    no = 1,
#if RFEM
                    line = line.no,
                    lineSpecified = true,
#elif RSTAB
                    node_start = n1.no,
                    node_startSpecified = true,
                    node_end = n2.no,
                    node_endSpecified = true,
#endif
                    section_start = sectionRectangle.no,
                    section_startSpecified = true,
                    section_end = sectionRectangle.no,
                    section_endSpecified = true,
                    comment = "concrete beam"
                };

                nodal_support support = new()
                {
                    no = 1,
                    nodes = new int[] { n1.no },
                    spring = new vector_3d() { x = double.PositiveInfinity, y = double.PositiveInfinity, z = double.PositiveInfinity },
                    rotational_restraint = new vector_3d() { x = double.PositiveInfinity, y = double.PositiveInfinity, z = double.PositiveInfinity }
                };

                try
                {
                    model.begin_modification("Geometry");
                    model.set_material(materialConcrete);
                    model.set_section(sectionRectangle);
                    model.set_node(n1);
                    model.set_node(n2);
#if RFEM
                    model.set_line(line);
#endif
                    model.set_member(member);
                    model.set_nodal_support(support);
                }
                catch (Exception exception)
                {
                    model.cancel_modification();
                    logger.Error(exception, "Something happen when creation of geometry" + exception.Message);
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
                        model.reset();
                    }
                }


                static_analysis_settings analysis = new()
                {
                    no = 1,
                    analysis_type = static_analysis_settings_analysis_type.GEOMETRICALLY_LINEAR,
                    analysis_typeSpecified = true,
                };


                load_case selfWeightLC = new()
                {
                    no = 1,
                    name = "SelfWeight",
                    static_analysis_settings = analysis.no,
                    analysis_type = load_case_analysis_type.ANALYSIS_TYPE_STATIC,
                    analysis_typeSpecified = true,
                    stability_analysis_settingsSpecified = true,
                    self_weight_active = true,
                    self_weight_activeSpecified = true,
                    self_weight_factor_z = 1.0,
                    self_weight_factor_zSpecified = true,
                };
                try
                {
                    model.begin_modification("Load");
                    model.set_static_analysis_settings(analysis);
                    model.set_load_case(selfWeightLC);

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
                        model.reset();
                    }
                }


                model.calculate_all(true);
                #region Export Result XML / CSV
                model.export_result_tables_with_detailed_members_results_to_csv(CurrentDirectory);


                model.export_result_tables_with_detailed_members_results_to_xml(CurrentDirectory + @"\Test.xml");

                //string XMLFile = File.ReadAllText(CurrentDirectory + @"\Test.xml");
                //XDocument doc = XDocument.Parse(XMLFile);

                //foreach (XElement loadcase in doc.Descendants("load_case"))
                //{
                //    //Console.WriteLine(loadcase);
                //    if (loadcase.Element("no").Value == "1")
                //    {
                //        var internalForces = loadcase.Descendants("E_MODEL_MEMBERS_INTERNAL_FORCES");
                //        var internalForcesItems = internalForces.Descendants("item");
                //        foreach (var item in internalForcesItems)
                //        {
                //            string My = item.Element("internal_force_my").Value.ToString();
                //            Console.WriteLine(My);
                //        }

                //    }
                //}
                #endregion








                application.close_model(0, false);//close model
                application.close_application();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                logger.Error(ex, "Stopped program because of exception :" + ex.Message);
            }
        }
    }

}