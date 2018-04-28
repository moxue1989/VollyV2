using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV2.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendEmailsAsync(List<string> emailList, string subject, string message);
    }
}
