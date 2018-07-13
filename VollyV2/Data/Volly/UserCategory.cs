using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VollyV2.Models;

namespace VollyV2.Data.Volly
{
    public class UserCause
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int CauseId { get; set; }
        public Cause Cause { get; set; }
    }
}
