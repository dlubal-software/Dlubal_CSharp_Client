using Dlubal.WS.Common.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if RFEM
using Dlubal.WS.Rfem6.Model;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
#endif


namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
        public static bool Test_Results_Parts\u2040List()
        {
            DataLogger.AddLogStart("Getting part list...");
            DataLogger.InitializeProgressBar(0, 8, 0);

            try
            {
                SoapModelClient.generate_parts_lists();

                DataLogger.SetProgressBarValue(1);

                parts_list_members_by_material_row[] members = SoapModelClient.get_parts_list_members_by_material();

                DataLogger.AddText($"Members by material: {members.Length}");
                DataLogger.SetProgressBarValue(2);

                parts_list_member_representatives_by_material_row[] memberRepresentatives = SoapModelClient.get_parts_list_member_representatives_by_material();

                DataLogger.AddText($"Member representatives by material: {memberRepresentatives.Length}");
                DataLogger.SetProgressBarValue(3);

                parts_list_member_sets_by_material_row[] memberSets = SoapModelClient.get_parts_list_member_sets_by_material();

                DataLogger.AddText($"Member sets by material: {memberSets.Length}");
                DataLogger.SetProgressBarValue(4);

                parts_list_member_set_representatives_by_material_row[] memberSetRepresentatives = SoapModelClient.get_parts_list_member_set_representatives_by_material();

                DataLogger.AddText($"Member set representatives by material: {memberSetRepresentatives.Length}");
                DataLogger.SetProgressBarValue(5);

#if RFEM
                parts_list_surfaces_by_material_row[] surfaces = SoapModelClient.get_parts_list_surfaces_by_material();

                DataLogger.AddText($"Surfaces by material: {surfaces.Length}");
                DataLogger.SetProgressBarValue(6);

                parts_list_solids_by_material_row[] solids = SoapModelClient.get_parts_list_solids_by_material();

                DataLogger.AddText($"Solids by material: {solids.Length}");
                DataLogger.SetProgressBarValue(7);
#endif

                parts_list_all_by_material_row[] all = SoapModelClient.get_parts_list_all_by_material();

                DataLogger.AddText($"All by material: {all.Length}");
                DataLogger.SetProgressBarValue(8);
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            finally
            {
                DataLogger.ResetProgressBar();
            }

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }
    }
}
