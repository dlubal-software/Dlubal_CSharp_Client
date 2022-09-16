#if RFEM

using Dlubal.WS.Rfem6.Model;
using System;
using static Dlubal.WS.Common.Tools.DataLogger;

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
        public static bool Test_Punching⁀Reinforcement_Get()
        {
            DataLogger.AddLogStart("Reading punching reinforcement...");
            try
            {
                ReadPunchingReinforcement();
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            DataLogger.ResetProgressBar();
            DataLogger.AddLogEnd(LogResultType.DONE);
            return true;
        }

        public static bool Test_Punching⁀Reinforcement_Set()
        {
            DataLogger.AddLogStart("Creating punching reinforcement...");
            DataLogger.InitializeProgressBar();
            bool result = Test_General_Delete⁀All();
            if (!result || (SoapModelClient == null))
            {
                DataLogger.AddLogEnd(LogResultType.FAILED);
                return false;
            }

            DataLogger.SetProgressBarValue(30);
            try
            {
                CreatePunchingReinforcement();
                DataLogger.ResetProgressBar();
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                DataLogger.AddLogEnd(LogResultType.FAILED);
                return false;
            }
            DataLogger.AddLogEnd(LogResultType.DONE);
            return true;
        }

        public static bool Test_Punching⁀Reinforcement_Delete()
        {
            DataLogger.AddLogStart("Deleting punching reinforcement...");
            try
            {
                bool result = DeleteObjects(object_types.E_OBJECT_TYPE_PUNCHING_REINFORCEMENT, 0, "Punching reinforcement");
                DataLogger.AddLogEnd(LogResultType.DONE);
                return result;
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
        }

        private static void ReadPunchingReinforcement()
        {
            int count = SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_PUNCHING_REINFORCEMENT, 0);
            int[] numbers = SoapModelClient.get_all_object_numbers(object_types.E_OBJECT_TYPE_PUNCHING_REINFORCEMENT, 0);

            if (count != numbers.Length)
            {
                throw new Exception("Object count does not match object number count.");
            }

            DataLogger.AddText($"{numbers.Length} Punching reinforcements have been read.");
            DataLogger.InitializeProgressBar(0, numbers.Length, 0);

            for (int i = 0; i < count; i++)
            {
                punching_reinforcement reinforcement = SoapModelClient.get_punching_reinforcement(numbers[i]);
                DataLogger.IncrementOffset();
                LogPunchingReinforcement(reinforcement);
                DataLogger.DecrementOffset();
                DataLogger.SetProgressBarValue(i);
            }
        }

        private static void LogPunchingReinforcement(punching_reinforcement reinforcement)
        {
            DataLogger.AddText($"No: {reinforcement.no}");
            DataLogger.IncrementOffset();
            DataLogger.AddText($"Name: {reinforcement.name}");
            DataLogger.AddText($"Absolute spacing between perimeters: {reinforcement.absolute_spacing_between_perimeters}");
            DataLogger.AddText($"Absolute spacing between perimeters automatically enabled: {reinforcement.absolute_spacing_between_perimeters_set_automatically_enabled}");
            DataLogger.AddText($"Bend up diameter: {reinforcement.bend_up_diameter}");
            DataLogger.DecrementOffset();
        }

        private static void CreatePunchingReinforcement()
        {
            SoapModelClient.begin_modification(nameof(CreatePunchingReinforcement));
            DataLogger.SetProgressBarValue(60);
            DataLogger.AddText("Generating punching reinforcement...");
            material material = new material()
            {
                no = 1,
                name = "Grade 50",
            };
            SoapModelClient.set_material(material);

            var reinforcement = GetPunchingReinforcement();
            SoapModelClient.set_punching_reinforcement(reinforcement);
            DataLogger.AddText("Generated punching reinforcement");
            SoapModelClient.finish_modification();
            DataLogger.SetProgressBarValue(100);
        }

        private static punching_reinforcement GetPunchingReinforcement()
        {
            var reinforcement = new punching_reinforcement()
            {
                no = 1,
                name = "Generated",
                absolute_spacing_between_perimeters = 1,
                absolute_spacing_between_perimetersSpecified = true,
                absolute_spacing_between_perimeters_set_automatically_enabled = true,
                absolute_spacing_between_perimeters_set_automatically_enabledSpecified = true,
                bend_up_diameter = 1,
                bend_up_diameterSpecified = true,
                material = 1,
                materialSpecified = true,
            };
            return reinforcement;
        }
    }
}

#endif // RFEM
