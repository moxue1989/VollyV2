using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VollyV2.Controllers;
using VollyV2.Data.Volly;
using VollyV2.Models.Views;

namespace VollyV2.Models.Volly
{
    public class ApplicationView
    {
        public int Id { get; set; }
        public int OpportunityId { get; set; }
        public string OpportunityName { get; set; }
        public string OpportunityImageUrl { get; set; }
        public string OpportunityDescription { get; set; }
        public string OpportunityAddress { get; set; }
        public List<OccurrenceView> OccurrenceViews { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        [DisplayFormat(DataFormatString = "{0:ddd MMM d yyyy h:mm tt}")]
        public DateTime DateTime { get; set; }
        public string UserName { get; set; }

        public static ApplicationView FromApplication(Application application)
        {
            return new ApplicationView()
            {
                Id = application.Id,
                OpportunityId = application.Opportunity.Id,
                OpportunityName = application.Opportunity.Name,
                OpportunityImageUrl = application.Opportunity.ImageUrl,
                OpportunityAddress = application.Opportunity.Address,
                OpportunityDescription = application.Opportunity.Description,
                Name = application.Name,
                Email = application.Email,
                Message = application.Message,
                DateTime = VollyConstants.ConvertFromUtc(application.DateTime),
                OccurrenceViews = application.Occurrences
                    .Select(ao => ao.Occurrence)
                    .Select(OccurrenceView.FromOccurrence)
                    .ToList(),
                UserName = application.User?.UserName
            };
        }
        public string GetEmailMessage()
        {
            return "<p>Applicant Name: " + Name + "<p/>" +
                   "<p>Applicant Email: " + Email + "<p/>" +
                   "<p>Message: " + Message + "<p/>";
        }
    }
}

