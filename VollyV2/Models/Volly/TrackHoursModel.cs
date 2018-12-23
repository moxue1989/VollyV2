using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV2.Models.Volly
{
    public class TrackHoursModel
    {
        [Required]
        [EmailAddress]
        [DisplayName("Your Email Address (the one you registered with)")]
        public string Email { get; set; }
        [Required]
        [DisplayName("What did you do? (Example: Art with Seniors, Community Clean up, Computer tutoring)")]
        public string OpportunityName { get; set; }
        [Required]
        [DisplayName("Who did you volunteer with? (Example: Art with Seniors, Community Clean up, Computer tutoring)")]
        public string OrganizationName { get; set; }
        [Required]
        [DisplayName("Date you volunteered")]
        public DateTime Date { get; set; }
        [Required]
        [DisplayName("How long did you volunteer? (Enter number of hours)")]
        public double Hours { get; set; }
    }
}
