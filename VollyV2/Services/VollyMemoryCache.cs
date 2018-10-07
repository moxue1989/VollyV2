using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VollyV2.Controllers;
using VollyV2.Data;
using VollyV2.Data.Volly;

namespace VollyV2.Services
{
    public class VollyMemoryCache
    {
        public static async Task<List<Opportunity>> GetAllOpportunities(IMemoryCache memoryCache, ApplicationDbContext context)
        {
            return await memoryCache.GetOrCreateAsync(VollyConstants.OpportunityCacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);

                List<int> opportunityIds = await context.Occurrences
                    .Where(o => o.ApplicationDeadline > DateTime.Now && o.Applications.Count < o.Openings)
                    .Select(o => o.OpportunityId)
                    .ToListAsync();

                List<Opportunity> opportunities = await context.Opportunities
                    .Where(o => opportunityIds.Contains(o.Id))
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
            });
        }
    }
}
