using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using VollyV2.Controllers;
using VollyV2.Data.Volly;
using VollyV2.Models.Views;
using VollyV2.Models.Volly;

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

        public async Task SendApplicationConfirmAsync(ApplicationView application)
        {
            var client = new SendGridClient(SendgridApiKey);

            SendGridMessage sendGridMessage = new SendGridMessage()
            {
                From = new EmailAddress(FromEmail, "Volly Team"),
                Subject = "Application For: " + application.OpportunityName,
                TemplateId = "e9713a19-2a9e-4d0f-8994-088633aaab25",
                HtmlContent = application.GetEmailMessage(),
                PlainTextContent = application.GetEmailMessage()
            };
            sendGridMessage.AddTo(new EmailAddress(application.Email, application.Name));
            sendGridMessage.AddCc(new EmailAddress(VollyConstants.AliceEmail, "Alice"));

            List<string> occurrenceStrings = application.OccurrenceViews
                .Select(o => ToOccurrenceTimeString().Invoke(o))
                .ToList();

            sendGridMessage.AddSubstitution(":time", string.Join("<br/>", occurrenceStrings));
            sendGridMessage.AddSubstitution(":description", application.OpportunityDescription);
            sendGridMessage.AddSubstitution(":address", application.OpportunityAddress);
            sendGridMessage.AddSubstitution(":name", application.OpportunityName);
            sendGridMessage.AddSubstitution(":image", application.OpportunityImageUrl);

            await client.SendEmailAsync(sendGridMessage);
        }

        private static Func<OccurrenceView, string> ToOccurrenceTimeString()
        {
            return o => o.StartTime.ToShortDateString() + " " + o.StartTime.ToShortTimeString() +
            " --> " + o.EndTime.ToShortTimeString();
        }
    }
}
