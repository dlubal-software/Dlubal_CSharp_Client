using System.ServiceModel;
using Dlubal.WS.Rfem6.Application;
using ApplicationClient = Dlubal.WS.Rfem6.Application.RfemApplicationClient;
using Dlubal.WS.Rfem6.Model;
using ModelClient = Dlubal.WS.Rfem6.Model.RfemModelClient;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

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