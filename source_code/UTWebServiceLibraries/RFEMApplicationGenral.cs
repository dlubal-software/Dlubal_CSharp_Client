using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using System;

#if RFEM
using Dlubal.WS.Rfem6.Application;
using ApplicationClient = Dlubal.WS.Rfem6.Application.RfemApplicationClient;
using Dlubal.WS.Rfem6.Model;
using ModelClient = Dlubal.WS.Rfem6.Model.RfemModelClient;
#elif RSTAB
using Dlubal.WS.Rstab9.Application;
using ApplicationClient = Dlubal.WS.Rstab9.Application.RstabApplicationClient;
using Dlubal.WS.Rstab9.Model;
using ModelClient = Dlubal.WS.Rstab9.Model.RstabModelClient;
#elif RSECTION
using Dlubal.WS.RSection1.Application;
using ApplicationClient = Dlubal.WS.RSection1.Application.RSectionApplicationClient;
using Dlubal.WS.RSection1.Model;
using ModelClient = Dlubal.WS.RSection1.Model.RSectionModelClient;
#endif

namespace UTWebServiceLibraries
{
    [TestClass]
    public class RFEMApplicationGeneral
    {

        public static EndpointAddress Address { get; set; } = new EndpointAddress("http://localhost:8081");


        private static BasicHttpBinding Binding
        {
            get
            {
                BasicHttpBinding binding = new BasicHttpBinding
                {
                    // Send timeout is set to 180 seconds.
                    SendTimeout = new TimeSpan(0, 0, 180),
                    UseDefaultWebProxy = true,
                };

                return binding;
            }
        }

        //private static RfemApplicationClient application = null;
        private static ApplicationClient application = null;

        [TestMethod]
        public void GetRFEMInfo()
        {
            try
            {
                application_information ApplicationInfo;
                try
                {
                    // connects to RFEM6 or RSTAB9 application
                    application = new ApplicationClient(Binding, Address);

                }
                catch (Exception exception)
                {
                    if (application != null)
                    {
                        if (application.State != CommunicationState.Faulted)
                        {
                            application.Close();
                        }
                        else
                        {
                            application.Abort();
                        }

                        application = null;
                    }
                }
                finally
                {
                    ApplicationInfo = application.get_information();
                }
                Assert.IsNotNull(ApplicationInfo, "ApplicationInfo is null. It seems to be that UT is runned in different application");
                Assert.AreEqual(ApplicationInfo.type.ToString(), "RFEM6", "Application type is incorrect");
                Assert.AreEqual(ApplicationInfo.language_name, "English", "Application language is incorrect");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }

        }
    }
}