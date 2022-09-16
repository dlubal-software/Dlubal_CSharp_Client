using Dlubal.WS.Common.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

#if RFEM
using Dlubal.WS.Rfem6.Model;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
#elif RSECTION
using Dlubal.WS.RSection1.Model;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
#if !RSECTION
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
#endif

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

#if !RSECTION
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
#endif

#if !RSECTION
        public static bool Test_Results_Has\u2040Results()
        {
            DataLogger.AddLogStart("Checking results existence...");

            try
            {
                bool hasAnyResults = SoapModelClient.has_any_results();
                DataLogger.AddText($"Has any results: {hasAnyResults}");

                IEnumerable<case_object_types> caseObjectTypes = Enum.GetValues(typeof(case_object_types)).Cast<case_object_types>();

                DataLogger.InitializeProgressBar(0, caseObjectTypes.Count(), 0);

                foreach (case_object_types caseObjectType in caseObjectTypes)
                {
                    object_types objectType = (object_types)Enum.Parse(typeof(object_types), caseObjectType.ToString());

                    int[] caseNumbers = SoapModelClient.get_all_object_numbers(objectType, 0);
                    foreach (int caseNo in caseNumbers)
                    {
                        bool hasResults = SoapModelClient.has_results(caseObjectType, caseNo);
                        DataLogger.AddText($"Has {caseObjectType} No.{caseNo} results: {hasResults}");
                    }

                    DataLogger.SetProgressBarValue(((int)caseObjectType) + 1);
                }
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

        private static MethodInfo[] resultGetterMethods;
        const string resultGetterPrefix = "get_results_for_";
        private static MethodInfo[] GetResultGetters()
        {
            if (resultGetterMethods is null)
            {
                resultGetterMethods =
                    (
                        from method in SoapModelClient.GetType().GetMethods()
                        where method.Name.StartsWith(resultGetterPrefix)
                        select method
                    )
                    .ToArray()
                ;
            }
            return resultGetterMethods;
        }

        private static bool GetResults(int objectNo)
        {
            bool succeeded = true;

            try
            {
                if (SoapModelClient.has_any_results())
                {
                    IEnumerable<case_object_types> caseObjectTypes = Enum.GetValues(typeof(case_object_types)).Cast<case_object_types>();

                    DataLogger.InitializeProgressBar(0, caseObjectTypes.Count(), 0);

                    MethodInfo[] resultGetters = GetResultGetters();

                    foreach (case_object_types caseObjectType in caseObjectTypes)
                    {
                        object_types objectType = (object_types)Enum.Parse(typeof(object_types), caseObjectType.ToString());

                        int[] caseNumbers = SoapModelClient.get_all_object_numbers(objectType, 0);
                        foreach (int caseNo in caseNumbers)
                        {
                            if (SoapModelClient.has_results(caseObjectType, caseNo))
                            {
                                DataLogger.AddLogStart($"{caseObjectType} No.{caseNo}...");

                                foreach (MethodInfo resultsGetter in resultGetters)
                                {
                                    try
                                    {
                                        Array results =
                                            resultsGetter.GetParameters().FirstOrDefault(x => x.Name == "no") == default(ParameterInfo)
                                            ? resultsGetter.Invoke(SoapModelClient, new object[] { caseObjectType, caseNo           }) as Array
                                            : resultsGetter.Invoke(SoapModelClient, new object[] { caseObjectType, caseNo, objectNo }) as Array
                                        ;
                                        if (results.Length > 0)
                                        {
                                            DataLogger.AddText($"{resultsGetter.Name.Substring(resultGetterPrefix.Length)} = {results.Length} items");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        succeeded = false;
                                        DataLogger.AddException($"{resultsGetter.Name}:\n{Utilities.ParseException(ex)}");
                                    }
                                }

                                DataLogger.AddLogEnd(succeeded);
                            }
                        }

                        DataLogger.SetProgressBarValue(((int)caseObjectType) + 1);
                    }
                }
                else
                {
                    DataLogger.AddText("No results available");
                }
            }
            catch (Exception exception)
            {
                succeeded = false;
                ModelWsExceptionHandler(exception);
            }
            finally
            {
                DataLogger.ResetProgressBar();
            }

            return succeeded;
        }

        public static bool Test_Results_Get\u2040All\u2040Results()
        {
            DataLogger.AddLogStart("Getting results...");

            // gets results for all objects with No.1
            bool succeeded = GetResults(1);

            DataLogger.AddLogEnd(succeeded);
            return succeeded;
        }

        public static bool Test_Results_Get\u2040All\u2040Results\u2040At\u2040Once()
        {
            DataLogger.AddLogStart("Getting all results at once...");

            // gets all results for all objects
            bool succeeded = GetResults(0);

            DataLogger.AddLogEnd(succeeded);
            return succeeded;
        }
#endif
    }
}
