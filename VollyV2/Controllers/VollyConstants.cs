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
        public const string AliceEmail = "alicelam22@gmail.com";
        public const string VollyAdminEmail = "admin@vollyapp.com";
        public const string MoEmail = "moxue2017@gmail.com";

        public static DateTime ConvertFromUtc(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo);
        }
        public static DateTime ConvertToUtc(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(dateTime, TimeZoneInfo);
        }
    }
}
