using MailChimp.Net;
using MailChimp.Net.Core;
using MailChimp.Net.Interfaces;
using MailChimp.Net.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VollyV2.Data;
using VollyV2.Data.Volly;
using VollyV2.Extensions;
using VollyV2.Services;

namespace VollyV2.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/VolunteerNewsletterApi")]
    [Authorize(Roles = "PowerUser")]
    public class VolunteerNewsletterApiController : Controller
    {
        private static readonly string ReplyToEmail = VollyConstants.VollyAdminEmail;
        private static readonly string NewsletterFrom = "Volly Newsletter";
        private static readonly string NewsletterSubject = "Volly Opportunities";
        private static readonly string StreetAddress = "Calgary, AB";

        private static readonly string UnsubscribeUrl = "https://volly.app/Unsubscribe";

        private static readonly int TakeFromTopCount = 10;
        private static readonly int NumberOfOpportunitiesForTypeToInclude = 4;

        private static readonly string MailChimpApiKey = Environment.GetEnvironmentVariable("mailchimp_api");
        private static readonly string ListId = "100b8d1f2d";

        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public VolunteerNewsletterApiController(
            ApplicationDbContext context,
            IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        [HttpGet]
        public async Task<IActionResult> CreateAndSendNewslettersAsync()
        {
            ViewData["StreetAddress"] = StreetAddress;
            ViewData["UnsubscribeUrl"] = UnsubscribeUrl;
            await CreateAndSendSendGridNewsletterAsync(
                await GetRandomRecentOpportunitiesForType(OpportunityType.Episodic),
                await GetRandomRecentOpportunitiesForType(OpportunityType.Ongoing),
                await GetRandomRecentOpportunitiesForType(OpportunityType.Donation));
            return Ok();
        }

        private async Task<IActionResult> CreateAndSendSendGridNewsletterAsync(
            List<Opportunity> episodicOpportunities,
            List<Opportunity> ongoingOpportunities,
            List<Opportunity> donationOpportunities)
        {
            var mailChimpManager = new MailChimpManager(MailChimpApiKey);

            var members = await mailChimpManager.Members.GetAllAsync(ListId).ConfigureAwait(false);

            var html = await GenerateSendGridHtmlFromOpportunitiesAsync(
                episodicOpportunities,
                ongoingOpportunities,
                donationOpportunities);


            //await _emailSender.SendEmailsAsync(
            //    members.Select(member => member.EmailAddress).ToList(),
            //    NewsletterSubject,
            //    html);

            return Ok();
        }

        public async Task<IActionResult> RunJob()
        {
            //await _emailSender.SendEmailsAsync(new List<string>()
            //{
            //    VollyConstants.AliceEmail,
            //    VollyConstants.MoEmail
            //}, "Newsletter Web job", "web job started");

            //List<NewsletterEntry> newsletterEntries = _dbContext.NewsletterEntries.ToList();

            //List<string> emailsForNewsletter = newsletterEntries
            //       .Select(e => e.Email)
            //       .ToList();

            //await _emailSender.SendEmailsAsync(emailsForNewsletter, "This is a test Subject","Message testing the mssages");

            return Ok();
        }

        private async Task<string> GenerateSendGridHtmlFromOpportunitiesAsync(
            List<Opportunity> episodicOpportunities,
            List<Opportunity> ongoingOpportunities,
            List<Opportunity> donationOpportunities)
        {
            ViewData["OpportunitiesHtml"] = "<br/><div style='display:block;text-align:center;color:#fff;'>Episodic Opportunities</div><br/>";
            ViewData["OpportunitiesHtml"] += GetOpportunitiesHtml(episodicOpportunities);
            ViewData["OpportunitiesHtml"] += "<br/><div style='display:block;text-align:center;color:#fff;'>Ongoing Opportunities</div><br/>";
            ViewData["OpportunitiesHtml"] += GetOpportunitiesHtml(ongoingOpportunities);
            ViewData["OpportunitiesHtml"] += "<br/><div style='display:block;text-align:center;color:#fff;'>Accepting Donations</div><br/>";
            ViewData["OpportunitiesHtml"] += GetOpportunitiesHtml(donationOpportunities);
            return await this.RenderViewAsync("NewsletterSendGrid");
        }


        private string GetOpportunitiesHtml(List<Opportunity> opportunities)
        {
            string html = "";
            for (var i = 0; i < opportunities.Count; i++)
            {
                html += GetTemplateForOpportunity(opportunities[i], i % 2 == 0);
            }
            return html;
        }

        private string GetTemplateForOpportunity(Opportunity opportunity, bool IsTextLeft)
        {
            return "<table border='0' cellpadding='0' cellspacing='0' width='100%'>" +
                "<tbody>" +
                "<tr>" +
                "<td valign='top' style='padding:9px;'>" +
                "<table border='0' cellpadding='0' cellspacing='0' width='100%'>" +
                "<tbody>" +
                "<tr>" +
                "<td valign ='top' style='padding:0 9px ;'>" +
                 GetTextForTile(opportunity, IsTextLeft) +
                 GetImageForTile(opportunity, !IsTextLeft) +
                "</td>" +
                "</tr>" +
                "</tbody>" +
                "</table>" +
                "</td>" +
                "</tr>" +
                "</tbody>" +
                "</table>";
        }

        private string GetTextForTile(Opportunity opportunity, bool IsLeft)
        {
            return "<table align='" + (IsLeft ? "left" : "right") + "' border='0' cellpadding='0' cellspacing='0' width='264'>" +
            "<tbody>" +
            "<tr>" +
            "<td valign='top'>" +
            "<h3>" +
            "<font color='#ffffff'>" + opportunity.Name + "<br/>" + "</font>" +
            "</h3>" +
            "<p><span style='color:#FFFFFF'>" + opportunity.Description + "</span><br />" +
            "<a href='https://volly.app/Home/Opportunities/" + opportunity.Id + "' target='_blank'>" +
            "<span style='color:#EE82EE'>Sign up</span>" +
            "</a>" +
            "</p>" +
            "</td>" +
            "</tr>" +
            "</tbody>" +
            "</table>";

        }

        private string GetImageForTile(Opportunity opportunity, bool IsLeft)
        {
            return "<table align='" + (IsLeft ? "left" : "right") + "' border='0' cellpadding='0' cellspacing='0' width='264'>" +
                "<tbody>" +
                "<tr>" +
                "<td align='center' valign='top'>" +
                "<img alt='' src='" + opportunity.ImageUrl + "' width='264' style='max-width:564px;'>" +
                "</td>" +
                "</tr>" +
                "</tbody>" +
                "</table>";
        }

        private async Task<List<Opportunity>> GetRandomRecentOpportunitiesForType(OpportunityType opportunityType)
        {
            List<int> opportunityIds = await _context.Occurrences
                .Where(o => o.ApplicationDeadline > DateTime.Now && o.Applications.Count < o.Openings)
                .Select(o => o.OpportunityId)
                .ToListAsync();

            opportunityIds = await _context.Opportunities
                .Where(o =>
                o.Approved
                && o.OpportunityType == opportunityType
                && opportunityIds.Contains(o.Id))
                .Select(o => o.Id)
                .ToListAsync();

            opportunityIds.Sort();
            opportunityIds.Reverse();

            if (opportunityIds.Count > TakeFromTopCount)
            {
                opportunityIds = opportunityIds.Take(TakeFromTopCount).ToList();
            }

            List<int> filteredOpportunityIds = opportunityIds;

            if (opportunityIds.Count > NumberOfOpportunitiesForTypeToInclude)
            {
                filteredOpportunityIds = new List<int>();
                Random random = new Random();
                while (filteredOpportunityIds.Count < NumberOfOpportunitiesForTypeToInclude)
                {
                    int opportunityId = opportunityIds[random.Next(opportunityIds.Count)];
                    if (!filteredOpportunityIds.Contains(opportunityId))
                    {
                        filteredOpportunityIds.Append(opportunityId);
                    }
                }
            }

            List<Opportunity> opportunities = await _context.Opportunities
                .Where(o => filteredOpportunityIds.Contains(o.Id)
                && o.Approved
                && o.OpportunityType == opportunityType
                )
                .Include(o => o.Category)
                .Include(o => o.Community)
                .Include(o => o.Organization)
                .ThenInclude(o => o.Cause)
                .Include(o => o.Location)
                .Include(o => o.Occurrences)
                .ThenInclude(o => o.Applications)
                .AsNoTracking()
                .ToListAsync();

            foreach (Opportunity opportunity in opportunities)
            {
                opportunity.Occurrences = opportunity.Occurrences
                    .Where(oc => oc.ApplicationDeadline > DateTime.Now && oc.Openings > oc.Applications.Count)
                    .ToList();
            }

            return opportunities;
        }
    }
}
