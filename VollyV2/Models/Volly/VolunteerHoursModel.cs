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
        public int ApplicationId { get; set; }
        public string ApplicationDescription { get; set; }
        public string OrganizationName { get; set; }
        public DateTime DateTime { get; set; }
        [Required]
        public double Hours { get; set; }

        public static VolunteerHoursModel FromApplication(Application application)
        {
            VolunteerHours existingVolunteerHours = application.VolunteerHours;
            string applicationDescription = GetApplicationDescription(application);
            if (existingVolunteerHours != null)
            {
                return new VolunteerHoursModel()
                {
                    Id = existingVolunteerHours.Id,
                    ApplicationId = existingVolunteerHours.ApplicationId,
                    ApplicationDescription = applicationDescription,
                    DateTime = VollyConstants.ConvertFromUtc(existingVolunteerHours.DateTime),
                    Hours = existingVolunteerHours.Hours,
                    OrganizationName = existingVolunteerHours.OrganizationName
                };
            }
            return new VolunteerHoursModel()
            {
                ApplicationId = application.Id,
                ApplicationDescription = applicationDescription,
                DateTime = application.DateTime,
                OrganizationName = application.Opportunity.Organization.Name
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
                volunteerHours.OrganizationName = OrganizationName;
            }
            else
            {
                VolunteerHours newVolunteerHours = new VolunteerHours()
                {
                    Application = context.Applications.Find(ApplicationId),
                    DateTime = dateTime,
                    Hours = Hours,
                    OrganizationName = OrganizationName,
                    UserId = userId
                };

                context.Add(newVolunteerHours);
            }
            context.SaveChanges();
        }

        private static string GetApplicationDescription(Application application)
        {
            if (application == null)
            {
                return "User entered hours";
            }
            ApplicationView applicationView = ApplicationView.FromApplication(application);
            return applicationView.OpportunityName + ": " +
                   applicationView.OccurrenceViews
                       .Select(o => o.StartTime.ToString(TimeFormat) + " -> " +
                                    o.EndTime.ToString(TimeFormat))
                       .Join(" And ");
        }
    }
}
