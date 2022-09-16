using Dlubal.WS.Common.Tools;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

#if RFEM
using Dlubal.WS.Rfem6.Model;
using Dlubal.WS.Rfem6.Application;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
using Dlubal.WS.Rstab9.Application;
#elif RSECTION
using Dlubal.WS.RSection1.Model;
using Dlubal.WS.RSection1.Application;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
        public static bool Test_Export\u2040and\u2040Import_Export\u2040Objects\u2040Information\u2040to\u2040XML()
        {
            DataLogger.AddLogStart("Exporting object information to XML...");

            bool succeeded = true;

            XmlDocument document = new XmlDocument();
            XmlElement list = document.CreateElement("object_information_list");
            document.AppendChild(list);

            Array types = Enum.GetValues(typeof(object_types));

            DataLogger.InitializeProgressBar(0, types.Length, 0);

            for (int i = 0; i < types.Length; i++)
            {
                try
                {
                    object_types type = (object_types)types.GetValue(i);

                    DataLogger.AddLogStart($"Getting {type} information...");

                    XmlAttribute typeAttribute = document.CreateAttribute("type");
                    typeAttribute.Value = type.ToString();

                    object_information info = SoapModelClient.get_object_information(type);

                    using (MemoryStream stream = new MemoryStream())
                    {
                        XmlTextWriter xmlWriter = new XmlTextWriter(stream, Encoding.UTF8);

                        XmlSerializer xmlSerializer = new XmlSerializer(info.GetType());
                        xmlSerializer.Serialize(xmlWriter, info);

                        string infoXmlString = Encoding.UTF8.GetString(stream.ToArray());
                        infoXmlString = infoXmlString.Remove(0, infoXmlString.IndexOf("?>") + 2);

                        XmlDocument infoDocument = new XmlDocument();
                        infoDocument.LoadXml(infoXmlString);

                        XmlNode node = document.ImportNode(infoDocument.FirstChild, true);
                        node.Attributes.RemoveAll();
                        node.Attributes.Append(typeAttribute);
                        list.AppendChild(node);
                    }

                    DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
                }
                catch (Exception exception)
                {
                    succeeded &= false;
                    ModelWsExceptionHandler(exception);
                }

                DataLogger.SetProgressBarValue(i);
            }

            using (XmlTextWriter writer = new XmlTextWriter(Path.Combine(GetDataPath(), Properties.Settings.Default.ObjectInformationFile), Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                document.WriteTo(writer);
                writer.Close();
            };

            DataLogger.ResetProgressBar();
            DataLogger.AddLogEnd(succeeded ? DataLogger.LogResultType.DONE : DataLogger.LogResultType.FAILED);
            return succeeded;
        }
#if !RSECTION
        public static bool Test_Export\u2040and\u2040Import_Export\u2040Result\u2040Tables\u2040to\u2040CSV()
        {
            DataLogger.AddLogStart("Exporting result tables to CSV...");

            try
            {
                SoapModelClient.export_result_tables_to_csv(GetDataPath());
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }
        public static bool Test_Export\u2040and\u2040Import_Export\u2040Result\u2040Tables\u2040With\u2040Detailed\u2040Members\u2040Results\u2040to\u2040CSV()
        {
            DataLogger.AddLogStart("Exporting result tables with detailed members results to CSV...");

            try
            {
                SoapModelClient.export_result_tables_with_detailed_members_results_to_csv(GetDataPath());
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }

        public static bool Test_Export\u2040and\u2040Import_Export\u2040Result\u2040Tables\u2040to\u2040XML()
        {
            DataLogger.AddLogStart("Exporting result tables to XML...");

            try
            {
                SoapModelClient.export_result_tables_to_xml(Path.Combine(GetDataPath(), "ResultTables.xml"));
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }

        public static bool Test_Export\u2040and\u2040Import_Export\u2040Result\u2040Tables\u2040With\u2040Detailed\u2040Members\u2040Results\u2040to\u2040XML()
        {
            DataLogger.AddLogStart("Exporting result tables with detailed members results to XML...");

            try
            {
                SoapModelClient.export_result_tables_with_detailed_members_results_to_xml(Path.Combine(GetDataPath(), "ResultTablesWithDetailedMemberResults.xml"));
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

        public static bool Test_Export\u2040and\u2040Import_Export\u2040Model()
        {
            DataLogger.AddLogStart("Exporting model to various file formats...");

            DataLogger.InitializeProgressBar(0, 14, 0);
            void incrementProgress()
            {
                DataLogger.SetProgressBarValue(DataLogger.ProgressBar.Value + 1);
            }
#if !RSECTION
            void closeModel(string name)
            {
                var modelList = SoapApplicationClient.get_model_list();
                for (int i = 0; i != modelList.Length; ++i)
                {
                    if (name == modelList[i])
                    {
                        SoapApplicationClient.close_model(i, false);
                        return;
                    }
                }
                throw new Exception($"Closing model failed: no model '{name}' is found");
            }
#endif
            try
            {
#if !RSECTION
                DataLogger.AddText("Export to gltf");
                SoapModelClient.export_to(Path.Combine(GetDataPath(), "exported_model.gltf"));
                incrementProgress();

                DataLogger.AddText("Export to glb");
                SoapModelClient.export_to(Path.Combine(GetDataPath(), "exported_model.glb"));
                incrementProgress();

                DataLogger.AddText("Export to vtk");
                SoapModelClient.export_to(Path.Combine(GetDataPath(), "exported_model.vtk"));
                incrementProgress();

                DataLogger.AddText("Export to xml");
                SoapModelClient.export_to(Path.Combine(GetDataPath(), "exported_model.xml"));
                incrementProgress();

                DataLogger.AddText("Export to vtm");
                SoapModelClient.export_to(Path.Combine(GetDataPath(), "exported_model.vtm"));
                incrementProgress();

                DataLogger.AddText("Change SAF settings");
                {
                    var safSettings = SoapApplicationClient.get_saf_settings();
                    safSettings.property_general_run_excel_application = false;
                    bool imperial = safSettings.property_export_set_unit_system_imperial;
                    safSettings.property_export_set_unit_system_imperial = !imperial;
                    SoapApplicationClient.set_saf_settings(safSettings);
                    safSettings = SoapApplicationClient.get_saf_settings();
                    if (safSettings.property_export_set_unit_system_imperial != !imperial)
                    {
                        throw new Exception("Failed to change SAF settings");
                    }
                    if (safSettings.property_export_set_unit_system_imperial != false)
                    {
                        safSettings.property_export_set_unit_system_imperial = false;
                        SoapApplicationClient.set_saf_settings(safSettings);
                    }
                }
                incrementProgress();

                DataLogger.AddText("Change conversion tables");
                {
                    bool Equal(in ConversionTables lhs, in ConversionTables rhs) // No default operator== in c#. Why not auto-generate it in soap ws client?
                    {
                        var lm = lhs.MaterialConversionTablesManager;
                        var rm = rhs.MaterialConversionTablesManager;
                        var ls = lhs.SectionConversionTablesManager;
                        var rs = rhs.SectionConversionTablesManager;
                        if (
                               lm.property_active_config != rm.property_active_config
                            || ls.property_active_config != rs.property_active_config
                            || lm.configs.Length != rm.configs.Length
                            || ls.configs.Length != rs.configs.Length
                        )
                        {
                            return false;
                        }
                        for (int i = 0; i != lm.configs.Length; ++i)
                        {
                            var lmct = lm.configs[i].ConversionTable.property_conversion_table;
                            var rmct = rm.configs[i].ConversionTable.property_conversion_table;
                            if (
                                   lm.configs[i].property_config_id != rm.configs[i].property_config_id
                                || lm.configs[i].property_config_user_name != rm.configs[i].property_config_user_name
                                || lmct.Length != rmct.Length
                            )
                            {
                                return false;
                            }
                            for (int j = 0; j != lmct.Length; ++j)
                            {
                                if (lmct[j].first != rmct[j].first || lmct[j].second != rmct[j].second)
                                {
                                    return false;
                                }
                            }
                        }
                        for (int i = 0; i != ls.configs.Length; ++i)
                        {
                            var lsct = ls.configs[i].ConversionTable.property_conversion_table;
                            var rsct = rs.configs[i].ConversionTable.property_conversion_table;
                            if (
                                   ls.configs[i].property_config_id != rs.configs[i].property_config_id
                                || ls.configs[i].property_config_user_name != rs.configs[i].property_config_user_name
                                || lsct.Length != rsct.Length
                            )
                            {
                                return false;
                            }
                            for (int j = 0; j != lsct.Length; ++j)
                            {
                                if (lsct[j].first != rsct[j].first || lsct[j].second != rsct[j].second)
                                {
                                    return false;
                                }
                            }
                        }
                        return true;
                    }

                    var conversionTables = SoapApplicationClient.get_conversion_tables();
                    {
                        var configs_ = conversionTables.MaterialConversionTablesManager.configs;
                        int configsSize = configs_.Length + 1;
                        var configs = new MaterialConversionTablesManager_config[configsSize];
                        int maxIndex = 0;
                        for (int i = 0; i != configsSize - 1; ++i)
                        {
                            configs[i] = configs_[i];
                            if (configs[i].property_config_id > maxIndex)
                            {
                                maxIndex = configs[i].property_config_id;
                            }
                        }
                        configs[configsSize - 1] =
                            new MaterialConversionTablesManager_config {
                                property_config_user_name = "♥"
                                , property_config_id = maxIndex + 1
                                , ConversionTable = new ConversionTable
                                  {
                                      property_conversion_table =
                                          new string_and_string_pair[]
                                          {
                                              new string_and_string_pair{ first = "♥♦♣♠", second = "♠♣♦♥" }
                                              , new string_and_string_pair{ first = "☺☻", second = "☻☺" }
                                          }
                                  }
                            }
                        ;
                        conversionTables.MaterialConversionTablesManager.configs = configs;
                        conversionTables.MaterialConversionTablesManager.property_active_config = maxIndex + 1;
                    }
                    {
                        var configs_ = conversionTables.SectionConversionTablesManager.configs;
                        int configsSize = configs_.Length + 1;
                        var configs = new SectionConversionTablesManager_config[configsSize];
                        int maxIndex = 0;
                        for (int i = 0; i != configsSize - 1; ++i)
                        {
                            configs[i] = configs_[i];
                            if (configs[i].property_config_id > maxIndex)
                            {
                                maxIndex = configs[i].property_config_id;
                            }
                        }
                        configs[configsSize - 1] =
                            new SectionConversionTablesManager_config
                            {
                                property_config_user_name = "♥"
                                , property_config_id = maxIndex + 1
                                , ConversionTable = new ConversionTable
                                {
                                    property_conversion_table =
                                          new string_and_string_pair[]
                                          {
                                              new string_and_string_pair{ first = "♥♦♣♠", second = "♠♣♦♥" }
                                              , new string_and_string_pair{ first = "☺☻", second = "☻☺" }
                                          }
                                }
                            }
                        ;
                        conversionTables.SectionConversionTablesManager.configs = configs;
                        conversionTables.SectionConversionTablesManager.property_active_config = maxIndex + 1;
                    }
                    SoapApplicationClient.set_conversion_tables(conversionTables);
                    if (!Equal(conversionTables, SoapApplicationClient.get_conversion_tables()))
                    {
                        throw new Exception("Failed to change conversion tables");
                    }
                }
                incrementProgress();

                DataLogger.AddText("Export to saf");
                SoapModelClient.export_to(Path.Combine(GetDataPath(), "exported_model.saf"));
                incrementProgress();

                DataLogger.AddText("Export to xlsx");
                SoapModelClient.export_to(Path.Combine(GetDataPath(), "exported_model.xlsx"));
                incrementProgress();

                // DataLogger.AddText("Import from xml"); // bug in RFEM team9/master bd236862f0d
                // SoapApplicationClient.import_from(Path.Combine(GetDataPath(), "exported_model.xml"));
                // closeModel("model");
                incrementProgress();

                DataLogger.AddText("Import from saf");
                SoapApplicationClient.import_from(Path.Combine(GetDataPath(), "exported_model.saf"));
                closeModel("exported_model");
                incrementProgress();

                DataLogger.AddText("Import from xlsx");
                SoapApplicationClient.import_from(Path.Combine(GetDataPath(), "exported_model.xlsx"));
                closeModel("exported_model");
                incrementProgress();

                /* // bug in RFEM team9/master bd236862f0d
                DataLogger.AddText("Export to ifc");
                var settings = new export_to_ifc_settings_type
                {
                    mirror_axis_x = false
                    , mirror_axis_y = false
                    , mirror_axis_z = false
                    , origin_coordinate_x = 0.0
                    , origin_coordinate_y = 0.0
                    , origin_coordinate_z = 0.0
                    , axis_rotation_sequence = export_to_ifc_axis_rotation_sequence_type.X_Y_Z
                    , rotation_angle_0 = 0.0
                    , rotation_angle_1 = 0.0
                    , rotation_angle_2 = 0.0
                    , switch_axis_x = export_to_ifc_axis_type.X
                    , switch_axis_y = export_to_ifc_axis_type.Y
                    , switch_axis_z = export_to_ifc_axis_type.Z
                    , remove_accents = false
                    , export_type = export_to_ifc_export_type.E_EXPORT_IFC4_REFERENCE_VIEW
                };
                SoapModelClient.export_to_ifc(
                    Path.Combine(GetDataPath(), "exported_model.ifc")
                    , settings
                    , new export_to_ifc_object_location_type[]{}
                );
                SoapModelClient.export_to_ifc(
                    Path.Combine(GetDataPath(), "exported_model.ifc")
                    , settings
                    , new export_to_ifc_object_location_type[]
                      {
                          new export_to_ifc_object_location_type{ type = object_types.E_OBJECT_TYPE_NODE, no = 1 }
                      }
                );
                */
                incrementProgress();
#endif // !RSECTION
                DataLogger.AddText("Export to tables");
                {
                    var tableExportConfigManager = SoapModelClient.get_table_export_config_manager(); // this config is huge, so testing setter/getter for it is going to be painful...
                    var tableExportMainConfig = tableExportConfigManager.configs[tableExportConfigManager.property_active_config - 1].TableExportConfigBase.TableExportMainConfig;
                    tableExportMainConfig.property_export_target = TableExportMainConfig_property_export_target_type.E_EXPORT_TARGET_CSV;
                    SoapModelClient.set_table_export_config_manager(tableExportConfigManager);
                    var target = SoapModelClient.get_table_export_config_manager()
                        .configs[tableExportConfigManager.property_active_config - 1]
                        .TableExportConfigBase
                        .TableExportMainConfig
                        .property_export_target
                    ;
                    if (target != TableExportMainConfig_property_export_target_type.E_EXPORT_TARGET_CSV)
                    {
                        throw new Exception("Failed to set table export target to CSV");
                    }
                    SoapModelClient.export_to_tables(Path.Combine(GetDataPath(), "exported_model"));
                }
                incrementProgress();
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            finally
            {
                DataLogger.AddText("Deleting files");
                DirectoryInfo d = new DirectoryInfo(GetDataPath());
                FileInfo[] files = d.GetFiles("exported_model.*");
                foreach (FileInfo file in files)
                {
                    file.Delete();
                }
                d = new DirectoryInfo(Path.Combine(GetDataPath(), "exported_model"));
                if (d.Exists)
                {
                    d.Delete(true);
                }
                DataLogger.ResetProgressBar();
            }

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }
    }
}
