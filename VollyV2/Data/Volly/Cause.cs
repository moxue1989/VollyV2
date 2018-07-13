using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV2.Data.Volly
{
    public class Cause
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<UserCause> Users { get; set; }
    }
}
