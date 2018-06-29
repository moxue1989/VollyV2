using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VollyV2.Data.Volly;
using VollyV2.Models.Volly;

namespace VollyV2.Models.Views
{
    public class OccurrenceView
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime ApplicationDeadline { get; set; }
        public int Openings { get; set; }

        public static OccurrenceView FromOccurrence(Occurrence occurrence)
        {
            return new OccurrenceView
            {
                Id = occurrence.Id,
                StartTime = occurrence.StartTime,
                EndTime = occurrence.EndTime,
                ApplicationDeadline = occurrence.ApplicationDeadline,
                Openings = occurrence.Openings
            };
        }
    }
}
