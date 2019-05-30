using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace htgtemails
{
    class EmailHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EmailHelper));
        private readonly string _fromEmail, _fromEmailName, _subject;
        private readonly List<string> _additionalToEmails, _ccEmails;
        private readonly SendGridClient _client;

        public EmailHelper()
        {
            var apiKey = GetConfigValue("sendGridApiKey");
            _subject = GetConfigValue("emailSubject");
            _fromEmail = GetConfigValue("fromEmail");
            _fromEmailName = GetConfigValue("fromEmailName");
            _additionalToEmails = GetEmailsList(GetConfigValue("additionalToEmail"));
            _ccEmails = GetEmailsList(GetConfigValue("ccEmails"));
            _client = new SendGridClient(apiKey);
        }

        private List<string> GetEmailsList(string emails)
        {
            if (!string.IsNullOrWhiteSpace(emails))
                return emails.Contains(",") ? emails.Split(',').ToList() : new List<string> { emails };
            else
                return new List<string>();
        }

        private string GetConfigValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        internal async Task SendEmail(string toemail, string body, string plainTextMessage)
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

                var toEmailsList = _additionalToEmails.Select(toEmail => new EmailAddress(toEmail)).ToList();
                toEmailsList.Add(new EmailAddress(toemail));
                msg.AddTos(toEmailsList);

                var ccEmailsList = _ccEmails.Select(x => new EmailAddress(x)).ToList();
                msg.AddCcs(ccEmailsList);

                var response = await _client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
        }
    }
}
