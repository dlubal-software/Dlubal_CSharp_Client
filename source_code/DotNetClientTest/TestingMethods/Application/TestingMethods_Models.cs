using Dlubal.WS.Common.Tools;
using Microsoft.Win32;
using System;

#if RFEM
using Dlubal.WS.Rfem6.Model;
using ModelClient = Dlubal.WS.Rfem6.Model.RfemModelClient;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
using ModelClient = Dlubal.WS.Rstab9.Model.RstabModelClient;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
        public static bool Test_Models_Get\u2040List()
        {
            DataLogger.AddLogStart("Getting model list...");
            DataLogger.InitializeProgressBar(0, 100, 50);

            try
            {
                string[] models = SoapApplicationClient.get_model_list();
                DataLogger.AddText($"{models.Length} models have been read:");
                foreach (string model in models)
                {
                    DataLogger.AddText($"  {model}");
                }
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

        public static bool Test_Models_New\u2040Model()
        {
            DataLogger.AddLogStart("Creating new model...");
            DataLogger.InitializeProgressBar(0, 30, 0);

            try
            {
                string[] models = SoapApplicationClient.get_model_list();

                DataLogger.AddText($"{models.Length} models before");
                DataLogger.SetProgressBarValue(10);

                string endpoint = SoapApplicationClient.new_model("test");

                DataLogger.AddText($"New model endpoint: {endpoint}");
                DataLogger.SetProgressBarValue(20);

                models = SoapApplicationClient.get_model_list();

                DataLogger.AddText($"{models.Length} models after");
                DataLogger.SetProgressBarValue(30);
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

        public static bool Test_Models_Open\u2040Model()
        {
            DataLogger.AddLogStart("Opening model...");
            DataLogger.InitializeProgressBar(0, 30, 0);

            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
#if RFEM
                    DefaultExt = ".rf6",
                    Filter = "RFEM6 Files (*.rf6)|*.rf6|Other Files (*.*)|*.*"
#elif RSTAB
                    DefaultExt = ".rs9",
                    Filter = "RSTAB9 Files (*.rs9)|*.rs9|Other Files (*.*)|*.*"
#endif
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string[] models = SoapApplicationClient.get_model_list();

                    DataLogger.AddText($"{models.Length} models before");
                    DataLogger.SetProgressBarValue(10);

                    DataLogger.AddText($"Model path: {openFileDialog.FileName}");

                    string endpoint = SoapApplicationClient.open_model(openFileDialog.FileName);

                    DataLogger.AddText($"Model endpoint: {endpoint}");
                    DataLogger.SetProgressBarValue(20);

                    models = SoapApplicationClient.get_model_list();

                    DataLogger.AddText($"{models.Length} models after");
                    DataLogger.SetProgressBarValue(30);
                }
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

        public static bool Test_Models_Save\u2040Active\u2040Model()
        {
            DataLogger.AddLogStart("Saving active model...");

            try
            {
                if (!SoapModelClient.save(""))
                {
                    DataLogger.AddError("An error occurred while saving the file.");
                }
            }
            catch (Exception exception)
            {
                ApplicationWsExceptionHandler(exception);
                return false;
            }

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }

        public static bool Test_Models_Get\u2040Active\u2040Model()
        {
            DataLogger.AddLogStart("Getting active model...");
            DataLogger.InitializeProgressBar(0, 100, 50);

            try
            {
                string endpoint = SoapApplicationClient.get_active_model();
                DataLogger.AddText($"Active model endpoint: {endpoint}");
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

        public static bool Test_Models_Get\u2040First\u2040Model()
        {
            DataLogger.AddLogStart("Getting first model...");
            DataLogger.InitializeProgressBar(0, 100, 50);

            try
            {
                string endpoint = SoapApplicationClient.get_model(0);
                DataLogger.AddText($"Model endpoint: {endpoint}");
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

        public static bool Test_Models_Close\u2040First\u2040Model()
        {
            DataLogger.AddLogStart("Closing first model...");
            DataLogger.InitializeProgressBar(0, 30, 0);

            try
            {
                string[] models = SoapApplicationClient.get_model_list();

                DataLogger.AddText($"{models.Length} models before");
                DataLogger.SetProgressBarValue(10);

                SoapApplicationClient.close_model(0, false);

                DataLogger.AddText("Model closed");
                DataLogger.SetProgressBarValue(20);

                models = SoapApplicationClient.get_model_list();

                DataLogger.AddText($"{models.Length} models after");
                DataLogger.SetProgressBarValue(30);

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

        public static bool Test_Models_Close\u2040And\u2040Save\u2040First\u2040Model()
        {
            DataLogger.AddLogStart("Closing and saving first model...");
            DataLogger.InitializeProgressBar(0, 30, 0);

            try
            {
                string[] models = SoapApplicationClient.get_model_list();

                DataLogger.AddText($"{models.Length} models before");
                DataLogger.SetProgressBarValue(10);

                SoapApplicationClient.close_model(0, true);

                DataLogger.AddText("Model closed");
                DataLogger.SetProgressBarValue(20);

                models = SoapApplicationClient.get_model_list();

                DataLogger.AddText($"{models.Length} models after");
                DataLogger.SetProgressBarValue(30);

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

        public static bool Test_Models_Read\u2040count\u2040of\u2040nodes\u2040from\u2040all\u2040models()
        {
            DataLogger.AddLogStart("Reading count of nodes from all models...");

            try
            {
                string[] model_names = SoapApplicationClient.get_model_list();

                DataLogger.InitializeProgressBar(0, model_names.Length, 0);

                for (int i = 0; i < model_names.Length; i++)
                {
                    string endpoint = SoapApplicationClient.get_model(i);
                    DataLogger.AddText($"Model '{model_names[i]}' endpoint = {endpoint}");

                    ModelClient temporarySoapModelClient = null;

                    try
                    {
                        temporarySoapModelClient = new ModelClient(Session.Binding, new System.ServiceModel.EndpointAddress(endpoint));
                        int count = temporarySoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_NODE, 0);
                        DataLogger.AddText($"Node count = {count}");
                    }
                    catch (Exception exception)
                    {
                        if (temporarySoapModelClient != null)
                        {
                            if (temporarySoapModelClient.State != System.ServiceModel.CommunicationState.Faulted)
                            {
                                temporarySoapModelClient.Close();
                            }
                            else
                            {
                                temporarySoapModelClient.Abort();
                            }

                            temporarySoapModelClient = null;
                        }

                        DataLogger.ReportError(exception);
                        DataLogger.AddSeparator();
                    }

                    DataLogger.SetProgressBarValue(i);
                }
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

        public static bool Test_Models_Run\u2040Script()
        {
            DataLogger.AddLogStart("Running script...");
            DataLogger.InitializeProgressBar(0, 20, 0);

            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    DefaultExt = ".js",
                    Filter = "RFEM6 Script Files (*.js)|*.js"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    DataLogger.SetProgressBarValue(10);

                    SoapModelClient.run_script(openFileDialog.FileName);

                    DataLogger.SetProgressBarValue(20);
                }
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

        public static bool Test_Models_Automatic\u2040Tests\u2040Workflow()
        {
            DataLogger.AddLogStart("Testing workflow of automatic tests...");
            DataLogger.InitializeProgressBar(0, 60, 0);

            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
#if RFEM
                    DefaultExt = ".rf6",
                    Filter = "RFEM Files (*.rf6; *.rf5)|*.rf6; *.rf5|Other Files (*.*)|*.*"
#elif RSTAB
                    DefaultExt = ".rs9",
                    Filter = "RSTAB Files (*.rs9; *.rs8)|*.rs9; *.rs8|Other Files (*.*)|*.*"
#endif
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    DataLogger.AddLogStart("Opening model...");

                    string endpoint = SoapApplicationClient.open_model(openFileDialog.FileName);

                    DataLogger.AddText($"Model endpoint: {endpoint}");
                    DataLogger.SetProgressBarValue(10);
                    DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);

                    // Get method of property SoapModelClient starts connection to model WS.
                    // Starting connection takes some time and we don't want to include it to calculation time.
                    SoapModelClient.ToString();

                    DataLogger.AddLogStart("Calculation all...");
                    SoapModelClient.calculate_all(false);
                    DataLogger.SetProgressBarValue(20);
                    DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);

                    DataLogger.AddLogStart("Export results...");
                    SoapModelClient.export_result_tables_to_csv(GetDataPath());
                    DataLogger.SetProgressBarValue(30);
                    DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);

                    DataLogger.AddLogStart("Closing model connection...");
                    Session.CloseModelClient();
                    DataLogger.SetProgressBarValue(40);
                    DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);

                    DataLogger.AddLogStart("Closing model...");
                    SoapApplicationClient.close_model(0, true);
                    DataLogger.SetProgressBarValue(50);
                    DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);

                    DataLogger.AddLogStart("Closing application connection...");
                    SoapApplicationClient.close_application();
                    Session.CloseApplicationClient();
                    DataLogger.SetProgressBarValue(60);
                    DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
                }
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
