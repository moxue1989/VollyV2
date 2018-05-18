using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VollyV2.Controllers;
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
        public DateTime EndDateTime { get; set; }
        public DateTime ApplicationDeadline { get; set; }
        public int Openings { get; set; }
        public List<Application> Applications { get; set; }
        public string ImageUrl { get; set; }
        public string CreatedByUserId { get; set; }
        public ApplicationUser CreatedByUser { get; set; }

        public Opportunity Clone()
        {
            return new Opportunity()
            {
                Name = "copy of " + Name,
                Description = Description,
                Address = Address,
                Organization = Organization,
                Category = Category,
                Location = new Location()
                {
                    Longitude = Location.Longitude,
                    Latitude = Location.Latitude
                },
                DateTime = DateTime,
                EndDateTime = EndDateTime,
                ApplicationDeadline = ApplicationDeadline,
                ImageUrl = ImageUrl
            };
        }


    }

    public static class OpportunityTimeZoneConverter
    {
        public static Func<Opportunity, Opportunity> ConvertFromUtc()
        {
            return delegate (Opportunity opportunity)
            {
                opportunity.DateTime = VollyConstants.ConvertFromUtc(opportunity.DateTime);
                opportunity.EndDateTime = VollyConstants.ConvertFromUtc(opportunity.EndDateTime);
                opportunity.ApplicationDeadline = VollyConstants.ConvertFromUtc(opportunity.ApplicationDeadline);
                return opportunity;
            };
        }

        public static Func<Opportunity, Opportunity> ConvertToUtc()
        {
            return delegate (Opportunity opportunity)
            {
                opportunity.DateTime = VollyConstants.ConvertToUtc(opportunity.DateTime);
                opportunity.EndDateTime = VollyConstants.ConvertToUtc(opportunity.EndDateTime);
                opportunity.ApplicationDeadline = VollyConstants.ConvertToUtc(opportunity.ApplicationDeadline);
                return opportunity;
            };
        }
    }
}
