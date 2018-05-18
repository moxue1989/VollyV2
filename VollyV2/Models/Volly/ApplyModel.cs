using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VollyV2.Data;
using VollyV2.Data.Volly;

namespace VollyV2.Models.Volly
{
    public class ApplyModel
    {
        public Opportunity Opportunity { get; set; }
        [Required]
        public int OpportunityId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Message { get; set; }

        public Application GetApplication(ApplicationDbContext context)
        {
            return new Application()
            {
                Name = Name,
                Email = Email,
                Message = Message,
                DateTime = DateTime.UtcNow,
                Opportunity = context.Opportunities.Find(OpportunityId)
            };
        }
    }
}
