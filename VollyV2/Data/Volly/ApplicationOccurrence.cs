using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV2.Data.Volly
{
    public class ApplicationOccurrence
    {
        public int ApplicationId { get; set; }
        public Application Application { get; set; }
        public int OccurrenceId { get; set; }
        public Occurrence Occurrence { get; set; }
    }
}
