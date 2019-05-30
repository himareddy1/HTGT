using System;
using System.Configuration;
using System.Threading.Tasks;
using log4net;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.Linq;

namespace htgt.Business
{
    public class EmailHelpers
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EmailHelpers));
        private readonly string _fromEmail, _fromEmailName;
        private string _subject;
        private readonly SendGridClient _client;

        public EmailHelpers()
        {
            var apiKey = GetConfigValue("sendGridApiKey");
            _fromEmail = GetConfigValue("fromEmail");
            _fromEmailName = GetConfigValue("fromEmailName");
            _client = new SendGridClient(apiKey);
        }

        private string GetConfigValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public void SendPasswordResetEmail(string emailAddress, string callbackUrl)
        {
            string emailBody = $"To change the password, please click on the button below or copy and paste the URL into your browser. {callbackUrl}";
            _subject = "[HTGT] Password Reset!!!";
            SendEmail(emailAddress, emailBody).Wait(5000);
        }

        private async Task SendEmail(string toAddress, string emailBody)
        {
            try
            {
                var msg = new SendGridMessage()
                {
                    From = new EmailAddress(_fromEmail, _fromEmailName),
                    Subject = _subject,
                    HtmlContent = emailBody,
                    PlainTextContent = emailBody
                };

                msg.AddTo(new EmailAddress(toAddress));
                var response = await _client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
        }

        internal void SendReminderEmail(string toemail, string body, string plainTextMessage)
        {
            var additionalToEmails = GetEmailsList(GetConfigValue("additionalToEmail"));
            var ccEmails = GetEmailsList(GetConfigValue("ccEmails"));
            var bccEmails = GetEmailsList(GetConfigValue("bccEmails"));
            _subject = "HTGT - Birthday Archana Remainder!!!";
            SendEmail(toemail, additionalToEmails, ccEmails, bccEmails, body, plainTextMessage).Wait(5000);
        }

        private List<string> GetEmailsList(string emails)
        {
            if (!string.IsNullOrWhiteSpace(emails))
                return emails.Contains(",") ? emails.Split(',').ToList() : new List<string> { emails };
            else
                return new List<string>();
        }

        private async Task SendEmail(string toemail, List<string> additionalToEmails, List<string> ccEmails, List<string> bccEmails, string body, string plainTextMessage)
        {
            try
            {
                var msg = new SendGridMessage()
                {
                    From = new EmailAddress(_fromEmail, _fromEmailName),
                    Subject = _subject,
                    HtmlContent = body,
                    PlainTextContent = plainTextMessage
                };

                var toEmailsList = additionalToEmails.Select(toEmail => new EmailAddress(toEmail)).ToList();
                toEmailsList.Add(new EmailAddress(toemail));
                msg.AddTos(toEmailsList);

                var ccEmailsList = ccEmails.Select(x => new EmailAddress(x)).ToList();
                msg.AddCcs(ccEmailsList);

                var bccEmailsList = bccEmails.Select(x => new EmailAddress(x)).ToList();
                msg.AddBccs(bccEmailsList);

                var response = await _client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
        }
    }
}