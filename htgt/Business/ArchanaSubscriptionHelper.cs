using System.Collections.Generic;
using HTGT.Data;
using HTGT.Data.Models;

namespace htgt.Business
{
    class ArchanaSubscriptionHelper
    {
        internal static List<KidsInformationIndexViewModel> GetArchanaList()
        {
            using (var da = new DAArchanaInformation())
            {
                return da.GetArchanaList();
            }
        }

        internal static void CreateSubscription(KidsInformationCreateViewModel kidsInformation)
        {
            int day = kidsInformation.DateOfBirth.Day;
            int month = kidsInformation.DateOfBirth.Month;

            using (var da = new DAArchanaInformation(true))
            {
                da.CreateSubscription(kidsInformation.Name, kidsInformation.ParentName, kidsInformation.EmailAddress, day, month);
            }
        }

        internal static KidsInformationEditViewModel GetKidsArchanaInformation(int id)
        {
            using (var da = new DAArchanaInformation())
            {
                return da.GetArchanaInfo(id);
            }
        }

        internal static void UpdateSubscription(KidsInformationEditViewModel kidsInformation)
        {
            int day = kidsInformation.DateOfBirth.Day;
            int month = kidsInformation.DateOfBirth.Month;

            using (var da = new DAArchanaInformation(true))
            {
                da.UpdateSubscription(kidsInformation.SID, kidsInformation.KName, kidsInformation.ParentsName, kidsInformation.EmailAddress, day, month, kidsInformation.IsActive);
            }
        }
    }
}