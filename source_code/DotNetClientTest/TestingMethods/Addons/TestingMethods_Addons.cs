#if RFEM || RSTAB

using Dlubal.WS.Common.Tools;
using System;

#if RFEM
using Dlubal.WS.Rfem6.Model;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public partial class TestingMethods
    {
        public static bool Test_Addons_Get\u2040Statuses()
        {
            DataLogger.AddLogStart("Get addon statuses...");
            DataLogger.InitializeProgressBar(0, 100, 50);

            try
            {
                DumpObject(SoapModelClient.get_addon_statuses());
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

        public static bool Test_Addons_Set\u2040Analysis\u2040Addons\u2040Active()
        {
            DataLogger.AddLogStart("Set analysis addons active...");
            DataLogger.InitializeProgressBar(0, 100, 50);

            try
            {
                addon_list_type addons = new addon_list_type()
                {
                    analysis = new addon_list_analysis_list_type
                    {
                        structure_stability_active = true,
                        structure_stability_activeSpecified = true,
#if !RSTAB
                        material_nonlinear_analysis_active = true,
                        material_nonlinear_analysis_activeSpecified = true,
                        form_finding_active = true,
                        form_finding_activeSpecified = true,
#endif
                    }
                };

                SoapModelClient.set_addon_statuses(addons);

                addons = SoapModelClient.get_addon_statuses();
                if (addons.analysis.structure_stability_active != true
#if !RSTAB
                    || addons.analysis.material_nonlinear_analysis_active != true
                    || addons.analysis.form_finding_active != true
#endif
                    )
                {
                    throw new Exception("At least one of activated modules is not active.");
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

        public static bool Test_Addons_Set\u2040Dynamic\u2040Analysis\u2040Addons\u2040Active()
        {
            DataLogger.AddLogStart("Set dynamic analysis addons active...");
            DataLogger.InitializeProgressBar(0, 100, 50);

            try
            {
                addon_list_type addons = new addon_list_type()
                {
                    dynamic_analysis_settings = new addon_list_dynamic_analysis_settings_list_type
                    {
                        spectral_active = true,
                        spectral_activeSpecified = true,
                    }
                };

                SoapModelClient.set_addon_statuses(addons);

                addons = SoapModelClient.get_addon_statuses();
                if (addons.dynamic_analysis_settings.spectral_active != true)
                {
                    throw new Exception("At least one of activated modules is not active.");
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

#if !RSTAB
        public static bool Test_Addons_Set\u2040Special\u2040Solution\u2040Addons\u2040Active()
        {
            DataLogger.AddLogStart("Set special solution addons active...");
            DataLogger.InitializeProgressBar(0, 100, 50);

            try
            {
                addon_list_type addons = new addon_list_type()
                {
                    special_solutions = new addon_list_special_solutions_list_type
                    {
                        building_model_active = true,
                        building_model_activeSpecified = true,
                        geotechnical_analysis_active = true,
                        geotechnical_analysis_activeSpecified = true,
                    }
                };

                SoapModelClient.set_addon_statuses(addons);

                addons = SoapModelClient.get_addon_statuses();
                if (addons.special_solutions.building_model_active != true ||
                    addons.special_solutions.geotechnical_analysis_active != true)
                {
                    throw new Exception("At least one of activated modules is not active.");
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
#endif // !RSTAB

        public static bool Test_Addons_Set\u2040Design\u2040Addons\u2040Active()
        {
            DataLogger.AddLogStart("Set design addons active...");
            DataLogger.InitializeProgressBar(0, 100, 50);

            try
            {
                addon_list_type addons = new addon_list_type()
                {
                    design_addons = new addon_list_design_addons_list_type
                    {
                        concrete_design_active = true,
                        concrete_design_activeSpecified = true,
                        steel_design_active = true,
                        steel_design_activeSpecified = true,
                        timber_design_active = true,
                        timber_design_activeSpecified = true,
                        aluminum_design_active = true,
                        aluminum_design_activeSpecified = true,
                    }
                };

                SoapModelClient.set_addon_statuses(addons);

                addons = SoapModelClient.get_addon_statuses();
                if (addons.design_addons.concrete_design_active != true ||
                    addons.design_addons.steel_design_active != true ||
                    addons.design_addons.timber_design_active != true ||
                    addons.design_addons.aluminum_design_active != true)
                {
                    throw new Exception("At least one of activated modules is not active.");
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
    }
}

#endif // RFEM RSTAB