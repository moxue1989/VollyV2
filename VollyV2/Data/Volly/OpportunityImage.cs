using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VollyV2.Services;

namespace VollyV2.Data.Volly
{
    public class OpportunityImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        [Required]
        public int OpportunityId { get; set; }
        public virtual Opportunity Opportunity { get; set; }
    }
}
