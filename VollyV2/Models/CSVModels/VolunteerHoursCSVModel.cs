using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VollyV2.Controllers;
using VollyV2.Data.Volly;

namespace VollyV2.Models.CSVModels
{
    public class VolunteerHoursCSVModel
    {
        public string Company { get; set; }
        public string CompanyCode { get; set; }
        public string User { get; set; }
        public string OrganizationName { get; set; }
        public string OpportunityName { get; set; }
        public DateTime DateTime { get; set; }
        public double Hours { get; set; }
        public static VolunteerHoursCSVModel FromVolunteerHours(VolunteerHours volunteerHours)
        {
            return new VolunteerHoursCSVModel()
            {
                Company = volunteerHours.User?.Company?.Name,
                CompanyCode = volunteerHours.User?.Company?.CompanyCode,
                User = volunteerHours.User.Email,
                OrganizationName = volunteerHours.OrganizationName,
                OpportunityName = volunteerHours.OpportunityName,
                DateTime = VollyConstants.ConvertFromUtc(volunteerHours.DateTime),
                Hours = volunteerHours.Hours
            };
        }
    }
    public class VolunteerHoursCSVModelMap : ClassMap<VolunteerHoursCSVModel>
    {
        public VolunteerHoursCSVModelMap()
        {
            Map(m => m.Company).Name("Company");
            Map(m => m.CompanyCode).Name("CompanyCode");
            Map(m => m.User).Name("User");
            Map(m => m.OpportunityName).Name("OpportunityName");
            Map(m => m.OrganizationName).Name("OrganizationName");
            Map(m => m.DateTime).Name("Date");
            Map(m => m.Hours).Name("Hours");
        }
    }
}
