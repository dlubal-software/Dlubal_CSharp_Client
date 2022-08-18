using Dlubal.WS.Common.Tools;
using System;
using System.Collections.Generic;
using System.IO;

#if RFEM
using Dlubal.WS.Rfem6.Model;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public partial class TestingMethods
    {
        /// <summary>
        /// Creates load actions and change count of defined LCs.
        /// </summary>
        /// <remarks>
        /// Based on scripts\internal\tests\actionCombinationsPrepare.js
        /// </remarks>
        public static bool Test_Load\u2040Cases\u2040and\u2040Combinations_Action\u2040Combinations_Create()
        {
            DataLogger.AddLogStart("Create action combinations...");
            DataLogger.InitializeProgressBar(0, 110, 0);

            bool succeeded = InitializeTest();
            if (!succeeded)
            {
                return false;
            }

            List<WsObjectClass> scriptObjects;

            #region Run script

            try
            {
                DataLogger.AddLogStart("Generating action combinations via script 'actionCombinationsPrepare.js'...");

                SoapModelClient.run_script(Path.Combine(ScriptDirectoryPaths[0], @"internal\tests\actionCombinationsPrepare.js"));
                scriptObjects = GetObjects();
                SoapModelClient.delete_all();

                DataLogger.SetProgressBarValue(10);
            }
            catch (Exception ex)
            {
                succeeded = false;
                ModelWsExceptionHandler(ex);
                return false;
            }
            finally
            {
                DataLogger.AddLogEnd(succeeded ? DataLogger.LogResultType.DONE : DataLogger.LogResultType.FAILED);
            }

            #endregion Run script

            #region Set data

            try
            {
                DataLogger.AddLogStart("Creating action combinations...");

                SoapModelClient.begin_modification("Set action combinations");

                // design situation No.1
                design_situation designSituation = new design_situation()
                {
                    no = 1,
                    design_situation_type = "DESIGN_SITUATION_TYPE_EQU_PERMANENT_AND_TRANSIENT" // ULS (EQU) - Permanent and transient
                };

                SoapModelClient.set_design_situation(designSituation);

                DataLogger.SetProgressBarValue(20);

                // static analysis settings No. 1
                static_analysis_settings settings = new static_analysis_settings()
                {
                    no = 1
                };

                SoapModelClient.set_static_analysis_settings(settings);

                // combination wizard No. 1
                combination_wizard combinationWizard = new combination_wizard()
                {
                    no = 1,
                    static_analysis_settings = settings.no,
                    static_analysis_settingsSpecified = true,
                    user_defined_action_combinations = true,
                    user_defined_action_combinationsSpecified = true
                };

                SoapModelClient.set_combination_wizard(combinationWizard);

                // design situation No.2
                designSituation = new design_situation()
                {
                    no = 2,
                    design_situation_type = "DESIGN_SITUATION_TYPE_EQU_PERMANENT_AND_TRANSIENT", // ULS (EQU) - Permanent and transient
                    active = true,
                    activeSpecified = true,
                    combination_wizard = combinationWizard.no,
                    combination_wizardSpecified = true
                };

                SoapModelClient.set_design_situation(designSituation);

                // combination wizard No. 2
                combinationWizard = new combination_wizard()
                {
                    no = 2,
                    user_defined_action_combinations = true,
                    user_defined_action_combinationsSpecified = true,
                    generate_combinations = combination_wizard_generate_combinations.GENERATE_RESULT_COMBINATIONS,
                    generate_combinationsSpecified = true
                };

                SoapModelClient.set_combination_wizard(combinationWizard);

                // design situation No.3
                designSituation = new design_situation()
                {
                    no = 3,
                    design_situation_type = "DESIGN_SITUATION_TYPE_EQU_PERMANENT_AND_TRANSIENT", // ULS (EQU) - Permanent and transient
                    combination_wizard = combinationWizard.no,
                    combination_wizardSpecified = true
                };

                SoapModelClient.set_design_situation(designSituation);

                // action No. 7
                action action = new action()
                {
                    no = 7,
                    action_category = "ACTION_CATEGORY_PERMANENT_G",
                    action_type = action_action_type.ACTING_DIFFERENTLY,
                    action_typeSpecified = true,
                    is_active = true,
                    is_activeSpecified = true
                };

                SoapModelClient.set_action(action);

                // action No. 8
                action = new action()
                {
                    no = 8,
                    action_category = "ACTION_CATEGORY_PRESTRESS_P",
                    action_type = action_action_type.ACTING_SIMULTANEOUSLY,
                    action_typeSpecified = true,
                    comment = "comment",
                    user_defined_name_enabled = true,
                    user_defined_name_enabledSpecified = true,
                    name = "Second Action",
                    is_active = false,
                    is_activeSpecified = true
                };

                SoapModelClient.set_action(action);

                // load case No. 19
                load_case loadCase = new load_case
                {
                    no = 19,
                    action_category = "ACTION_CATEGORY_PERMANENT_G"
                };

                SoapModelClient.set_load_case(loadCase);

                // load case No. 20
                loadCase = new load_case
                {
                    no = 20,
                    action_category = "ACTION_CATEGORY_PERMANENT_G"
                };

                SoapModelClient.set_load_case(loadCase);

                // load case No. 21
                loadCase = new load_case
                {
                    no = 21,
                    action_category = "ACTION_CATEGORY_PRESTRESS_P"
                };

                SoapModelClient.set_load_case(loadCase);

                // update action 7 group numbers
                action = SoapModelClient.get_action(7);
                action.items[0].row.acting_group_number = 1;
                action.items[0].row.acting_group_numberSpecified = true;
                action.items[1].row.acting_group_number = 2;
                action.items[1].row.acting_group_numberSpecified = true;
                SoapModelClient.set_action(action);

                // action combination No. 1
                action_combination actionCombination = new action_combination()
                {
                    no = 1,
                    comment = "US18839",
                    user_defined_name_enabled = true,
                    user_defined_name_enabledSpecified = true,
                    name = "AC - test",
                    design_situation = 2,
                    design_situationSpecified = true,
                    items = new action_combination_items_row[]
                    {
                        new action_combination_items_row()
                        {
                            row = new action_combination_items
                            {
                                action_item = 7,
                                action_itemSpecified = true,
                                action_factor = 1.5,
                                action_factorSpecified = true
                            }
                        },
                        new action_combination_items_row()
                        {
                            row = new action_combination_items
                            {
                                action_item = 8,
                                action_itemSpecified = true,
                            }
                        }
                    }
                };

                SoapModelClient.set_action_combination(actionCombination);

                // action combination No. 2
                actionCombination = new action_combination()
                {
                    no = 2,
                    design_situation = 3,
                    design_situationSpecified = true,
                    items = new action_combination_items_row[]
                    {
                        new action_combination_items_row()
                        {
                            row = new action_combination_items
                            {
                                action_item = 7,
                                action_itemSpecified = true,
                                action_factor = 1.5,
                                action_factorSpecified = true,
                                operator_type = operator_type.OPERATOR_AND,
                                operator_typeSpecified = true,
                                left_parenthesis = true,
                                left_parenthesisSpecified = true,
                                group_factor = 2,
                                group_factorSpecified = true,
                                action_load_type = action_load_type.LOAD_TYPE_PERMANENT,
                                action_load_typeSpecified = true
                            }
                        },
                        new action_combination_items_row()
                        {
                            row = new action_combination_items
                            {
                                action_item = 8,
                                action_itemSpecified = true,
                                right_parenthesis = true,
                                right_parenthesisSpecified = true,
                                group_load_type = group_load_type.LOAD_TYPE_PERMANENT,
                                group_load_typeSpecified = true,
                                operator_type = operator_type.OPERATOR_OR,
                                operator_typeSpecified = true
                            }
                        },
                        new action_combination_items_row()
                        {
                            row = new action_combination_items
                            {
                                action_item  = 7,
                                action_itemSpecified = true
                            }
                        }
                    }
                };

                SoapModelClient.set_action_combination(actionCombination);

                // action combination No. 3
                actionCombination = new action_combination()
                {
                    no = 3,
                    design_situation = 3,
                    design_situationSpecified = true,
                    combination_type = action_combination_combination_type.ENVELOPE_PERMANENT,
                    combination_typeSpecified = true,
                    active = false,
                    activeSpecified = true
                };

                SoapModelClient.set_action_combination(actionCombination);

                DataLogger.SetProgressBarValue(100);
            }
            catch (Exception ex)
            {
                succeeded = false;
                ModelWsExceptionHandler(ex);
                return false;
            }
            finally
            {
                try
                {
                    SoapModelClient.finish_modification();
                }
                catch (Exception exception)
                {
                    succeeded = false;
                    ModelWsExceptionHandler(exception);
                    SoapModelClient.reset();
                }

                DataLogger.AddLogEnd(succeeded ? DataLogger.LogResultType.DONE : DataLogger.LogResultType.FAILED);
            }

            #endregion Set data

            #region Validate data

            DataLogger.AddLogStart("Verifying actions...");

            try
            {
                List<WsObjectClass> wsObjects = GetObjects();
                succeeded = AreListsEqual(scriptObjects, wsObjects);
                DataLogger.SetProgressBarValue(110);
            }
            catch (Exception ex)
            {
                ModelWsExceptionHandler(ex);
                return false;
            }
            finally
            {
                DataLogger.AddLogEnd(succeeded ? DataLogger.LogResultType.DONE : DataLogger.LogResultType.FAILED);
            }

            #endregion Validate data

            DataLogger.AddLogEnd(succeeded ? DataLogger.LogResultType.DONE : DataLogger.LogResultType.FAILED);
            DataLogger.ResetProgressBar();
            return succeeded;
        }
    }
}
