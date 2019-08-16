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
        [Display(Name = "Title of Event")]
        public string Name { get; set; }
        [Display(Name = "Describe the volunteer opportunity. Please include minimum age to volunteer and perks of volunteering.")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Where does the volunteering occur? Enter Address or simply write the city name if there are multiple locations.")]
        public string Address { get; set; }
        [Required]
        [Display(Name = "Non Profit Name")]
        public int OrganizationId { get; set; }
        [Required]
        [Display(Name = "What type of volunteering is this?")]
        public int CategoryId { get; set; }
        [Display(Name = "Community")]
        public int CommunityId { get; set; }
        public SelectList Organizations { get; set; }
        public SelectList Categories { get; set; }
        public SelectList Communities { get; set; }
        public string ImageUrl { get; set; }
        [Display(Name = "Upload an image for this event")]
        public IFormFile ImageFile { get; set; }
        [Display(Name = "Enter the URL of the application form or sign up sheet. Ex: volunteersignup.org or signupgenius")]
        public string ExternalSignUpUrl { get; set; }
        [Display(Name = "Opportunity Type")]
        public OpportunityType OpportunityType { get; set; }
        public bool Approved { get; set; }

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
                    .ToList(), "Id", "Name"),
                Approved = opportunity.Approved
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
