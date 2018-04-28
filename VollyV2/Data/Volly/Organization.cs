using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV2.Data.Volly
{
    public class Organization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string WebsiteLink { get; set; }
        public string MissionStatement { get; set; }
        public string FullDescription { get; set; }
        public Cause Cause { get; set; }
        public Location Location { get; set; }
    }
}
