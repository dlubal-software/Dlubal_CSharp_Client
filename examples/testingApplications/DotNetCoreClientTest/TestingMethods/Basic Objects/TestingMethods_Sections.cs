using Dlubal.WS.RfemRstabSoapWsCoreLib.Tools;
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
        public static bool Test_Basic\u2040Objects_Sections_Create()
        {
            const int MATERIAL_NO = 1;
            const int SECTION_NO = 2;

            DataLogger.AddLogStart("Creating section...");
            DataLogger.InitializeProgressBar(0, 30, 0);

            bool result = Test_General_Delete⁀All();
            if (!result || (SoapModelClient == null))
            {
                DataLogger.AddLogEnd(DataLogger.LogResultType.FAILED);
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
                DataLogger.AddText($"Set material No.{SECTION_NO}");
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
    }
}
