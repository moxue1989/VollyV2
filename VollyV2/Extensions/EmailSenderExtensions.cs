using System.Text.Encodings.Web;
using System.Threading.Tasks;
using VollyV2.Services;

namespace VollyV2.Extensions
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by clicking this link: {link}");
        }
    }
}
