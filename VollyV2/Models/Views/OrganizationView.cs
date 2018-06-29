using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VollyV2.Data.Volly;

namespace VollyV2.Models.Views
{
    public class OrganizationView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string WebsiteLink { get; set; }
        public string MissionStatement { get; set; }
        public string FullDescription { get; set; }
        public string CauseName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ImageUrl { get; set; }

        public static OrganizationView FromOrganization(Organization organization)
        {
            return new OrganizationView()
            {
                Id = organization.Id,
                Name = organization.Name,
                ContactEmail = organization.ContactEmail,
                PhoneNumber = organization.PhoneNumber,
                Address = organization.Address,
                WebsiteLink = organization.WebsiteLink,
                MissionStatement = organization.MissionStatement,
                FullDescription = organization.FullDescription,
                CauseName = organization.Cause.Name,
                Latitude = organization.Location.Latitude,
                Longitude = organization.Location.Longitude,
                ImageUrl = organization.ImageUrl
            };
        }
    }
}
