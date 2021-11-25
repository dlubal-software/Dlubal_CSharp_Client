using Dlubal.WS.RfemRstabSoapWsCoreLib.Tools;
using Dlubal.WS.RfemRstabSoapWsCoreLib;
using System;
using System.IO;
using System.Reflection;
using DotNetCoreClientTest.Web2;

#if RFEM
using Dlubal.WS.Rfem6.Model;
using ApplicationClient = Dlubal.WS.Rfem6.Application.RfemApplicationClient;
using ModelClient = Dlubal.WS.Rfem6.Model.RfemModelClient;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
using ApplicationClient = Dlubal.WS.Rstab9.Application.RstabApplicationClient;
using ModelClient = Dlubal.WS.Rstab9.Model.RstabModelClient;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
        public enum TestResultType
        {
            PASSED = 0,
            FAILED = 1,
            SKIPPED = 2
        }

        public static DataLogger DataLogger
        {
            get
            {
                return Session.DataLogger;
            }
        }

        public static ApplicationClient SoapApplicationClient
        {
            get
            {
                return Session.SoapApplicationClient;
            }
        }

        public static ModelClient SoapModelClient
        {
            get
            {
                return Session.SoapModelClient;
            }
        }

        // Shared objects and parameters of tests should be placed here.
        private const string MATERIAL_STEEL = "S235"; // (EN 10025-2:2004-11) - zatim bez pouziti normy, aplikace s tim ma problemy
        private const string SECTION_I = "IPE 100";
        private const string SECTION_RECTANGLE = "R_M1 200/300"; // Rectangle 200/300

        //private static string materialTimber = "Hardwood Timber D30";

        public static void ApplicationWsExceptionHandler(Exception exception)
        {
            Session.ApplicationWsExceptionHandler(exception);
        }

        public static void ModelWsExceptionHandler(Exception exception)
        {
            Session.ModelWsExceptionHandler(exception);
        }

        public static string GetDataPath()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), MyConfig.DataFolder);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public static double DegToRad(double angle)
        {
            return angle / 180 * Math.PI;
        }

        public static bool InitializeTest()
        {
            if (!Test_General_Delete⁀All() || SoapModelClient == null)
            {
                DataLogger.AddLogEnd(DataLogger.LogResultType.FAILED);
                return false;
            }
            return true;
        }

        private delegate T GetObject<T>(int id);

        /// <summary>
        /// Gets all objects of specified type.
        /// </summary>
        /// <typeparam name="T">Template type of objects to read.</typeparam>
        /// <param name="handler">Delegate to reading function.</param>
        /// <param name="type">Type of object</param>
        /// <returns></returns>
        private static T[] GetAllObjects<T>(ref GetObject<T> handler, object_types type)
        {
            int[] numbers = SoapModelClient.get_all_object_numbers(type, 0);

            DataLogger.AddText($"Reading {numbers.Length} {typeof(T).Name}s...");

            int i = 0;
            T[] objects = new T[numbers.Length];
            foreach (int no in numbers)
            {
                objects[i++] = handler(no);
            }

            DataLogger.AddText("...done");
            return objects;
        }

        private delegate void SetObject<T>(T obj);

        /// <summary>
        /// Sets all objects to model.
        /// </summary>
        /// <typeparam name="T">Template type of objects to write.</typeparam>
        /// <param name="handler">Delegate to writing function.</param>
        /// <param name="objects">Array of object to write.</param>
        private static void SetAllObjects<T>(ref SetObject<T> handler, T[] objects)
        {
            DataLogger.AddText($"Writing {objects.Length} {typeof(T).Name}s...");

            foreach (T obj in objects)
            {
                handler(obj);
            }

            DataLogger.AddText("...done");
        }

        public static void DumpObject<T>(object obj)
        {
            Type type = typeof(T);

            PropertyInfo[] infos = type.GetProperties();
            foreach (PropertyInfo info in infos)
            {
                if (!info.Name.EndsWith("Specified", StringComparison.OrdinalIgnoreCase))
                {
                    DataLogger.AddText($"{info.Name} = {info.GetValue(obj)}");
                }
            }
        }

        /* *** Example of testing method ***
        /// <summary>
        /// This is example of testing method.
        /// Name of testing method has to start with word "Test" followed by group, subgroups and name of test at the end.
        /// Method is placed into the tree according to group and subgroups attached before the name of test.
        /// Each section of method name is separated by "_". Character \u2040 is used as a "space",
        /// because it is not possible to use real space character within the name of method.
        /// </summary>
        /// <returns>Each testing method must have boolean return value which contains result of test.</returns>
        public static bool Test_Group_Sub\u2040Group_Testing()
        {
            // This opens named section within the log.
            DataLogger.AddLogStart("Testing something...");

            // This initialize ProgressBar parameters (min, max, current).
            DataLogger.InitializeProgressBar(0, 100, 0);

            // SOAP WS calls should be placed within the try block followed by specific catch blocks.
            try
            {
                // Core of testing method should be placed here.
                SoapClient.delete_all();

                // If we want to write some data then we have to call begin_modification.
                SoapClient.begin_modification("caption");

                //...

                // This sets value of ProgressBar.
                DataLogger.SetProgressBarValue(50);

                //...

                // At the end we have to call finish_modification.
                SoapClient.finish_modification();
            }
            catch (Exception exception)
            {
                // This catches TimeoutException, FaultException, CommunicationException and also general Exception.
                WsExceptionHandler(exception);
                return false;
            }
            finally
            {
                // Sometimes we need to perform some operations before exiting.

                // This resets ProgressBar to its default state.
                DataLogger.ResetProgressBar();
            }

            // This closes the last opened named section within the log.
            // It contains one of three results [DONE, FAILED, CANCELED].
            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }
        */
    }
}