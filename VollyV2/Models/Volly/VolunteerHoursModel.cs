using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using VollyV2.Controllers;
using VollyV2.Data;
using VollyV2.Data.Volly;

namespace VollyV2.Models.Volly
{
    public class VolunteerHoursModel
    {
        private const string TimeFormat = "ddd MMM d yyyy h:mm tt";

        public int Id { get; set; }
        public string User { get; set; }
        public string UserId { get; set; }

        [Display(Name = "Opportunity Name")]
        public string OpportunityName { get; set; }

        [Display(Name = "Organization")] public string OrganizationName { get; set; }
        [Display(Name = "Date")] public DateTime DateTime { get; set; }
        [Required] public double Hours { get; set; }

        public static VolunteerHoursModel FromVolunteerHours(VolunteerHours volunteerHours)
        {
            return new VolunteerHoursModel()
            {
                Id = volunteerHours.Id,
                User = volunteerHours.User.Email,
                UserId = volunteerHours.User.Id,
                DateTime = VollyConstants.ConvertFromUtc(volunteerHours.DateTime),
                Hours = volunteerHours.Hours,
                OrganizationName = volunteerHours.OrganizationName,
                OpportunityName = volunteerHours.OpportunityName
            };
        }

        public void CreateOrUpdate(ApplicationDbContext context, string userId)
        {
            VolunteerHours volunteerHours = context.VolunteerHours.Find(Id);
            DateTime dateTime = VollyConstants.ConvertToUtc(DateTime);

            if (volunteerHours != null)
            {
                volunteerHours.Hours = Hours;
                volunteerHours.DateTime = dateTime;
                volunteerHours.OpportunityName = OpportunityName;
                volunteerHours.OrganizationName = OrganizationName;
            }
            else
            {
                VolunteerHours newVolunteerHours = new VolunteerHours()
                {
                    OpportunityName = OpportunityName,
                    OrganizationName = OrganizationName,
                    DateTime = dateTime,
                    Hours = Hours,
                    UserId = userId
                };

                context.Add(newVolunteerHours);
            }

            context.SaveChanges();
        }

    }
}