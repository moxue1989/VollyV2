using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV2.Models.Volly
{
    public class OpportunitySearch
    {
        public List<int> CauseIds { get; set; }
        public List<int> CategoryIds { get; set; }
        public List<DateTime> Dates { get; set; }
    }
}
