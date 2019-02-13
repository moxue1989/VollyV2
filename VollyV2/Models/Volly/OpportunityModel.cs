using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using VollyV2.Data;
using VollyV2.Data.Volly;
using VollyV2.Services;

namespace VollyV2.Models.Volly
{
    public class OpportunityModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        [Display(Name = "Organization")]
        public int OrganizationId { get; set; }
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [Display(Name = "Community")]
        public int CommunityId { get; set; }
        public SelectList Organizations { get; set; }
        public SelectList Categories { get; set; }
        public SelectList Communities { get; set; }
        public string ImageUrl { get; set; }
        public IFormFile ImageFile { get; set; }
        [Display(Name = "External Sign Up Url")]
        public string ExternalSignUpUrl { get; set; }
        public OpportunityType OpportunityType { get; set; }

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
                CommunityId = opportunity.Community?.Id ?? 0,
                ImageUrl = opportunity.ImageUrl,
                ExternalSignUpUrl = opportunity.ExternalSignUpUrl,
                OpportunityType = opportunity.OpportunityType,
                Categories = new SelectList(dbContext.Categories
                    .OrderBy(c => c.Name)
                    .ToList(), "Id", "Name"),
                Organizations = new SelectList(dbContext.Organizations
                    .OrderBy(o => o.Name)
                    .ToList(), "Id", "Name"),
                Communities = new SelectList(dbContext.Communities
                    .OrderBy(o => o.Name)
                    .ToList(), "Id", "Name")
            };
        }

        public Opportunity GetOpportunity(ApplicationDbContext context, IImageManager imageManager)
        {
            string imageUrl = ImageFile == null ? null : imageManager.UploadImageAsync(ImageFile, "opp" + Id + ImageFile.FileName).Result;

            Opportunity opportunity = context.Opportunities.Find(Id) ?? new Opportunity();
            opportunity.Name = Name;
            opportunity.Description = Description;
            opportunity.Address = Address;
            if (imageUrl != null)
            {
                opportunity.ImageUrl = imageUrl;
            }
            opportunity.ExternalSignUpUrl = ExternalSignUpUrl;
            opportunity.Organization = context.Organizations.Find(OrganizationId);
            opportunity.Category = context.Categories.Find(CategoryId);
            opportunity.Community = context.Communities.Find(CommunityId);
            opportunity.Location = GoogleLocator.GetLocationFromAddress(Address);
            opportunity.OpportunityType = OpportunityType;

            return opportunity;
        }
    }
}
