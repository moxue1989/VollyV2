using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

        public OpportunitiesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Opportunities
        [HttpGet]
        public async Task<IActionResult> GetOpportunities()
        {
            var opportunities = await _context.Opportunities
                .Include(o => o.Category)
                .Include(o => o.Organization)
                .Include(o => o.Location)
                .ToListAsync();
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

            var opportunity = await _context.Opportunities
                .Include(o => o.Category)
                .Include(o => o.Organization)
                .ThenInclude(o => o.Cause)
                .Include(o => o.Location)
                .SingleOrDefaultAsync(m => m.Id == id);

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

            DateTime startDate = TimeZoneInfo.ConvertTimeToUtc(opportunitySearch.StartDate, VollyConstants.TimeZoneInfo);
            DateTime endDate = TimeZoneInfo.ConvertTimeToUtc(opportunitySearch.EndDate, VollyConstants.TimeZoneInfo);

            var opportunities = await _context.Opportunities
                .Include(o => o.Category)
                .Include(o => o.Organization)
                .ThenInclude(o => o.Cause)
                .Include(o => o.Location)
                .Where(o => (opportunitySearch.CauseIds.Contains(0) || opportunitySearch.CauseIds.Contains(o.Organization.Cause.Id)) &&
                (opportunitySearch.CategoryIds.Contains(0) || opportunitySearch.CategoryIds.Contains(o.Category.Id)) &&
                (opportunitySearch.StartDate == DateTime.MinValue || startDate.Date <= o.DateTime.Date) &&
                (opportunitySearch.EndDate == DateTime.MinValue || endDate.Date >= o.DateTime.Date))
                .ToListAsync();

            opportunities.ForEach(o => o.DateTime = TimeZoneInfo.ConvertTimeFromUtc(o.DateTime, VollyConstants.TimeZoneInfo));
            
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