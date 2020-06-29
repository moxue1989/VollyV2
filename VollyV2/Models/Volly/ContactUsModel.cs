using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV2.Models.Volly
{
    public class ContactUsModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Message { get; set; }
        public bool TripCheck { get; set; }

        public string GetEmailMessage(string ipAddress)
        {
            return "<p>Contact Name: " + Name + "</p>" +
                   "<p>Contact Email: " + Email + "</p>" +
                   "<p>Message: " + Message + "</p>" +
                   "<p>From IP: " + ipAddress + "</p>";
        }
    }
}
