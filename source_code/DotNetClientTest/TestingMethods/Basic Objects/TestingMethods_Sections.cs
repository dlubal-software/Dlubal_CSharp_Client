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
    }
}

#endif // RFEM RSTAB