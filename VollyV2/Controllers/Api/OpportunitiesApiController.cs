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
            
            return Ok(opportunities);
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
        public async Task<IActionResult> Search([FromBody]OpportunitySearch opportunitySearch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<DateTime> dates = opportunitySearch
                .Dates
                .Select(datetime => datetime.Date)
                .ToList();

            var opportunities = (await VollyMemoryCache.GetAllOpportunities(_memoryCache, _context))
                .Where(o => (opportunitySearch.CauseIds == null || o.Organization.Cause != null && opportunitySearch.CauseIds.Contains(o.Organization.Cause.Id)) &&
                (opportunitySearch.CategoryIds == null || opportunitySearch.CategoryIds.Contains(o.Category.Id)) &&
                (opportunitySearch.OrganizationIds == null || opportunitySearch.OrganizationIds.Contains(o.Organization.Id)) &&
                (dates.Count == 0 || dates.Contains(o.DateTime.Date)))
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