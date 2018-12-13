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
        public string Email { get; set; }
        [Required]
        [DisplayName("What did you do?")]
        public string OpportunityName { get; set; }
        [Required]
        [DisplayName("Who did you volunteer with?")]
        public string OrganizationName { get; set; }
        [Required]
        [DisplayName("Volunteer Date")]
        public DateTime Date { get; set; }
        [Required]
        [DisplayName("How long did you volunteer?")]
        public double Hours { get; set; }
    }
}
