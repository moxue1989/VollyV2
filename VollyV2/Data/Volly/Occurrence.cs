using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VollyV2.Controllers;

namespace VollyV2.Data.Volly
{
    public class Occurrence
    {
        public int Id { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:ddd MMM d yyyy h:mm tt}")]
        public DateTime StartTime { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:ddd MMM d yyyy h:mm tt}")]
        public DateTime EndTime { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:ddd MMM d yyyy h:mm tt}")]
        public DateTime ApplicationDeadline { get; set; }
        [Required]
        public int Openings { get; set; }
        [JsonIgnore]
        [Required]
        public int OpportunityId { get; set; }
        [JsonIgnore]
        public Opportunity Opportunity { get; set; }
        [JsonIgnore]
        public List<ApplicationOccurrence> Applications { get; set; }

        public override string ToString()
        {
            return "Start: " + StartTime + " End: " + EndTime;
        }
    }

    public static class OccurrenceTimeZoneConverter
    {
        public static Func<Occurrence, Occurrence> ConvertFromUtc()
        {
            return delegate (Occurrence occurrence)
            {
                occurrence.StartTime = VollyConstants.ConvertFromUtc(occurrence.StartTime);
                occurrence.EndTime = VollyConstants.ConvertFromUtc(occurrence.EndTime);
                occurrence.ApplicationDeadline = VollyConstants.ConvertFromUtc(occurrence.ApplicationDeadline);
                return occurrence;
            };
        }

        public static Func<Occurrence, Occurrence> ConvertToUtc()
        {
            return delegate (Occurrence occurrence)
            {
                occurrence.StartTime = VollyConstants.ConvertToUtc(occurrence.StartTime);
                occurrence.EndTime = VollyConstants.ConvertToUtc(occurrence.EndTime);
                occurrence.ApplicationDeadline = VollyConstants.ConvertToUtc(occurrence.ApplicationDeadline);
                return occurrence;
            };
        }
    }
}
