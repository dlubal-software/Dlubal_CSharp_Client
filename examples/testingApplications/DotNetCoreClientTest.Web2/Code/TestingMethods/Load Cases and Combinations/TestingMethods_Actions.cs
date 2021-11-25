using Dlubal.WS.RfemRstabSoapWsCoreLib.Tools;
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
        /// Based on scripts\internal\tests\actionsPrepare.js
        /// </remarks>
        public static bool Test_Load\u2040Cases\u2040and\u2040Combinations_Actions_Create()
        {
            DataLogger.AddLogStart("Create actions...");
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
                DataLogger.AddLogStart("Generating actions via script 'actionsPrepare.js'...");

                SoapModelClient.run_script(Path.Combine(ScriptDirectoryPath, @"internal\tests\actionsPrepare.js"));
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
                DataLogger.AddLogStart("Creating actions...");

                SoapModelClient.begin_modification("Set actions");

                // action No. 10
                action action = new action()
                {
                    no = 10,
                    action_category = "Permanent | G",
                    action_type = action_action_type.ACTING_DIFFERENTLY,
                    action_typeSpecified = true,
                };

                SoapModelClient.set_action(action);

                DataLogger.SetProgressBarValue(20);

                // action No. 11
                action = new action()
                {
                    no = 11,
                    action_category = "Prestress | P",
                    action_type = action_action_type.ACTING_SIMULTANEOUSLY,
                    action_typeSpecified = true,
                    comment = "comment",
                    user_defined_name_enabled = true,
                    user_defined_name_enabledSpecified = true,
                    name = "Second Action"
                };

                SoapModelClient.set_action(action);

                DataLogger.SetProgressBarValue(30);

                // load case No. 12
                load_case loadCase = new load_case
                {
                    no = 12,
                    action_category = "Permanent | G"
                };

                SoapModelClient.set_load_case(loadCase);

                DataLogger.SetProgressBarValue(40);

                // load case No. 13
                loadCase = new load_case
                {
                    no = 13,
                    action_category = "Permanent | G"
                };

                SoapModelClient.set_load_case(loadCase);

                DataLogger.SetProgressBarValue(50);

                // load case No. 14
                loadCase = new load_case
                {
                    no = 14,
                    action_category = "Prestress | P"
                };

                SoapModelClient.set_load_case(loadCase);

                DataLogger.SetProgressBarValue(60);

                // load case No. 15
                loadCase = new load_case
                {
                    no = 15,
                    action_category = "Temperature (non-fire) | QT"
                };

                SoapModelClient.set_load_case(loadCase);

                DataLogger.SetProgressBarValue(70);

                // load case No. 16
                loadCase = new load_case
                {
                    no = 16,
                    action_category = "Temperature (non-fire) | QT"
                };

                SoapModelClient.set_load_case(loadCase);

                DataLogger.SetProgressBarValue(80);

                // action No. 13
                action = new action
                {
                    no = 13,
                    action_category = "Prestress | P"
                };

                SoapModelClient.set_action(action);

                DataLogger.SetProgressBarValue(90);

                // action No. 14
                action = new action
                {
                    no = 14,
                    action_category = "None | None",
                    action_type = action_action_type.ACTING_SIMULTANEOUSLY,
                    action_typeSpecified = true,
                    has_short_duration = true,
                    has_short_durationSpecified = true,
                    has_duration_shorter_than_one_month = true,
                    has_duration_shorter_than_one_monthSpecified = true
                };

                SoapModelClient.set_action(action);

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

        /// <summary>
        /// Creates load actions and change count of defined LCs.
        /// </summary>
        public static bool Test_Load\u2040Cases\u2040and\u2040Combinations_Actions_Modify()
        {
            const int LOAD_CASE_1_NO = 1;
            const int LOAD_CASE_2_NO = 2;
            const int LOAD_CASE_3_NO = 3;
            const int LOAD_CASE_4_NO = 4;
            const int LOAD_CASE_5_NO = 5;
            const int ACTION_1_NO = 1;

            DataLogger.AddLogStart("Creating action...");
            DataLogger.InitializeProgressBar(0, 30, 0);

            bool succeeded = InitializeTest();
            if (!succeeded)
            {
                return false;
            }

            try
            {
                bool ValidateItemCount(int actionNo, int itemsCount)
                {
                    action a = SoapModelClient.get_action(actionNo);

                    if (a.items.Length != itemsCount)
                    {
                        DataLogger.AddError($"Verification failed - count of items does not match {a.items.Length} != {itemsCount}");
                        return false;
                    }

                    if (!a.is_active)
                    {
                        DataLogger.AddError($"Verification failed - is not active");
                        return false;

                    }

                    DataLogger.AddText("Verification succeeded");
                    return true;
                }

                SoapModelClient.begin_modification("Set actions");

                action action = new action()
                {
                    no = ACTION_1_NO
                };

                action.items = new action_items[]
                {
                    new action_items() { no = 1, noSpecified = true, load_case_item = LOAD_CASE_1_NO, load_case_itemSpecified = true }
                };

                DataLogger.AddText($"Set action No.{action.no} with {action.items.Length} items");

                SetLoadCase(LOAD_CASE_1_NO);
                SoapModelClient.set_action(action);
                succeeded &= ValidateItemCount(ACTION_1_NO, action.items.Length);

                DataLogger.SetProgressBarValue(10);

                action.items = new action_items[]
                {
                    new action_items() { no = 1, noSpecified = true, load_case_item = LOAD_CASE_1_NO, load_case_itemSpecified = true },
                    new action_items() { no = 2, noSpecified = true, load_case_item = LOAD_CASE_2_NO, load_case_itemSpecified = true },
                    new action_items() { no = 3, noSpecified = true, load_case_item = LOAD_CASE_3_NO, load_case_itemSpecified = true },
                    new action_items() { no = 4, noSpecified = true, load_case_item = LOAD_CASE_4_NO, load_case_itemSpecified = true },
                    new action_items() { no = 5, noSpecified = true, load_case_item = LOAD_CASE_5_NO, load_case_itemSpecified = true }
                };

                DataLogger.AddText($"Set action No.{action.no} with {action.items.Length} items");

                SetLoadCase(LOAD_CASE_2_NO);
                SetLoadCase(LOAD_CASE_3_NO);
                SetLoadCase(LOAD_CASE_4_NO);
                SetLoadCase(LOAD_CASE_5_NO);
                SoapModelClient.set_action(action);
                succeeded &= ValidateItemCount(ACTION_1_NO, action.items.Length);

                DataLogger.SetProgressBarValue(20);

                action.items = new action_items[]
                {
                    new action_items() { no = 1, noSpecified = true, load_case_item = LOAD_CASE_1_NO, load_case_itemSpecified = true },
                    new action_items() { no = 2, noSpecified = true, load_case_item = LOAD_CASE_2_NO, load_case_itemSpecified = true },
                    new action_items() { no = 2, noSpecified = true, load_case_item = LOAD_CASE_3_NO, load_case_itemSpecified = true }
                };

                DataLogger.AddText($"Set action No.{action.no} with {action.items.Length} items");

                SoapModelClient.delete_object(object_types.E_OBJECT_TYPE_LOAD_CASE, LOAD_CASE_5_NO, 0);
                SoapModelClient.delete_object(object_types.E_OBJECT_TYPE_LOAD_CASE, LOAD_CASE_4_NO, 0);
                SoapModelClient.set_action(action);
                succeeded &= ValidateItemCount(ACTION_1_NO, action.items.Length);

                DataLogger.SetProgressBarValue(30);
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
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
                    ModelWsExceptionHandler(exception);
                    SoapModelClient.reset();
                }

                DataLogger.ResetProgressBar();
            }

            DataLogger.AddLogEnd(succeeded ? DataLogger.LogResultType.DONE : DataLogger.LogResultType.FAILED);
            return succeeded;
        }
    }
}
