using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using VollyV2.Data.Volly;

namespace VollyV2.Models.Volly
{
    public class PreferenceView
    {
        public List<Cause> Causes { get; set; }
    }
}
