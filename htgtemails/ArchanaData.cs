using System.Collections.Generic;
using HTGT.Data;
using HTGT.Data.Models;

namespace htgtemails
{
    class ArchanaData
    {
        internal static List<KidsInformationEmailModel> GetEmailReminders()
        {
            using (var da = new DAArchanaInformation())
            {
                return da.GetArchanaEmailReminders();
            }
        }

        internal static void UpdateReminderDate(int kidid, string toEmail)
        {
            using (var da = new DAArchanaInformation(true))
            {
                da.UpdateReminderDate(kidid, toEmail);
            }
        }
    }
}
