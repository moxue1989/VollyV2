using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using VollyV2.Controllers;
using VollyV2.Data;
using VollyV2.Data.Volly;
using VollyV2.Services;

namespace VollyV2.Models.Volly
{
    public class OpportunityModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public int OrganizationId { get; set; }
        public int CategoryId { get; set; }
        public DateTime DateTime { get; set; }
        public SelectList Organizations { get; set; }
        public SelectList Categories { get; set; }
        public string ImageUrl { get; set; }

        public static OpportunityModel FromOpportunity(ApplicationDbContext dbContext, Opportunity opportunity)
        {
            return new OpportunityModel()
            {
                Id = opportunity.Id,
                Name = opportunity.Name,
                Description = opportunity.Description,
                Address = opportunity.Address,
                OrganizationId = opportunity.Organization.Id,
                CategoryId = opportunity.Category.Id,
                ImageUrl = opportunity.ImageUrl,
                DateTime = TimeZoneInfo.ConvertTimeFromUtc(opportunity.DateTime, VollyConstants.TimeZoneInfo),
                Categories = new SelectList(dbContext.Categories
                    .OrderBy(c => c.Name)
                    .ToList(), "Id", "Name"),
                Organizations = new SelectList(dbContext.Organizations
                    .OrderBy(o => o.Name)
                    .ToList(), "Id", "Name")
            };
        }

        public Opportunity GetOpportunity(ApplicationDbContext context)
        {
            Opportunity opportunity = context.Opportunities.Find(Id);
            if (opportunity == null)
            {
                opportunity = new Opportunity();
            }
            opportunity.Name = Name;
            opportunity.Description = Description;
            opportunity.Address = Address;
            opportunity.ImageUrl = ImageUrl;
            opportunity.Organization = context.Organizations.Find(OrganizationId);
            opportunity.Category = context.Categories.Find(CategoryId);
            opportunity.DateTime = TimeZoneInfo.ConvertTimeToUtc(DateTime, VollyConstants.TimeZoneInfo);
            opportunity.Location = GoogleLocator.GetLocationFromAddress(Address);

            return opportunity;
        }
    }
}
