using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VollyV2.Data;
using VollyV2.Data.Volly;

namespace VollyV2.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Occurrences")]
    public class OccurrencesApiController : Controller
    {

        private readonly ApplicationDbContext _context;
        public OccurrencesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> GetOccurrences(int opportunityId)
        {
            List<Occurrence> occurrences = await _context
                .Occurrences
                .Where(o => o.OpportunityId == opportunityId).ToListAsync();

            return Ok(occurrences);
        }
    }
}