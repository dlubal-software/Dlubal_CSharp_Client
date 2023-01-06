using Dlubal.WS.Common.Tools;
using System;

#if RFEM
using Dlubal.WS.Rfem6.Model;
using MySectionList = Dlubal.WS.Rfem6.Model.get_my_section_lists__list;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
using MySectionList = Dlubal.WS.Rstab9.Model.get_my_section_lists__list;
#elif RSECTION
using Dlubal.WS.RSection1.Model;
using MySectionList = Dlubal.WS.RSection1.Model.get_my_section_lists__list;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
        public static bool Test_Basic\u2040Objects_Sections_Create()
        {
            const int MATERIAL_NO = 1;
            const int SECTION_NO = 2;

            DataLogger.AddLogStart("Creating section...");
            DataLogger.InitializeProgressBar(0, 30, 0);

            bool succeeded = InitializeTest();
            if (!succeeded)
            {
                return false;
            }

            DataLogger.SetProgressBarValue(10);

            try
            {
                SoapModelClient.begin_modification("set section");

                // create material
                DataLogger.AddText($"Set material No.{MATERIAL_NO}");
                material material = new material()
                {
                    no = MATERIAL_NO,
                    name = MATERIAL_STEEL,
                };
                SoapModelClient.set_material(material);

                DataLogger.SetProgressBarValue(20);

                // create section
                DataLogger.AddText($"Set section No.{SECTION_NO}");
                section section = new section
                {
                    no = SECTION_NO,
                    name = SECTION_RECTANGLE,
                    material = MATERIAL_NO,
                    materialSpecified = true
                };
                SoapModelClient.set_section(section);

                DataLogger.SetProgressBarValue(30);

                DataLogger.AddText("Section has been created.");
                succeeded = true;
            }
            catch (Exception exception)
            {
                succeeded = false;
                ModelWsExceptionHandler(exception);
            }
            finally
            {
                try
                {
                    SoapModelClient.finish_modification();
                    DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
                }
                catch (Exception exception)
                {
                    succeeded = false;
                    ModelWsExceptionHandler(exception);
                    SoapModelClient.reset();
                }

                DataLogger.ResetProgressBar();
            }

            return succeeded;
        }

        public static bool Test_Basic\u2040Objects_Sections_Create\u2040From\u2040Name()
        {
            const int MATERIAL_NO = 1;
            const int SECTION_NO = 2;

            const string BAD_SECTION_NAME = "bad section name";
            const string GOOD_SECTION_NAME = "RR_M1 0.51/1/0.11";

            DataLogger.AddLogStart("Creating section from name...");
            DataLogger.InitializeProgressBar(0, 40, 0);

            bool result = Test_General_Delete⁀All();
            if (!result || (SoapModelClient == null))
            {
                DataLogger.AddLogEnd(DataLogger.LogResultType.FAILED);
                return false;
            }

            DataLogger.SetProgressBarValue(10);

            try
            {
                SoapModelClient.begin_modification("create section from name");

                // create material
                DataLogger.AddText($"Set material No.{MATERIAL_NO}");
                material material = new material()
                {
                    no = MATERIAL_NO,
                    name = MATERIAL_STEEL,
                };
                SoapModelClient.set_material(material);

                DataLogger.SetProgressBarValue(20);

                DataLogger.AddLogStart($"Attempting to create section No.{SECTION_NO} from name \"{BAD_SECTION_NAME}\"...");
                try
                {
                    SoapModelClient.create_section_by_name(SECTION_NO, MATERIAL_NO, BAD_SECTION_NAME);
                }
                catch (Exception exception)
                {
                    DataLogger.AddText($"Exception caught: {exception.Message}");
                    DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
                    goto l1;
                }
                DataLogger.AddLogEnd(DataLogger.LogResultType.FAILED);
                throw new Exception($"Error: no exception was caught for section name \"{BAD_SECTION_NAME}\".");
            l1: DataLogger.SetProgressBarValue(30);

                DataLogger.AddText($"Create section No.{SECTION_NO} from name \"{GOOD_SECTION_NAME}\"");
                SoapModelClient.create_section_by_name(SECTION_NO, MATERIAL_NO, GOOD_SECTION_NAME);
                DataLogger.SetProgressBarValue(40);
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

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }

        public static bool Test_Basic\u2040Objects_Sections_My\u2040Section\u2040Lists()
        {
            const string mainListName = ""; // as of 04.11.2021, name of the main list of my sections is hardcoded in RxApp as empty string

            DataLogger.AddLogStart("Testing section my section lists...");
            DataLogger.InitializeProgressBar(0, 130, 0);

            MySectionList[] listsBackup;

            try
            {
                DataLogger.AddText("Backuping old my section lists...");
                listsBackup = SoapModelClient.get_my_section_lists();
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }

            bool OrderIndependentEquals(in MySectionList[] l, in MySectionList[] r_)
            {
                bool OrderIndependentEquals_(in string[] ls, in string[] rs_)
                {
                    if (ls.Length != rs_.Length)
                    {
                        return false;
                    }
                    var rs = (string[])rs_.Clone();
                    var rsLength = rs.Length;
                    foreach (string s in ls)
                    {
                        bool found = false;
                        for (uint i = 0; i != rsLength; ++i)
                        {
                            if (s == rs[i])
                            {
                                found = true;
                                if (--rsLength != i)
                                {
                                    rs[i] = rs[rsLength];
                                }
                                break;
                            }
                        }
                        if (!found)
                        {
                            return false;
                        }
                    }
                    return true;
                }

                if (l.Length != r_.Length)
                {
                    return false;
                }
                var r = (MySectionList[])r_.Clone();
                var rLength = r.Length;
                foreach (var listItem in l)
                {
                    bool found = false;
                    for (int i = 0; i != rLength; ++i)
                    {
                        if (listItem.name == r[i].name && OrderIndependentEquals_(listItem.list, r[i].list))
                        {
                            found = true;
                            if (--rLength != i)
                            {
                                r[i] = r[rLength];
                            }
                            break;
                        }
                    }
                    if (!found)
                    {
                        return false;
                    }
                }
                return true;
            }

            bool ClearMySectionLists()
            {
                try
                {
                    MySectionList[] listItems = SoapModelClient.get_my_section_lists();
                    foreach (var listItem in listItems)
                    {
                        if (listItem.name == mainListName) // main list cannot be deleted
                        {
                            foreach (string sectionName in listItem.list)
                            {
                                SoapModelClient.delete_section_from_my_section_list(listItem.name, sectionName);
                            }
                            continue;
                        }
                        SoapModelClient.delete_my_section_list(listItem.name);
                    }

                    listItems = SoapModelClient.get_my_section_lists();
                    if (
                        !OrderIndependentEquals(
                            listItems
                          , new MySectionList[]
                            {
                                new MySectionList
                                {
                                    name = mainListName
                                    , list = new string[0]
                                }
                            }
                        )
                    )
                    {
                        DataLogger.AddText("Failed to clear the my section lists.");
                        return false;
                    }
                }
                catch (Exception exception)
                {
                    ModelWsExceptionHandler(exception);
                    return false;
                }
                return true;
            }

            bool RestoreMySectionLists()
            {
                if (!ClearMySectionLists())
                {
                    return false;
                }

                try
                {
                    foreach (var listItem in listsBackup)
                    {
                        if (listItem.name != mainListName) // main list is already there
                        {
                            SoapModelClient.create_my_section_list(listItem.name);
                        }
                        foreach (string sectionName in listItem.list)
                        {
                            SoapModelClient.add_section_to_my_section_list(listItem.name, sectionName);
                        }
                    }

                    var listItems = SoapModelClient.get_my_section_lists();
                    if (!OrderIndependentEquals(listItems, listsBackup))
                    {
                        DataLogger.AddText("Failed to restore the my section lists.");
                        return false;
                    }
                }
                catch (Exception exception)
                {
                    ModelWsExceptionHandler(exception);
                    return false;
                }
                return true;
            }

            void CheckException(Action action, string startMessage, string errorMessage)
            {
                DataLogger.AddLogStart(startMessage);
                try
                {
                    action();
                }
                catch (Exception exception)
                {
                    DataLogger.AddText($"Exception caught: {exception.Message}");
                    DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
                    return;
                }
                DataLogger.AddLogEnd(DataLogger.LogResultType.FAILED);
                throw new Exception(errorMessage);
            }

            bool finished = false;

            try
            {
                DataLogger.AddText("Clearing my_section lists...");
                if (!ClearMySectionLists())
                {
                    throw new Exception("Failed to clear my_section lists.");
                }
                DataLogger.SetProgressBarValue(10);
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                const string badSectionName = "bad section name";
                CheckException(
                    () => SoapModelClient.add_section_to_my_section_list(mainListName, badSectionName)
                    , $"Attempting to add incorrect section name \"{badSectionName}\"..."
                    , $"Error: no exception was caught for section name \"{badSectionName}\"."
                );
                DataLogger.SetProgressBarValue(20);
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                string[] sectionNames =
                    {
                        "RR_M1 0.51/1/0.11"
                        , "Z 0.3/0.1/0.15/0.014/0.016/0.012/0.01/0.008/H"
                        , "PHS 5/0.3/0.01"
                        , "IS2FBW 0.4/0.25/0.18/0.01/0.014/0.012/0.2/0.1/0.008/0.016/0/0/0/0"
                    }
                ;
                DataLogger.AddText($"Adding correct section name \"{sectionNames[0]}\" to main list...");
                SoapModelClient.add_section_to_my_section_list(mainListName, sectionNames[0]);
                DataLogger.AddText($"Adding correct section name \"{sectionNames[1]}\" to main list...");
                SoapModelClient.add_section_to_my_section_list(mainListName, sectionNames[1]);
                DataLogger.SetProgressBarValue(30);
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                CheckException(
                    () => SoapModelClient.create_my_section_list(mainListName)
                    , $"Attempting to create a my section list with the name identical to the main list's name..."
                    , "Error: no exception was caught while creating my section list with the same name as the main list."
                );
                DataLogger.SetProgressBarValue(40);
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                string listName = "♥";
                DataLogger.AddText($"Creating my section list with name \"{listName}\"...");
                SoapModelClient.create_my_section_list(listName);
                DataLogger.SetProgressBarValue(50);
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                DataLogger.AddText("Checking lists contents...");
                {
                    var listItems = SoapModelClient.get_my_section_lists();
                    if (
                        !OrderIndependentEquals(
                            listItems
                          , new MySectionList[]
                            {
                                new MySectionList
                                {
                                    name = mainListName
                                    , list = new string[]{sectionNames[0], sectionNames[1]}
                                }
                              , new MySectionList
                                {
                                    name = listName
                                    , list = new string[0]
                                }
                            }
                        )
                    )
                    {
                        throw new Exception("My section lists contain unexpected data.");
                    }
                }
                DataLogger.SetProgressBarValue(53);
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                CheckException(
                    () => SoapModelClient.create_my_section_list(listName)
                    , $"Attempting to create a my section list with the same name \"{listName}\"..."
                    , "Error: no exception was caught while creating duplicate my section list."
                );
                DataLogger.SetProgressBarValue(55);
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                DataLogger.AddText($"Adding correct section name \"{sectionNames[2]}\" to the list \"{listName}\"...");
                SoapModelClient.add_section_to_my_section_list(listName, sectionNames[2]);
                DataLogger.AddText($"Adding correct section name \"{sectionNames[3]}\" to the list \"{listName}\"...");
                SoapModelClient.add_section_to_my_section_list(listName, sectionNames[3]);
                DataLogger.SetProgressBarValue(60);
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                CheckException(
                    () => SoapModelClient.add_section_to_my_section_list(listName, sectionNames[3])
                    , $"Attempting to add a section with the same name \"{sectionNames[3]}\"..."
                    , "Error: no exception was caught while adding duplicate section name."
                );
                DataLogger.SetProgressBarValue(65);
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                DataLogger.AddText("Checking lists contents...");
                {
                    var listItems = SoapModelClient.get_my_section_lists();
                    if (
                        !OrderIndependentEquals(
                            listItems
                          , new MySectionList[]
                            {
                                new MySectionList
                                {
                                    name = mainListName
                                    , list = new string[]{sectionNames[0], sectionNames[1]}
                                }
                              , new MySectionList
                                {
                                    name = listName
                                    , list = new string[]{sectionNames[2], sectionNames[3]}
                                }
                            }
                        )
                    )
                    {
                        throw new Exception("My section lists contain unexpected data.");
                    }
                }
                DataLogger.SetProgressBarValue(70);
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                DataLogger.AddText($"Deleting section name \"{sectionNames[0]}\" from the main list...");
                SoapModelClient.delete_section_from_my_section_list(mainListName, sectionNames[0]);
                DataLogger.AddText($"Deleting section name \"{sectionNames[2]}\" from the list \"{listName}\"...");
                SoapModelClient.delete_section_from_my_section_list(listName, sectionNames[2]);
                DataLogger.SetProgressBarValue(80);
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                CheckException(
                    () => SoapModelClient.delete_section_from_my_section_list(listName, sectionNames[2])
                    , $"Attempting to delete section name \"{sectionNames[2]}\" from the list \"{listName}\" again..."
                    , "Error: no exception was caught while deleting non-existent section name."
                );
                DataLogger.SetProgressBarValue(85);
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                DataLogger.AddText("Checking lists contents...");
                {
                    var listItems = SoapModelClient.get_my_section_lists();
                    if (
                        !OrderIndependentEquals(
                            listItems
                          , new MySectionList[]
                            {
                                new MySectionList
                                {
                                    name = mainListName
                                    , list = new string[]{sectionNames[1]}
                                }
                              , new MySectionList
                                {
                                    name = listName
                                    , list = new string[]{sectionNames[3]}
                                }
                            }
                        )
                    )
                    {
                        throw new Exception("My section lists contain unexpected data.");
                    }
                }
                DataLogger.SetProgressBarValue(90);
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                DataLogger.AddText($"Deleting my section list with name \"{listName}\"...");
                SoapModelClient.delete_my_section_list(listName);
                DataLogger.SetProgressBarValue(100);
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                CheckException(
                    () => SoapModelClient.delete_my_section_list(mainListName)
                    , "Attempting to delete the main my section list..."
                    , "Error: no exception was caught while deleting the main my section list."
                );
                DataLogger.SetProgressBarValue(110);
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                DataLogger.AddText("Checking lists contents...");
                {
                    var listItems = SoapModelClient.get_my_section_lists();
                    if (
                        !OrderIndependentEquals(
                            listItems
                          , new MySectionList[]
                            {
                                new MySectionList
                                {
                                    name = mainListName
                                    , list = new string[]{sectionNames[1]}
                                }
                            }
                        )
                    )
                    {
                        throw new Exception("My section lists contain unexpected data.");
                    }
                }
                DataLogger.SetProgressBarValue(120);
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                DataLogger.AddText("Restoring old my section lists...");
                if (!RestoreMySectionLists())
                {
                    return false;
                }
                DataLogger.SetProgressBarValue(130);
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                finished = true;
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            finally
            {
                if (!finished)
                {
                    RestoreMySectionLists();
                }

                DataLogger.ResetProgressBar();
            }

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }
    }
}
