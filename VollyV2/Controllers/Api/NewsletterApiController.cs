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
    [Route("api/NewsletterApi")]
    [Authorize(Roles = "PowerUser")]
    public class NewsletterApiController : Controller
    {
        private static readonly string ReplyToEmail = VollyConstants.VollyAdminEmail;
        private static readonly string NewsletterFrom = "Volly Newsletter";
        private static readonly string NewsletterSubject = "Volly Opportunities";
        private static readonly string StreetAddress = "123 Fake St.";

        private static readonly string UnsubscribeUrl = "https://volly.app/Unsubscribe";

        private static readonly int TakeFromTopCount = 8;
        private static readonly int NumberOfOpportunitiesToInclude = 8;

        private static readonly string MailChimpApiKey = Environment.GetEnvironmentVariable("mailchimp_api");
        private static readonly string ListId = "100b8d1f2d";

        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        private Setting _campaignSettings = new Setting
        {
            ReplyTo = ReplyToEmail,
            FromName = NewsletterFrom,
            Title = NewsletterSubject,
            SubjectLine = NewsletterSubject,
        };

        public NewsletterApiController(
            ApplicationDbContext context,
            IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        [HttpGet]
        public async Task<IActionResult> CreateAndSendNewslettersAsync()
        {
            ViewData["SteetAdress"] = StreetAddress;
            ViewData["UnsubscribeUrl"] = UnsubscribeUrl;
            List<Opportunity> opportunities = await GetRandomRecentOpportunities();
            //await CreateAndSendMailChimpNewsletterAsync(opportunities);
            await CreateAndSendSendGridNewsletterAsync(opportunities);
            return Ok();
        }

        private async Task<IActionResult> CreateAndSendMailChimpNewsletterAsync(List<Opportunity> opportunities)
        {
            var mailChimpManager = new MailChimpManager(MailChimpApiKey);

            var campaign = mailChimpManager.Campaigns.AddAsync(new Campaign
            {
                Settings = _campaignSettings,
                Recipients = new Recipient { ListId = ListId },
                Type = CampaignType.Regular
            }).Result;

            var content = mailChimpManager.Content.AddOrUpdateAsync(
             campaign.Id,
             new ContentRequest()
             {
                 Html = await GenerateMailChimpHtmlFromOpportunitiesAsync(opportunities)
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

        private async Task<IActionResult> CreateAndSendSendGridNewsletterAsync(List<Opportunity> opportunities)
        {
            //var html = await GenerateSendGridHtmlFromOpportunitiesAsync(opportunities);

            var html = "testing";
            await _emailSender.SendEmailAsync(VollyConstants.MarkEmail, NewsletterSubject, html);

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

        private async Task<string> GenerateMailChimpHtmlFromOpportunitiesAsync(List<Opportunity> opportunities)
        {
            ViewData["OpportunitiesHtml"] = GetOpportunitiesHtml(opportunities);
            return await this.RenderViewAsync("NewsletterMailChimp");
        }

        private async Task<string> GenerateSendGridHtmlFromOpportunitiesAsync(List<Opportunity> opportunities)
        {
            ViewData["OpportunitiesHtml"] = GetOpportunitiesHtml(opportunities);
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

        private async Task<List<Opportunity>> GetRandomRecentOpportunities()
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

            if (opportunityIds.Count > TakeFromTopCount)
            {
                opportunityIds = opportunityIds.Take(TakeFromTopCount).ToList();
            }

            //List<int> filteredOpportunityIds;

            //if (opportunityIds.Count <= NumberOfOpportunitiesToInclude)
            //{
            //    filteredOpportunityIds = opportunityIds;
            //}
            //else
            //{
            //    filteredOpportunityIds = new List<int>();
            //    Random random = new Random();
            //    while (filteredOpportunityIds.Count < NumberOfOpportunitiesToInclude)
            //    {
            //        int opportunityId = opportunityIds[random.Next(opportunityIds.Count)];
            //        if (!filteredOpportunityIds.Contains(opportunityId))
            //        {
            //            filteredOpportunityIds.Append(opportunityId);
            //        }
            //    }
            //}

            List<Opportunity> opportunities = await _context.Opportunities
                .Where(o => opportunityIds.Contains(o.Id)
                && o.Approved
                && o.OpportunityType == OpportunityType.Episodic
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
