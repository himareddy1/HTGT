using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using htgt.Business;

namespace htgt.Controllers
{
    public class ReminderEmailsController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReminderEmailsController));
        // GET: api/ReminderEmails
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ReminderEmails/5
        public string Get(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (IsRequestValid(id))
                {
                    SendReminders();
                }
            }
            return "Sent!!!";
        }

        private void SendReminders()
        {
            ReminderEmailsHelper.SendReminder();
        }

        private bool IsRequestValid(string value)
        {
            return value.Equals("F1E22704-69E3-49AF-8896-D145F55169C7", StringComparison.OrdinalIgnoreCase);
        }
    }
}
