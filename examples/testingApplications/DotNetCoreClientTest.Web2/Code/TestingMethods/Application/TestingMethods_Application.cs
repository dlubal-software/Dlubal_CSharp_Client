using Dlubal.WS.RfemRstabSoapWsCoreLib.Tools;
using System;

#if RFEM
using Dlubal.WS.Rfem6.Application;
#elif RSTAB
using Dlubal.WS.Rstab9.Application;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
        public static bool Test_Application_Close()
        {
            DataLogger.AddLogStart("Closing application...");

            try
            {
                SoapApplicationClient.close_application();
            }
            catch (Exception exception)
            {
                ApplicationWsExceptionHandler(exception);
                return false;
            }

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }

        public static bool Test_Application_Get\u2040Information()
        {
            DataLogger.AddLogStart("Getting application information...");
            DataLogger.InitializeProgressBar(0, 100, 50);

            try
            {
                application_information info = SoapApplicationClient.get_information();

                DataLogger.AddText($"Name = {info.name}");
                DataLogger.AddText($"Type = {info.type}");
                DataLogger.AddText($"Version = {info.version}");
                DataLogger.AddText($"Language name = {info.language_name}");
                DataLogger.AddText($"Language id = {info.language_id}");
            }
            catch (Exception exception)
            {
                ApplicationWsExceptionHandler(exception);
                return false;
            }
            finally
            {
                DataLogger.ResetProgressBar();
            }

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }
    }
}
