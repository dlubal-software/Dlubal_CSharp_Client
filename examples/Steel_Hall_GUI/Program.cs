using Microsoft.VisualBasic.Logging;
using System.Globalization;


using NLog;
using System.ServiceModel;

namespace Steel_Hall_GUI
{
    class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new HallGeneratorForm());

        }
    }
}