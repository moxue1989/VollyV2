using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VollyV2.Models;

namespace VollyV2.Data.Volly
{
    public class VolunteerHours
    {
        public int VolunteerHoursId { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string OrganizationName { get; set; }
        public DateTime DateTime { get; set; }
        public int OccurrenceId { get; set; }
        public Occurrence Occurrence { get; set; }
        public double Hours { get; set; }
    }
}
