using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV2.Data.Volly
{
    public class ApplicationOccurrence
    {
        public int ApplicationId { get; set; }
        public virtual Application Application { get; set; }
        public int OccurrenceId { get; set; }
        public virtual Occurrence Occurrence { get; set; }
    }
}
