using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VollyV2.Data.Volly;

namespace VollyV2.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendEmailsAsync(List<string> emailList, string subject, string message);
        Task SendApplicationConfirmAsync(Application application);
    }
}
