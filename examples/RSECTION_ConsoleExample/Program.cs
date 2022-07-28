using Dlubal.WS.RSection1.Application;
using Dlubal.WS.RSection1.Model;
using System;
using System.ServiceModel;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ComponentModel.Design;
using System.Xml.Serialization;
using NLog;
using NLog.Fluent;
using System.Data;
using System.Drawing;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Security.Policy;

using NLog.LayoutRenderers.Wrappers;
using System.Runtime.InteropServices;
using System.Data.Common;
using System.Diagnostics.Tracing;

namespace RSECTION_ConsoleExample
{
    class Program
    {
        public static EndpointAddress Address { get; set; } = new EndpointAddress("http://127.0.0.1:8101");

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
        // private static RfemApplicationClient RFEMapplication = null;
        private static RSectionApplicationClient application = null;


        static void OpenExistingModel()
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            LogManager.Configuration = config;
            var logger = LogManager.GetCurrentClassLogger();
            string CurrentDirectory = Directory.GetCurrentDirectory();
            string Examples = @"..\ExampleFiles";
            string ExamplesDirectory = Path.Combine(CurrentDirectory, Examples);
            #region RFEM Settings
            application_information RSECTIONInfo;
            try
            {
                application = new RSectionApplicationClient(Binding, Address);

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
                RSECTIONInfo = application.get_information();
                logger.Info("Name: {0}, Version:{1}, Type: {2}, language: {3} ", RSECTIONInfo.name, RSECTIONInfo.version, RSECTIONInfo.type, RSECTIONInfo.language_name);
                Console.WriteLine("Name: {0}, Version:{1}, Type: {2}, language: {3} ", RSECTIONInfo.name, RSECTIONInfo.version, RSECTIONInfo.type, RSECTIONInfo.language_name);
            }
            #endregion
            #region Open existing RSECTION model
            string openedModelUrl = application.open_model(ExamplesDirectory + @"\Test.rsc");
            Console.WriteLine(openedModelUrl);

            #endregion

            string[] listOfOpenedModels = application.get_model_list();
            foreach (var modelName in listOfOpenedModels)
            {
                Console.WriteLine(modelName);
            }
            RSectionModelClient modelRsection = null;
            try
            {
                modelRsection = new RSectionModelClient(Binding, new EndpointAddress(openedModelUrl));
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Communication with RFEM faulted:" + exception.Message);
            }
            #region Model parameters
            model_main_parameters model_Main_Parameters = modelRsection.get_model_main_parameters();

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

            //modelRsection.calculate_all(true); - not supported in RSECTION -> crash

            #region model info statuses
            modelInfo modelInfo = modelRsection.get_model_info();
            Console.WriteLine("Number of nodes in model: {0}", modelInfo.property_node_count.ToString());
            Console.WriteLine("Number of lines in model: {0}", modelInfo.property_line_count.ToString());
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

            // application.close_model(0, false);
        }


        static void CreateNewModel()
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            LogManager.Configuration = config;
            var logger = LogManager.GetCurrentClassLogger();
            string CurrentDirectory = Directory.GetCurrentDirectory();
            string Examples = @"..\ExampleFiles";
            string ExamplesDirectory = Path.Combine(CurrentDirectory, Examples);

            #region RSECTION Settings
            application_information RSECTIONInfo;
            try
            {
                application = new RSectionApplicationClient(Binding, Address);

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
                RSECTIONInfo = application.get_information();
                logger.Info("Name: {0}, Version:{1}, Type: {2}, language: {3} ", RSECTIONInfo.name, RSECTIONInfo.version, RSECTIONInfo.type, RSECTIONInfo.language_name);
                Console.WriteLine("Name: {0}, Version:{1}, Type: {2}, language: {3} ", RSECTIONInfo.name, RSECTIONInfo.version, RSECTIONInfo.type, RSECTIONInfo.language_name);
            }
            #endregion

            string modelName = "MyTestModel";
            string modelUrl = application.new_model(modelName);
            RSectionModelClient modelRsection = new RSectionModelClient(Binding, new EndpointAddress(modelUrl));

            point pointOne = new point()
            {
                no = 1,
                coordinate_1 = -0.25,
                coordinate_1Specified = true,
                coordinate_2 = -0.25,
                coordinate_2Specified = true,
            };
            point pointTwo = new point()
            {
                no = 2,
                coordinate_1 = 0.25,
                coordinate_1Specified = true,
                coordinate_2 = -0.25,
                coordinate_2Specified = true,
            };
            point pointThree = new point()
            {
                no = 3,
                coordinate_1 = 0.25,
                coordinate_1Specified = true,
                coordinate_2 = 0.25,
                coordinate_2Specified = true,
            };
            point pointFour = new point()
            {
                no = 4,
                coordinate_1 = -0.25,
                coordinate_1Specified = true,
                coordinate_2 = 0.25,
                coordinate_2Specified = true,
            };
            line sectionOutline = new line()
            {
                no = 1,
                definition_points = new int[] { pointOne.no, pointTwo.no, pointThree.no, pointFour.no, pointOne.no },
            };

            point pointFive = new point()
            {
                no = 5,
                coordinate_1 = -0.05,
                coordinate_1Specified = true,
                coordinate_2 = -0.05,
                coordinate_2Specified = true,
            };
            point pointSix = new point()
            {
                no = 6,
                coordinate_1 = 0.05,
                coordinate_1Specified = true,
                coordinate_2 = -0.05,
                coordinate_2Specified = true,
            };
            point pointSeven = new point()
            {
                no = 7,
                coordinate_1 = 0.05,
                coordinate_1Specified = true,
                coordinate_2 = 0.05,
                coordinate_2Specified = true,
            };
            point pointEight = new point()
            {
                no = 8,
                coordinate_1 = -0.05,
                coordinate_1Specified = true,
                coordinate_2 = 0.05,
                coordinate_2Specified = true,
            };
            line openingOutline = new line()
            {
                no = 2,
                definition_points = new int[] { pointFive.no, pointSix.no, pointSeven.no, pointEight.no, pointFive.no },
            };

            opening openingOne = new opening()
            {
                no = 1,
                boundary_lines = new int[] { openingOutline.no },
            };

            material concrete = new material()
            {
                no = 1,
                name = "C20/25 | EN 1992-1-1:2004/A1:2014",
                material_type = material_material_type.TYPE_CONCRETE,
            };

            part concretePart = new part()
            {
                no = 1,
                boundary_lines = new int[] { sectionOutline.no },
                material = 1,
                materialSpecified = true,
                integrated_openings = new int[] { openingOne.no },
                geometry = part_geometry.GEOMETRY_BOUNDARY_LINES,


            };

            section sectionRectangle = new section
            {
                no = 2,
                material = 1,
                materialSpecified = true,
                type = section_type.TYPE_PARAMETRIC_MASSIVE_I,
                typeSpecified = true,
                parametrization_type = section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_RECTANGLE__R_M1,
                parametrization_typeSpecified = true,
                name = "R_M1 0.5/1.0", // width/height as in RFEM, SI units
                location_of_centroidal_axis_y = 0.25,
                location_of_centroidal_axis_ySpecified = true,
                location_of_centroidal_axis_z = 0.5,
                location_of_centroidal_axis_zSpecified = true,
                //location_offset_y = -0.25,
                //location_offset_ySpecified = true,
                //location_y = 0.25,
                //location_ySpecified = true,q
                //location_z = 0.5,
                //location_zSpecified = true,

            };
    

            try
            {
                modelRsection.begin_modification("Points");
                modelRsection.set_point(pointOne);
                modelRsection.set_point(pointTwo);
                modelRsection.set_point(pointThree);
                modelRsection.set_point(pointFour);
                modelRsection.set_line(sectionOutline);
                modelRsection.set_point(pointFive);
                modelRsection.set_point(pointSix);
                modelRsection.set_point(pointSeven);
                modelRsection.set_point(pointEight);
                modelRsection.set_line(openingOutline);
                modelRsection.set_opening(openingOne);
                modelRsection.set_material(concrete);
                modelRsection.set_part(concretePart);
                modelRsection.set_section(sectionRectangle);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                logger.Error(exception, "Something wrong in setting of Points\n" + exception.Message + "\n");
                throw;
            }
            finally
            {
                try
                {
                    modelRsection.finish_modification();
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Something wrong in Points\n" + exception.Message + "\n");
                }
            }
            modelRsection.save(ExamplesDirectory + @"\Scripted.rsc");
            // application.close_model(0, false);


        }
        static void Main(string[] args)
        {
            switch (args[0])
            {
                case "OpenExistingModel":
                    OpenExistingModel();
                    break;
                case "CreateNewModel":
                    CreateNewModel();
                    break;
            }
        }
    }
}

