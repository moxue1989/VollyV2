using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using VollyV2.Controllers;
using VollyV2.Data.Volly;

namespace VollyV2.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private static readonly string FromEmail = Environment.GetEnvironmentVariable("email_address");
        private static readonly string SendgridApiKey = Environment.GetEnvironmentVariable("sendgrid_api");

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            await SendEmailsAsync(new List<string> { email }, subject, message);
        }

        public async Task SendEmailsAsync(List<string> emailList, string subject, string message)
        {
            var client = new SendGridClient(SendgridApiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(FromEmail, "Volly Team"),
                Subject = subject,
                HtmlContent = message,
                PlainTextContent = message
            };

            foreach (string email in emailList)
            {
                msg.AddTo(new EmailAddress(email));
            }
            await client.SendEmailAsync(msg);
        }

        public async Task SendApplicationConfirmAsync(Application application)
        {
            var client = new SendGridClient(SendgridApiKey);

            SendGridMessage sendGridMessage = new SendGridMessage()
            {
                From = new EmailAddress(FromEmail, "Volly Team"),
                Subject = "Application For: " + application.Opportunity.Name,
                TemplateId = "e9713a19-2a9e-4d0f-8994-088633aaab25",
                HtmlContent = application.GetEmailMessage(),
                PlainTextContent = application.GetEmailMessage()
            };
            sendGridMessage.AddTo(new EmailAddress(application.Email, application.Name));
            sendGridMessage.AddCc(new EmailAddress(VollyConstants.AliceEmail, "Alice"));

            List<string> occurrenceStrings = application.Occurrences
                .Select(ao => ToOccurrenceTimeString().Invoke(ao.Occurrence))
                .ToList();

            sendGridMessage.AddSubstitution(":time", string.Join("<br/>", occurrenceStrings));
            sendGridMessage.AddSubstitution(":description", application.Opportunity.Description);
            sendGridMessage.AddSubstitution(":address", application.Opportunity.Address);
            sendGridMessage.AddSubstitution(":name", application.Opportunity.Name);
            sendGridMessage.AddSubstitution(":image", application.Opportunity.ImageUrl);

            await client.SendEmailAsync(sendGridMessage);
        }

        private static Func<Occurrence, string> ToOccurrenceTimeString()
        {
            return o => o.StartTime.ToShortDateString() + " " + o.StartTime.ToShortTimeString() +
            " --> " + o.EndTime.ToShortTimeString();
        }
    }
}
