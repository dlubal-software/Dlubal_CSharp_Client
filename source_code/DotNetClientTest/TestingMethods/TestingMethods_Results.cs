using Dlubal.WS.Common.Tools;
using System;
using System.IO;

#if RFEM
using Dlubal.WS.Rfem6.Model;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
        private static bool PlausibilityCheck(bool skipWarnings)
        {
            DataLogger.AddLogStart($"Starting plausibility check (SkipWarnings={skipWarnings})...");

            try
            {
                plausibility_check_result result = SoapModelClient.plausibility_check(skipWarnings);
                DumpObject(result);
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }

        public static bool Test_Results_Plausibility\u2040Check() => PlausibilityCheck(false);

        public static bool Test_Results_Plausibility\u2040Check\u2040With\u2040Skip\u2040Warnings() => PlausibilityCheck(true);

        public static bool Test_Results_Delete\u2040All()
        {
            DataLogger.AddLogStart("Deleting all results...");

            try
            {
                SoapModelClient.delete_all_results(true);
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }

        public static bool Test_Results_Calculate\u2040All()
        {
            DataLogger.AddLogStart("Calculating all results...");

            try
            {
                SoapModelClient.calculate_all(true);
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }

        public static bool Test_Results_Calculate\u2040All\u2040with\u2040Old\u2040Solver\u2040()
        {
            DataLogger.AddLogStart("Calculating all results (using XML solver input)...");

            try
            {
                SoapModelClient.calculate_all(false);
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }

        public static bool Test_Results_Generate\u2040and\u2040Validate\u2040XML\u2040Solver\u2040Input()
        {
            DataLogger.AddLogStart("Generating and validating XML solver input...");

            try
            {
                generate_and_validate_xml_solver_input_result result = SoapModelClient.generate_and_validate_xml_solver_input(Path.Combine(GetDataPath(), "SolverInput.xml"));
                DumpObject(result);
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }
    }
}
