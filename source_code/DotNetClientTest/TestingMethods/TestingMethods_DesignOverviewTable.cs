using Dlubal.WS.Common.Tools;
using System;

#if RFEM
using Dlubal.WS.Rfem6.Model;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
        public static bool Test_Results_Design\u2040Overview\u2040Table()
        {
            DataLogger.AddLogStart("Getting design overview table...");
            DataLogger.InitializeProgressBar(0, 1, 0);

            try
            {
                SoapModelClient.calculate_all(false);
                design_overview_row[] members = SoapModelClient.get_design_overview();
                DataLogger.AddText($"Count rows of design overview table: {members.Length}");
                DataLogger.SetProgressBarValue(1);
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
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
