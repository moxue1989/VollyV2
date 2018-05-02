using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VollyV2.Models;
using VollyV2.Models.Volly;

namespace VollyV2.Data.Volly
{
    public class Opportunity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public Organization Organization { get; set; }
        public Category Category { get; set; }
        public Location Location { get; set; }
        public DateTime DateTime { get; set; }
        public string ImageUrl { get; set; }
        public string CreatedByUserId { get; set; }
        public ApplicationUser CreatedByUser { get; set; }
    }
}
