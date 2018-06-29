using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VollyV2.Models;

namespace VollyV2.Data.Volly
{
    public class Company
    {
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string CompanyCode { get; set; }
        public Location Location { get; set; }
        public List<ApplicationUser> Users { get; set; }
    }
}
