using HTGT.Data;
using HTGT.Data.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web.Hosting;

namespace htgt.Business
{
    internal class ReminderEmailsHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReminderEmailsHelper));
        internal static void SendReminder()
        {
            log.Info("Starting Email Service");
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                log.Info("Getting Email Remainders");

                sw.Start();
                var reminders = GetEmailReminders();
                sw.Stop();

                log.Info("Email remainders retrieved in " + sw.ElapsedMilliseconds + "(ms)");
                if (reminders.Count > 0)
                {
                    sw.Restart();
                    foreach (var reminder in reminders)
                    {
                        string message = GetHTMLMessageBody(reminder);
                        string plainTextMessage = GetPlainTextMessage(reminder);
                        //log.Info(message);

                        log.Info("Sending email remainder to " + reminder.Email);
                        SendEmailRemainders(reminder.Email, message, plainTextMessage);
                        log.Info("Email remainder sent");

                        log.Info("Updating database with email sent date");
                        UpdateReminderDate(reminder.SID, reminder.Email);
                        log.Info("Update Completed");
                    }
                    sw.Stop();
                    log.Info("Email remainders sent in " + sw.ElapsedMilliseconds + "(ms)");
                }
                else
                {
                    log.Info("No Data Found!!!");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
        }

        private static void SendEmailRemainders(string toemail, string body, string plainTextMessage)
        {
            new EmailHelpers().SendReminderEmail(toemail, body, plainTextMessage);
        }

        private static string GetHTMLMessageBody(KidsInformationEmailModel remainder)
        {
            var message = File.ReadAllText(HostingEnvironment.MapPath("~/emailtemplate.html"));
            message = message.Replace("vchildname", remainder.KName);
            message = message.Replace("vparentname", remainder.ParentsName);

            var dob = new DateTime(DateTime.Now.Year, remainder.MonthofBirth, remainder.DayofBirth);
            message = message.Replace("vbirthdate", dob.ToString("dd-MMM"));

            return message;
        }

        private static string GetPlainTextMessage(KidsInformationEmailModel reminder)
        {
            var message = File.ReadAllText(HostingEnvironment.MapPath("~/PlainTextEmail.txt"));
            message = message.Replace("{vchildname}", reminder.KName);
            message = message.Replace("{vparentname}", reminder.ParentsName);

            var dob = new DateTime(DateTime.Now.Year, reminder.MonthofBirth, reminder.DayofBirth);
            message = message.Replace("{vbirthdate}", dob.ToString("dd-MMM"));

            return message;
        }

        private static List<KidsInformationEmailModel> GetEmailReminders()
        {
            using (var da = new DAArchanaInformation())
            {
                return da.GetArchanaEmailReminders();
            }
        }

        private static void UpdateReminderDate(int kidid, string toEmail)
        {
            using (var da = new DAArchanaInformation(true))
            {
                da.UpdateReminderDate(kidid, toEmail);
            }
        }
    }
}