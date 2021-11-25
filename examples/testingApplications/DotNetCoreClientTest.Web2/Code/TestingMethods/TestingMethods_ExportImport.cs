using Dlubal.WS.RfemRstabSoapWsCoreLib.Tools;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using DotNetCoreClientTest.Web2;

#if RFEM
using Dlubal.WS.Rfem6.Model;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
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

            using (XmlTextWriter writer = new XmlTextWriter(Path.Combine(GetDataPath(), MyConfig.ObjectInformationFile), Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                document.WriteTo(writer);
                writer.Close();
            };

            DataLogger.ResetProgressBar();
            DataLogger.AddLogEnd(succeeded ? DataLogger.LogResultType.DONE : DataLogger.LogResultType.FAILED);
            return succeeded;
        }

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
    }
}
