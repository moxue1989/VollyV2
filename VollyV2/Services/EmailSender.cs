using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

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
                HtmlContent = message + "<strong>Contact Us!</strong>"
            };

            foreach (string email in emailList)
            {
                msg.AddTo(new EmailAddress(email));
            }
            await client.SendEmailAsync(msg);
        }
    }
}
