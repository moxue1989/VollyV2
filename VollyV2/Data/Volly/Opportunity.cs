using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using VollyV2.Controllers;
using VollyV2.Models;
using VollyV2.Models.Volly;
using VollyV2.Services;

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
        [DisplayFormat(DataFormatString = "{0:ddd MMM d yyyy h:mm tt}")]
        public DateTime DateTime { get; set; }
        [DisplayFormat(DataFormatString = "{0:ddd MMM d yyyy h:mm tt}")]
        public DateTime EndDateTime { get; set; }
        [DisplayName("Application deadline")]
        [DisplayFormat(DataFormatString = "{0:ddd MMM d yyyy h:mm tt}")]
        public DateTime ApplicationDeadline { get; set; }
        public int Openings { get; set; }
        public List<Application> Applications { get; set; }
        public string ImageUrl { get; set; }
        public string CreatedByUserId { get; set; }
        public ApplicationUser CreatedByUser { get; set; }
        public List<OpportunityImage> OpportunityImages { get; set; }
        public List<Occurrence> Occurrences { get; set; }

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

        public async Task<OpportunityImage> UploadImage(IImageManager imageManager, ApplicationDbContext context, IFormFile imageFile)
        {
            string imageUrl = await imageManager.UploadImageAsync(imageFile, GetImageFileName(imageFile.FileName));
            OpportunityImage image = new OpportunityImage()
            {
                OpportunityId = Id,
                ImageUrl = imageUrl

            };
            context.OpportunityImages.Add(image);
            await context.SaveChangesAsync();

            return image;
        }

        private string GetImageFileName(string fileName)
        {
            return "oppimage" + Id + fileName;
        }
    }
}
