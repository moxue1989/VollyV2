using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        [MinLength(1)]
        [DisplayName("Occurrence")]
        public List<int> OccurrenceIds { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Message { get; set; }
        public SelectList Occurrences { get; set; } 

        public async Task<Application> GetApplication(ApplicationDbContext context)
        {
            Application application = new Application()
            {
                Name = Name,
                Email = Email,
                Message = Message,
                DateTime = DateTime.UtcNow,
                Opportunity = context.Opportunities.Find(OpportunityId)
            };

            context.Applications.Add(application);
            await context.SaveChangesAsync();
            UpdateOccurences(context, application);
            return application;
        }

        private  void UpdateOccurences(ApplicationDbContext context, Application application)
        {
            List<ApplicationOccurrence> occurrenceApplications = OccurrenceIds.Select(o => new ApplicationOccurrence()
            {
                Application = application,
                Occurrence = context.Occurrences.Find(o)
            }).ToList();

            context.ApplicationsOccurrence.AddRange(occurrenceApplications);
            context.SaveChangesAsync();
        }
    }
}
