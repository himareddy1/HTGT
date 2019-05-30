using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTGT.Data
{
    public class EmailRemainders
    {
        public static List<KidsInfo> GetEmailRemainders()
        {
            htgtEntities db = new htgtEntities();
            var query_results = (from items in db.KidsInformations
                                where items.NextEmailRemainder.Date == DateTime.Today.Date && items.Active == true
                                select new KidsInfo
                                {
                                    ChildName = items.Firstname + " " + items.LastName,
                                    FathersName = items.FatherFirstName + " " + items.FatherLastName,
                                    MothersName = items.MotherFirstName + " " + items.MotherLastName,
                                    Gothram = items.Gothram,
                                    Nakshatram = items.Nakshatram,
                                    EmailAddress = items.EmailAddress
                                }).ToList();

            return query_results;
        }

        public bool HasEmailRemainders { get; set; }
    }

    public class KidsInfo
    {
        public string ChildName { get; set; }
        public string FathersName { get; set; }
        public string MothersName { get; set; }
        public string Gothram { get; set; }
        public string Nakshatram { get; set; }
        public string EmailAddress { get; set; }
    }
}
