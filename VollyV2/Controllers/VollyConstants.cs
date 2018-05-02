using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV2.Controllers
{
    public static class VollyConstants
    {
        public static readonly TimeZoneInfo TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");
        public static readonly String OpportunityCacheKey = "OpportunityCache";
        public static readonly String OrganizationCacheKey = "OrganizationCache";
    }
}
