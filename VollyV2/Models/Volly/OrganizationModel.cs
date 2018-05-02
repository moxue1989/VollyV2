using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using VollyV2.Controllers;
using VollyV2.Data;
using VollyV2.Data.Volly;
using VollyV2.Services;

namespace VollyV2.Models.Volly
{
    public class OrganizationModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string ContactEmail { get; set; }
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Required]
        public string Address { get; set; }
        [Url]
        public string WebsiteLink { get; set; }
        public string MissionStatement { get; set; }
        public string FullDescription { get; set; }
        [Required]
        [Display(Name = "Cause")]
        public int CauseId { get; set; }
        public SelectList Causes { get; set; }

        public static OrganizationModel FromOrganization(ApplicationDbContext dbContext, Organization organization)
        {
            return new OrganizationModel
            {
                Id = organization.Id,
                Name = organization.Name,
                ContactEmail = organization.ContactEmail,
                PhoneNumber = organization.PhoneNumber,
                Address = organization.Address,
                WebsiteLink = organization.WebsiteLink,
                MissionStatement = organization.MissionStatement,
                FullDescription = organization.FullDescription,
                CauseId = organization.Cause?.Id ?? 0,
                Causes = new SelectList(dbContext.Causes
                    .OrderBy(c => c.Name)
                    .ToList(), "Id", "Name")
            };
        }

        public Organization GetOrganization(ApplicationDbContext context)
        {
            return new Organization
            {
                Id = Id,
                Name = Name,
                ContactEmail = ContactEmail,
                PhoneNumber = PhoneNumber,
                Address = Address,
                WebsiteLink = WebsiteLink,
                MissionStatement = MissionStatement,
                FullDescription = FullDescription,
                Cause = context.Causes.Find(CauseId),
                Location = GoogleLocator.GetLocationFromAddress(Address)
            };
        }
    }
}
