using MailChimp.Net;
using MailChimp.Net.Core;
using MailChimp.Net.Interfaces;
using MailChimp.Net.Models;
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
    [Route("api/NewsletterApi")]
    public class NewsletterApiController : Controller
    {
        private static readonly string MailChimpApiKey = Environment.GetEnvironmentVariable("mailchimp_api");
        private static readonly string ListId = "100b8d1f2d";
        private Setting _campaignSettings = new Setting
        {
            ReplyTo = "maillet.mark@gmail.com",
            FromName = "From name",
            Title = "Your campaign title",
            SubjectLine = "The email subject",
        };

        private readonly ApplicationDbContext _context;

        public NewsletterApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> CreateAndSendCampaignAsync()
        {
            var mailChimpManager = new MailChimpManager(MailChimpApiKey);

            List<Opportunity> opportunities = await GetRandomRecentOpportunities(10, 5);

            var campaign = mailChimpManager.Campaigns.AddAsync(new Campaign
            {
                Settings = _campaignSettings,
                Recipients = new Recipient { ListId = ListId },
                Type = CampaignType.Regular
            }).Result;
            var timeStr = DateTime.Now.ToString();
            var content = mailChimpManager.Content.AddOrUpdateAsync(
             campaign.Id,
             new ContentRequest()
             {
                 Html = await GenerateHtmlFromOpportunitiesAsync(opportunities)
                 //Template = new ContentTemplate
                 //{
                 //    Id = TemplateId,
                 //    Sections = new Dictionary<string, object>()
                 //         {
                 //           { "body", "hello world" },
                 //         }
                 //}
             }).Result;
            mailChimpManager.Campaigns.SendAsync(campaign.Id).Wait();

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

        private async Task<string> GenerateHtmlFromOpportunitiesAsync(List<Opportunity> opportunities)
        {

            string test = await this.RenderViewAsync(new EmailTemplateController(), "Newsletter", false);

            string html =
                "<p>Hi Everyone!</p>" +
                "<p>Please find below some new volunteer opportunities!</p>" +
                "If you need help finding something specific to do, please email " +
                "<a href='mailto:" + VollyConstants.VollyAdminEmail + "'>" + VollyConstants.VollyAdminEmail + "</a><br/>" +
                "Upcoming Volunteer Opportunities<br/>" +
                "<ul>";
            foreach (Opportunity opportunity in opportunities)
            {
                html += "<li>" + opportunity.Name + "</li>";
            }
            html += "</ul>";
            html +=
                "<a href='https://volly.app/Home/Opportunities'>Browse All Opportunities</a><br/>" +
                "Feel free to share with your friends and family!<br/>" +
                "Team Volly";
            return html;
        }

        private async Task<List<Opportunity>> GetRandomRecentOpportunities(int takeFromTopCount, int count)
        {
            List<int> opportunityIds = await _context.Occurrences
                .Where(o => o.ApplicationDeadline > DateTime.Now && o.Applications.Count < o.Openings)
                .Select(o => o.OpportunityId)
                .ToListAsync();

            opportunityIds = await _context.Opportunities
                .Where(o =>
                o.Approved
                && o.OpportunityType == OpportunityType.Episodic
                && opportunityIds.Contains(o.Id))
                .Select(o => o.Id)
                .ToListAsync();

            opportunityIds.Sort();
            opportunityIds.Reverse();

            if (opportunityIds.Count > takeFromTopCount)
            {
                opportunityIds = opportunityIds.Take(takeFromTopCount).ToList();
            }

            List<int> filteredOpportunityIds;

            if (opportunityIds.Count <= count)
            {
                filteredOpportunityIds = opportunityIds;
            }
            else
            {
                filteredOpportunityIds = new List<int>();
                Random random = new Random();
                while (filteredOpportunityIds.Count < count)
                {
                    int opportunityId = opportunityIds[random.Next(opportunityIds.Count)];
                    if (!filteredOpportunityIds.Contains(opportunityId))
                    {
                        filteredOpportunityIds.Append(opportunityId);
                    }
                }
            }

            List<Opportunity> opportunities = await _context.Opportunities
                .Where(o => filteredOpportunityIds.Contains(o.Id))
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
