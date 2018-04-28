using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VollyV2.Models.Volly
{
    public class UserRoleModel
    {
        public SelectList Roles { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public IList<string> RoleNames { get; set; }
    }
}
