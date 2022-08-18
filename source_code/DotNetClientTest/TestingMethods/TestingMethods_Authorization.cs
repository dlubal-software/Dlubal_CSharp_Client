using System;
using Dlubal.WS.Clients.DotNetClientTest.Tools;
using Dlubal.WS.Common.Tools;

#if RFEM
using Dlubal.WS.Rfem6.Model;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
        public static bool Test_Authorization_Set\u2040Objects\u2040Under\u2040Demo\u2040Limit()
        {
            DataLogger.AddLogStart("Set members under demo limit...");

            if (!InitializeTest())
            {
                return false;
            }

            DataLogger.SetProgressBarValue(0);

            try
            {
                SoapModelClient.begin_modification("set member imperfections");

                // set members
                for (int i = 0; i < 5; i++)
                {
                    SetDefaultMember(i + 1, i * 1);
                }
#if RFEM
                // set surface
                node[] nodes = new node[]
                {
                    new node { no = 101, coordinates = new vector_3d().SetCoordinates(1.0, -1.0, 0.0) },
                    new node { no = 102, coordinates = new vector_3d().SetCoordinates(5.0, -1.0, 0.0) },
                    new node { no = 103, coordinates = new vector_3d().SetCoordinates(5.0, -4.0, -1.0) },
                    new node { no = 1044, coordinates = new vector_3d().SetCoordinates(1.0, -4.0, -1.0) }
                };
                SetDefaultSurface( nodes, 1);
#endif

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

        public static bool Test_Authorization_Set\u2040Objects\u2040Over\u2040Demo\u2040Limit()
        {
            DataLogger.AddLogStart("Set members over demo limit...");

            if (!InitializeTest())
            {
                return false;
            }

            DataLogger.SetProgressBarValue(0);

            try
            {
                SoapModelClient.begin_modification("set member imperfections");

                // set members
                for (int i = 0; i < 13; i++)
                {
                    SetDefaultMember(i + 1, i * 1);
                }
#if RFEM
                // set surface
                node[] nodes = new node[]
                {
                    new node { no = 101, coordinates = new vector_3d().SetCoordinates(0.0, -1.0, 0.0) },
                    new node { no = 102, coordinates = new vector_3d().SetCoordinates(4.0, -1.0, 0.0) },
                    new node { no = 103, coordinates = new vector_3d().SetCoordinates(4.0, -4.0, -1.0) },
                    new node { no = 104, coordinates = new vector_3d().SetCoordinates(0.0, -4.0, -1.0) }
                };
                SetDefaultSurface(nodes, 1);

                nodes = new node[]
                {
                    new node { no = 105, coordinates = new vector_3d().SetCoordinates(5.0, -1.0, 0.0) },
                    new node { no = 106, coordinates = new vector_3d().SetCoordinates(9.0, -1.0, 0.0) },
                    new node { no = 107, coordinates = new vector_3d().SetCoordinates(9.0, -4.0, -1.0) },
                    new node { no = 108, coordinates = new vector_3d().SetCoordinates(5.0, -4.0, -1.0) }
                };
                SetDefaultSurface(nodes, 2);

                nodes = new node[]
                {
                    new node { no = 109, coordinates = new vector_3d().SetCoordinates(10.0, -1.0, 0.0) },
                    new node { no = 110, coordinates = new vector_3d().SetCoordinates(14.0, -1.0, 0.0) },
                    new node { no = 111, coordinates = new vector_3d().SetCoordinates(14.0, -4.0, -1.0) },
                    new node { no = 112, coordinates = new vector_3d().SetCoordinates(10.0, -4.0, -1.0) }
                };
                SetDefaultSurface(nodes, 3);
#endif

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
