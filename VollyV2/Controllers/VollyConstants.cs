using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV2.Controllers
{
    public static class VollyConstants
    {
        public static readonly TimeZoneInfo TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");
        public static readonly string OpportunityCacheKey = "OpportunityCache";
        public static readonly string OrganizationCacheKey = "OrganizationCache";
        public const string AliceEmail = "alicelam22@gmail.com";
        public const string VollyAdminEmail = "admin@vollyapp.com";
        public const string MoEmail = "moxue2017@gmail.com";
        public const string MarkEmail = "maillet.mark@gmail.com";
        public const string BearerSecret = "randomsecret12345678";
        public const string RecaptchaPOSTUrl = "https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}";
        public const string RecaptchaSecret = "6Lf7z4gUAAAAALLkw0Q8DNI_-yZ2Dkesvd8t-kni";

        public static readonly List<string> AllEmails = new List<String>()
        {
            AliceEmail,
            VollyAdminEmail,
            MoEmail,
            MarkEmail
        };

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
