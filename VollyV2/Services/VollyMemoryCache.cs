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

                HashSet<int> availableOpportunities = context.Occurrences
                    .Where(o => o.Openings > o.Applications.Count)
                    .OrderBy(o => o.StartTime)
                    .AsNoTracking()
                    .Select(o => o.OpportunityId)
                    .ToHashSet();

                List<Opportunity> opportunities = await context.Opportunities
                    .Include(o => o.Category)
                    .Include(o => o.Organization)
                    .ThenInclude(o => o.Cause)
                    .Include(o => o.Location)
                    .Where(o => availableOpportunities.Contains(o.Id))
                    .AsNoTracking()
                    .ToListAsync();

                return opportunities;
            });
        }
    }
}
