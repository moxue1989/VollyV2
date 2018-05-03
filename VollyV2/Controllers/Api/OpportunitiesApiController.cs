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
            List<Opportunity> opportunities = await GetAllOpportunities();
            
            return Ok(opportunities);
        }

        private async Task<List<Opportunity>> GetAllOpportunities()
        {
            return await _memoryCache.GetOrCreateAsync(VollyConstants.OpportunityCacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);
                List<Opportunity> opportunities = await _context.Opportunities
                    .Include(o => o.Category)
                    .Include(o => o.Organization)
                    .ThenInclude(o => o.Cause)
                    .Include(o => o.Location)
                    .ToListAsync();
                foreach (var o in opportunities)
                {
                    o.DateTime = TimeZoneInfo.ConvertTimeFromUtc(o.DateTime, VollyConstants.TimeZoneInfo);
                    o.EndDateTime = TimeZoneInfo.ConvertTimeFromUtc(o.EndDateTime, VollyConstants.TimeZoneInfo);
                    o.ApplicationDeadline = TimeZoneInfo.ConvertTimeFromUtc(o.ApplicationDeadline, VollyConstants.TimeZoneInfo);
                }
                return opportunities;
            });
        }

        // GET: api/Opportunities/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOpportunity([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Opportunity opportunity = (await GetAllOpportunities())
                .SingleOrDefault(m => m.Id == id);

            if (opportunity == null)
            {
                return NotFound();
            }

            return Ok(opportunity);
        }

        [HttpPost]
        [Route("/api/Opportunities/Search")]
        public async Task<IActionResult> Search([FromBody]OpportunitySearch opportunitySearch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var opportunities = (await GetAllOpportunities())
                .Where(o => (opportunitySearch.CauseIds.Contains(0) || o.Organization.Cause != null && opportunitySearch.CauseIds.Contains(o.Organization.Cause.Id)) &&
                (opportunitySearch.CategoryIds.Contains(0) || opportunitySearch.CategoryIds.Contains(o.Category.Id)) &&
                (opportunitySearch.StartDate == DateTime.MinValue || opportunitySearch.StartDate.Date <= o.DateTime.Date) &&
                (opportunitySearch.EndDate == DateTime.MinValue || opportunitySearch.EndDate.Date >= o.DateTime.Date))
                .ToList();
            
            return Ok(opportunities);
        }

        //// PUT: api/Opportunities/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutOpportunity([FromRoute] int id, [FromBody] Opportunity opportunity)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != opportunity.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(opportunity).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!OpportunityExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/Opportunities
        //[HttpPost]
        //public async Task<IActionResult> PostOpportunity([FromBody] Opportunity opportunity)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    _context.Opportunities.Add(opportunity);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetOpportunity", new { id = opportunity.Id }, opportunity);
        //}

        //// DELETE: api/Opportunities/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteOpportunity([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var opportunity = await _context.Opportunities.SingleOrDefaultAsync(m => m.Id == id);
        //    if (opportunity == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Opportunities.Remove(opportunity);
        //    await _context.SaveChangesAsync();

        //    return Ok(opportunity);
        //}

        private bool OpportunityExists(int id)
        {
            return _context.Opportunities.Any(e => e.Id == id);
        }
    }
}