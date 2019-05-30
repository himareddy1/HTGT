using System;
using System.Diagnostics;
using System.IO;
using HTGT.Data.Models;
using log4net;

namespace htgtemails
{
    // To learn more about Microsoft Azure WebJobs, please see http://go.microsoft.com/fwlink/?LinkID=401557
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main()
        {
            log.Info("Starting Email Service");
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
               log.Info("Getting Email Remainders");

                sw.Start();
                var reminders = ArchanaData.GetEmailReminders();
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
                        ArchanaData.UpdateReminderDate(reminder.SID, reminder.Email);
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
            new EmailHelper().SendEmail(toemail, body, plainTextMessage).Wait(5000);
        }

        private static string GetHTMLMessageBody(KidsInformationEmailModel remainder)
        {
            var message =  File.ReadAllText(@"emailtemplate.html");
            message = message.Replace("vchildname", remainder.KName);
            message = message.Replace("vparentname", remainder.ParentsName);
            
            var dob = new DateTime(DateTime.Now.Year, remainder.MonthofBirth, remainder.DayofBirth);
            message = message.Replace("vbirthdate", dob.ToString("dd-MMM"));
            
            return message;
        }

        private static string GetPlainTextMessage(KidsInformationEmailModel reminder)
        {
            var message = File.ReadAllText(@"PlainTextEmail.txt");
            message = message.Replace("{vchildname}", reminder.KName);
            message = message.Replace("{vparentname}", reminder.ParentsName);

            var dob = new DateTime(DateTime.Now.Year, reminder.MonthofBirth, reminder.DayofBirth);
            message = message.Replace("{vbirthdate}", dob.ToString("dd-MMM"));

            return message;
        }
    }
}
