using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VollyV2.Data;
using VollyV2.Data.Volly;
using VollyV2.Models.Volly;
using VollyV2.Services;

namespace VollyV2.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Opportunities")]
    public class OpportunitiesApiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;

        public OpportunitiesApiController(ApplicationDbContext context,
            IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        // GET: api/Opportunities
        [HttpGet]
        public async Task<IActionResult> GetOpportunities()
        {
            List<Opportunity> opportunities = await VollyMemoryCache.GetAllOpportunities(_memoryCache, _context);
            foreach (var opportunity in opportunities)
            {
                opportunity.FOccurrences = opportunity.Occurrences;
            }
            return Ok(Sort(opportunities, 1));
        }

        // GET: api/Opportunities/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOpportunity([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Opportunity opportunity = (await VollyMemoryCache.GetAllOpportunities(_memoryCache, _context))
                .SingleOrDefault(m => m.Id == id);

            if (opportunity == null)
            {
                return NotFound();
            }

            return Ok(opportunity);
        }

        [HttpPost]
        [Route("/api/Opportunities/Search")]
        public async Task<IActionResult> Search([FromBody] OpportunitySearch opportunitySearch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<DateTime> dates = opportunitySearch
                .Dates
                .Select(datetime => datetime.Date)
                .ToList();

            List<Opportunity> opportunities = await VollyMemoryCache.GetAllOpportunities(_memoryCache, _context);
            foreach (var opportunity in opportunities)
            {
                if (dates.Count > 0)
                {
                    opportunity.FOccurrences = opportunity.Occurrences
                        .Where(oc => dates.Contains(oc.StartTime.Date))
                        .ToList();
                }
                else
                {
                    opportunity.FOccurrences = opportunity.Occurrences;
                }
            }

            opportunities = opportunities.Where(o =>
                    (opportunitySearch.CauseIds == null || o.Organization.Cause != null &&
                     opportunitySearch.CauseIds.Contains(o.Organization.Cause.Id)) &&
                    (opportunitySearch.CategoryIds == null ||
                     opportunitySearch.CategoryIds.Contains(o.Category.Id)) &&
                    (opportunitySearch.OrganizationIds == null ||
                     opportunitySearch.OrganizationIds.Contains(o.Organization.Id)) &&
                     o.FOccurrences.Count > 0)
                .ToList();

            return Ok(Sort(opportunities, opportunitySearch.Sort));
        }

        private List<Opportunity> Sort(List<Opportunity> opportunities, int sort)
        {
            switch (sort)
            {
                case 1:
                    return opportunities.OrderBy(o =>
                    {
                        Occurrence firstOcc = o.Occurrences[0];
                        return firstOcc.StartTime;
                    }).ToList();
                case 2:
                    return opportunities.OrderBy(o => o.Organization.Name).ToList();
                case 3:
                    return opportunities.OrderBy(o => o.Openings).ToList();
                case 4:
                    return opportunities.OrderBy(o =>
                    {
                        Occurrence firstOcc = o.Occurrences[0];
                        return firstOcc.EndTime - firstOcc.StartTime;
                    }).ToList();
                default:
                    return opportunities;
            }
        }

        private bool OpportunityExists(int id)
        {
            return _context.Opportunities.Any(e => e.Id == id);
        }

    }
}