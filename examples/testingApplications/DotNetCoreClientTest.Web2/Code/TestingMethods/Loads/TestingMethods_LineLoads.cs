#if RFEM

using Dlubal.WS.Rfem6.Model;
using Dlubal.WS.RfemRstabSoapWsCoreLib.Tools;
using System;
using static Dlubal.WS.RfemRstabSoapWsCoreLib.Tools.DataLogger;

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
        /// <summary>
        /// Creates line load.
        /// </summary>
        /// <param name="caseNo">Load case number.</param>
        /// <param name="objectNo">Object number used for load and line.</param>
        /// <param name="type">Type of line load.</param>
        /// <param name="distribution">Distribution of line load.</param>
        /// <param name="tableRowCount">Count of rows in the varying load table.</param>
        private static void SetLineLoad(int caseNo, int objectNo, line_load_load_type type, line_load_load_distribution distribution, line_load_load_direction direction = line_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_TRUE, int tableRowCount = 0)
        {
            DataLogger.AddText($"Set line load No. {objectNo} in load case No.{caseNo}");

            // line load
            line_load line_Load = new line_load
            {
                no = objectNo,
                load_type = type,
                load_typeSpecified = true,
                load_distribution = distribution,
                load_distributionSpecified = true,
                load_direction = direction,
                load_directionSpecified = true,
                lines = new int[] { objectNo },
                magnitude = objectNo * 1000
            };

            // varying parameters for line load
            if (distribution == line_load_load_distribution.LOAD_DISTRIBUTION_CONCENTRATED_VARYING || distribution == line_load_load_distribution.LOAD_DISTRIBUTION_VARYING)
            {
                line_Load.varying_load_parameters = new line_load_varying_load_parameters[tableRowCount];

                for (int i = 0; i < tableRowCount; i++)
                {
                    line_Load.varying_load_parameters[i] = new line_load_varying_load_parameters
                    {
                        no = i + 1,
                        noSpecified = true,
                        distance = i * 1.1,
                        distanceSpecified = true,
                        delta_distance = i == 0 ? i * 0.1 : 1 + (i * 0.1),
                        delta_distanceSpecified = true,
                        magnitude = (i + 1) * 1000,
                        magnitudeSpecified = true,
                        note = $"Note {i + 1}"
                    };
                }
            }

            SoapModelClient.set_line_load(caseNo, line_Load);
        }

        /// <summary>
        /// Creates line loads.
        /// </summary>
        /// <param name="caseId">Load case id.</param>
        public static bool Test_Loads_Line\u2040Loads_Create()
        {
            const int OBJECT_1_NO = 1;
            const int OBJECT_2_NO = 2;
            const int OBJECT_3_NO = 3;
            const int OBJECT_4_NO = 4;
            const int LOAD_CASE_1_NO = 5;

            DataLogger.AddLogStart("Creating line loads...");
            DataLogger.InitializeProgressBar(0, 70, 0);

            bool result = Test_General_Delete⁀All();
            if (!result || (SoapModelClient == null))
            {
                DataLogger.AddLogEnd(LogResultType.FAILED);
                return false;
            }

            DataLogger.SetProgressBarValue(10);

            try
            {
                SoapModelClient.begin_modification("set line loads");

                // surface
                node[] nodes = new node[]
                {
                    new node { no = OBJECT_1_NO, coordinates = new vector_3d().SetCoordinates(1.0, -1.0, 0.0) },
                    new node { no = OBJECT_2_NO, coordinates = new vector_3d().SetCoordinates(5.0, -1.0, 0.0) },
                    new node { no = OBJECT_3_NO, coordinates = new vector_3d().SetCoordinates(5.0, -4.0, -1.0) },
                    new node { no = OBJECT_4_NO, coordinates = new vector_3d().SetCoordinates(1.0, -4.0, -1.0) }
                };
                SetDefaultSurface(nodes);

                DataLogger.SetProgressBarValue(20);

                SetLoadCase(LOAD_CASE_1_NO);

                DataLogger.SetProgressBarValue(30);

                SetLineLoad(LOAD_CASE_1_NO, OBJECT_1_NO, line_load_load_type.LOAD_TYPE_FORCE, line_load_load_distribution.LOAD_DISTRIBUTION_CONCENTRATED_VARYING, line_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_TRUE, 4);

                DataLogger.SetProgressBarValue(40);

                SetLineLoad(LOAD_CASE_1_NO, OBJECT_2_NO, line_load_load_type.LOAD_TYPE_FORCE, line_load_load_distribution.LOAD_DISTRIBUTION_VARYING, line_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_TRUE, 4);

                DataLogger.SetProgressBarValue(50);

                SetLineLoad(LOAD_CASE_1_NO, OBJECT_3_NO, line_load_load_type.LOAD_TYPE_MOMENT, line_load_load_distribution.LOAD_DISTRIBUTION_CONCENTRATED_VARYING, line_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_TRUE, 3);

                DataLogger.SetProgressBarValue(60);

                SetLineLoad(LOAD_CASE_1_NO, OBJECT_4_NO, line_load_load_type.LOAD_TYPE_MOMENT, line_load_load_distribution.LOAD_DISTRIBUTION_VARYING, line_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_TRUE, 3);

                DataLogger.SetProgressBarValue(70);
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

#endif // RFEM
