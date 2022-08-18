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
        public static bool Test_Formulas_Get\u2040Global\u2040Parameters()
        {
            DataLogger.AddLogStart("Getting global parameters...");

            bool succeeded = true;

            try
            {
                int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_GLOBAL_PARAMETER, 0);
                if (numbers.Length > 0)
                {
                    DataLogger.InitializeProgressBar(0, numbers.Length, 0);

                    for (int i = 0; i < numbers.Length; i++)
                    {
                        DataLogger.SetProgressBarValue(i);

                        global_parameter parameter = SoapModelClient.get_global_parameter(numbers[i]);

                        DataLogger.AddLogStart($"Parameter No.{parameter.no}");
                        try
                        {
                            DumpObject(parameter);
                        }
                        catch (Exception exception)
                        {
                            ModelWsExceptionHandler(exception);
                            succeeded = false;
                        }
                        finally
                        {
                            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
                        }
                        //DataLogger.AddText($"{parameter.no}: {parameter.name}, {parameter.symbol}, {parameter.definition_type}, ");
                    }
                }
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                succeeded = false;
            }
            finally
            {
                DataLogger.ResetProgressBar();
            }

            DataLogger.AddLogEnd(succeeded ? DataLogger.LogResultType.DONE : DataLogger.LogResultType.FAILED);
            return succeeded;
        }

        public static bool Test_Formulas_Get\u2040List\u2040Of\u2040Parameters\u2040Formula\u2040Allowed()
        {
            DataLogger.AddLogStart("Getting list of parameters formula allowed...");

            bool succeeded = true;

            Array types = Enum.GetValues(typeof(object_types));

            DataLogger.InitializeProgressBar(0, types.Length, 0);

            for (int i = 0; i < types.Length; i++)
            {
                try
                {
                    object_types type = (object_types)types.GetValue(i);
                    if (TypesWithParent.Contains(type))
                    {
                        continue;
                    }

                    int[] numbers = SoapModelClient.get_all_object_numbers(type, 0);
                    if (numbers.Length > 0)
                    {
                        DataLogger.AddLogStart($"Getting {type} parameters...");

                        object_location location = new object_location
                        {
                            no = numbers[0],
                            type = type,
                        };

                        string attributes = string.Empty;

                        object_parameter_location_type[] parameters = SoapModelClient.get_list_of_parameters_formula_allowed_for(location);
                        foreach (object_parameter_location_type parameter in parameters)
                        {
                            if (!string.IsNullOrEmpty(attributes))
                            {
                                attributes += ", ";
                            }
                            attributes += parameter.attribute;
                        }

                        DataLogger.AddText($"Attributes: {attributes}");

                        DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
                    }
                }
                catch (Exception exception)
                {
                    succeeded &= false;
                    ModelWsExceptionHandler(exception);
                }

                DataLogger.SetProgressBarValue(i);
            }

            DataLogger.ResetProgressBar();
            DataLogger.AddLogEnd(succeeded ? DataLogger.LogResultType.DONE : DataLogger.LogResultType.FAILED);
            return succeeded;
        }

        public static bool Test_Formulas_Get\u2040Formulas()
        {
            DataLogger.AddLogStart("Getting formulas...");

            bool succeeded = true;

            Array types = Enum.GetValues(typeof(object_types));

            DataLogger.InitializeProgressBar(0, types.Length, 0);

            for (int i = 0; i < types.Length; i++)
            {
                try
                {
                    object_types type = (object_types)types.GetValue(i);
                    if (TypesWithParent.Contains(type))
                    {
                        continue;
                    }

                    int[] numbers = SoapModelClient.get_all_object_numbers(type, 0);
                    foreach (int no in numbers)
                    {
                        object_location location = new object_location
                        {
                            no = no,
                            type = type,
                        };

                        object_parameter_location_type[] parameters = SoapModelClient.get_list_of_parameters_formula_allowed_for(location);
                        if (parameters.Length > 0)
                        {
                            DataLogger.AddLogStart($"Getting {type} No.{location.no} parameters...");

                            foreach (object_parameter_location_type parameter in parameters)
                            {
                                DataLogger.AddLogStart($"Attribute {parameter.attribute}");

                                DumpObject(SoapModelClient.get_formula(location, parameter));

                                DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
                            }

                            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
                        }
                    }
                }
                catch (Exception exception)
                {
                    succeeded &= false;
                    ModelWsExceptionHandler(exception);
                }

                DataLogger.SetProgressBarValue(i);
            }

            DataLogger.ResetProgressBar();
            DataLogger.AddLogEnd(succeeded ? DataLogger.LogResultType.DONE : DataLogger.LogResultType.FAILED);
            return succeeded;
        }
    }
}
