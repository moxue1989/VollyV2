using System.Collections.Generic;
using System.Threading.Tasks;
using VollyV2.Data.Volly;
using VollyV2.Models.Volly;

namespace VollyV2.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendEmailsAsync(List<string> emailList, string subject, string message);
        Task SendApplicationConfirmAsync(ApplicationView application);
        Task SendRemindersAsync(List<string> emailList, Occurrence occurrence);
        Task SendAccountCreatedConfirm(string email, string password);
    }
}
