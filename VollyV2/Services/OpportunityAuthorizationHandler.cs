using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using VollyV2.Data.Volly;
using VollyV2.Models;

namespace VollyV2.Services
{
    public class OpportunityAuthorizationHandler : AuthorizationHandler<OpportunityCreatorRequirement, Opportunity>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public OpportunityAuthorizationHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            OpportunityCreatorRequirement requirement,
            Opportunity opportunity)
        {
            ApplicationUser user = _userManager.GetUserAsync(context.User).Result;

            if (_userManager.IsInRoleAsync(user, "Admin").Result || user.Id == opportunity.CreatedByUserId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class OpportunityCreatorRequirement : IAuthorizationRequirement { }

}
