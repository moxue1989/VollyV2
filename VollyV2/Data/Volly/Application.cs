using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VollyV2.Models;

namespace VollyV2.Data.Volly
{
    public class Application
    {
        public int Id { get; set; }
        public Opportunity Opportunity { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string GetEmailMessage()
        {
            return "<p>Applicant Name: " + Name + "<p/>" +
                   "<p>Applicant Email: " + Email + "<p/>" +
                   "<p>Message: " + Message + "<p/>";
        }
    }
}
