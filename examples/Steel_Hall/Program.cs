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
using System.ServiceModel.Description;
using Dlubal.WS.Rfem6.Model;

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
            //Welcome message
            Console.WriteLine("Steel Hall Generator for RFEM6 and RSTAB9");

            //get input parameters from the user
            Console.Write("Height of frame []: ");
            double frameHeight = Convert.ToDouble(Console.ReadLine());
            Console.Write("Frame span [m]: ");
            double frameSpan = Convert.ToDouble(Console.ReadLine());
            Console.Write("Number of frames: ");
            int frameNumber = int.Parse(Console.ReadLine());
            Console.Write("Distance between frames [m]: ");
            double frameDistance = Convert.ToDouble(Console.ReadLine());

            //Amount/position of bracing

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

                string modelName = "SteelFrame";
                string modelUrl = application.new_model(modelName);

                //definition of material
                material steel = new material
                {
                    no = 1,
                    name = "S235",
                };

                //definition of sections
                section section1 = new section
                {
                    no = 1,
                    material = steel.no,
                    materialSpecified= true,
                    name = "HEA 200",
                    comment = "frame section"
                };

                section section2 = new section
                {
                    no = 1,
                    material = steel.no,
                    materialSpecified = true,
                    name = "IPE 200",
                    comment = "side member section"
                };

                section section3 = new section
                {
                    no = 1,
                    material = steel.no,
                    materialSpecified = true,
                    name = "RD 16",
                    comment = "bracing section"
                };

                //create nodes
                SortedList<int, node> nodes = new SortedList<int, node>();
                int nodeId = 1;
                double xVector = 0.0;
                double yVector = 0.0;
                double zVector = 0.0;

                for (int i = 0; i < 2; i++)
                {
                    if (i == 1)
                    {
                        zVector = -frameHeight;
                    }

                    for (int j = 0; j < frameNumber * 2; i++)
                    {   
                        //counter to recognize when the next frame begins -> node number per plane
                        int k = (j + 1) * 2;

                        yVector = -frameDistance * k; 

                        node newNode = new()
                        {
                            no = nodeId,
                            coordinates = new vector_3d() { x = xVector, y = 0.0, z = 0.0 },
                            coordinate_system_type = node_coordinate_system_type.COORDINATE_SYSTEM_CARTESIAN,
                            coordinate_system_typeSpecified = true,
                        };
                        nodeId++;

                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                logger.Error(ex, "Stopped program because of exception :" + ex.Message);
            }
            
        }
    }
}

