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
    [Route("api/Organizations")]
    public class OrganizationsApiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;

        public OrganizationsApiController(ApplicationDbContext context,
            IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        // GET: api/Organizations
        [HttpGet]
        public async Task<IEnumerable<Organization>> GetOrganizations()
        {
            return await GetAllOrganizations();
        }

        private async Task<List<Organization>> GetAllOrganizations()
        {
            return await _memoryCache.GetOrCreateAsync(VollyConstants.OrganizationCacheKey, entry =>
            {
                return _context.Organizations
                    .Include(o => o.Cause)
                    .Include(o => o.Location)
                    .ToListAsync();
            });
        }

        // GET: api/Organizations/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrganization([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var organization = (await GetAllOrganizations())
                .SingleOrDefault(m => m.Id == id);

            if (organization == null)
            {
                return NotFound();
            }

            return Ok(organization);
        }

        [HttpPost]
        [Route("/api/Organizations/Search")]
        public async Task<IActionResult> Search([FromBody]OrganizationSearch organizationSearch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<Organization> organizations = (await GetAllOrganizations())
                .Where(o => organizationSearch.CauseIds.Contains(0) ||
                o.Cause != null && organizationSearch.CauseIds.Contains(o.Cause.Id))
                             .ToList();

            return Ok(organizations);
        }

        //// PUT: api/Organizations/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutOrganization([FromRoute] int id, [FromBody] Organization organization)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != organization.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(organization).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!OrganizationExists(id))
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

        //// POST: api/Organizations
        //[HttpPost]
        //public async Task<IActionResult> PostOrganization([FromBody] Organization organization)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    _context.Organizations.Add(organization);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetOrganization", new { id = organization.Id }, organization);
        //}

        //// DELETE: api/Organizations/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteOrganization([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var organization = await _context.Organizations.SingleOrDefaultAsync(m => m.Id == id);
        //    if (organization == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Organizations.Remove(organization);
        //    await _context.SaveChangesAsync();

        //    return Ok(organization);
        //}

        private bool OrganizationExists(int id)
        {
            return _context.Organizations.Any(e => e.Id == id);
        }
    }
}