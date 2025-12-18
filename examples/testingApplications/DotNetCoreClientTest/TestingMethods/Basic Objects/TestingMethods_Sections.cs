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

            DataLogger.AddLogStart("Creating cross_section...");
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
                SoapModelClient.begin_modification("set cross_section");

                // create material
                DataLogger.AddText($"Set material No.{MATERIAL_NO}");
                material material = new material()
                {
                    no = MATERIAL_NO,
                    name = MATERIAL_STEEL,
                };
                SoapModelClient.set_material(material);

                DataLogger.SetProgressBarValue(20);

                // create cross_section
                DataLogger.AddText($"Set material No.{SECTION_NO}");
                cross_section cross_section = new cross_section
                {
                    no = SECTION_NO,
                    name = SECTION_RECTANGLE,
                    material = MATERIAL_NO,
                    materialSpecified = true
                };
                SoapModelClient.set_cross_section(cross_section);

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
