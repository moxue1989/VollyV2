using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VollyV2.Models;

namespace VollyV2.Data.Volly
{
    public class Company
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string ContactEmail { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        [Required]
        [MinLength(6)]
        public string CompanyCode { get; set; }
        public List<ApplicationUser> Users { get; set; }
    }
}
