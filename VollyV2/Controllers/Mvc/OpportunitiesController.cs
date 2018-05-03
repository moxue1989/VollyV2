using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using VollyV2.Data;
using VollyV2.Data.Volly;
using VollyV2.Models;
using VollyV2.Models.Volly;
using VollyV2.Services;

namespace VollyV2.Controllers.Mvc
{
    [Authorize(Roles = "Admin")]
    public class OpportunitiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OpportunitiesController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IAuthorizationService authorizationService)
        {
            _context = context;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        // GET: Opportunities
        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await GetCurrentUser();
            bool isAdmin = await IsAdmin(user);
            IIncludableQueryable<Opportunity, Organization> opportunitiesQueryable = _context.Opportunities
                .Include(o => o.Category)
                .Include(o => o.Organization);

            List<Opportunity> opportunities;
            if (isAdmin)
            {
                opportunities = await opportunitiesQueryable
                    .ToListAsync();
            }
            else
            {
                opportunities = await opportunitiesQueryable
                    .Where(o => o.CreatedByUserId == user.Id)
                    .ToListAsync();
            }

            opportunities.ForEach(a => a.DateTime = TimeZoneInfo.ConvertTimeFromUtc(a.DateTime, VollyConstants.TimeZoneInfo));

            return View(opportunities);
        }

        private async Task<bool> IsAdmin(ApplicationUser user)
        {
            return await _userManager.IsInRoleAsync(user, "Admin");
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            return user;
        }

        // GET: Opportunities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opportunity = await _context.Opportunities
                .Include(o => o.Category)
                .Include(o => o.Organization)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (opportunity == null)
            {
                return NotFound();
            }

            AuthorizationResult authorizationResult = await _authorizationService
                .AuthorizeAsync(User, opportunity, new OpportunityCreatorRequirement());

            if (!authorizationResult.Succeeded)
            {
                return new ForbidResult();
            }
            opportunity.DateTime =
                TimeZoneInfo.ConvertTimeFromUtc(opportunity.DateTime, VollyConstants.TimeZoneInfo);

            return View(opportunity);
        }

        // GET: Opportunities/Create
        public IActionResult Create()
        {
            OpportunityModel model = new OpportunityModel
            {
                Categories = new SelectList(_context.Categories
                .OrderBy(c => c.Name)
                .ToList(), "Id", "Name"),
                Organizations = new SelectList(_context.Organizations
                .OrderBy(o => o.Name)
                .ToList(), "Id", "Name")
            };
            return View(model);
        }

        // POST: Opportunities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Address,DateTime,EndDateTime,ApplicationDeadline,Openings,FamilyFriendly,CategoryId,OrganizationId,ImageUrl")] OpportunityModel model)
        {
            if (ModelState.IsValid)
            {
                Opportunity opportunity = model.GetOpportunity(_context);
                opportunity.CreatedByUser = await GetCurrentUser();
                _context.Add(opportunity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Opportunities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opportunity = await _context.Opportunities
                .Include(o => o.Category)
                .Include(o => o.Organization)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (opportunity == null)
            {
                return NotFound();
            }

            AuthorizationResult authorizationResult = await _authorizationService
                .AuthorizeAsync(User, opportunity, new OpportunityCreatorRequirement());

            if (!authorizationResult.Succeeded)
            {
                return new ForbidResult();
            }

            return View(OpportunityModel.FromOpportunity(_context, opportunity));
        }

        // POST: Opportunities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Address,DateTime,EndDateTime,ApplicationDeadline,Openings,FamilyFriendly,CategoryId,OrganizationId,ImageUrl")] OpportunityModel opportunityModel)
        {
            if (id != opportunityModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Opportunity opportunity = opportunityModel.GetOpportunity(_context);
                    AuthorizationResult authorizationResult = await _authorizationService
                        .AuthorizeAsync(User, opportunity, new OpportunityCreatorRequirement());

                    if (!authorizationResult.Succeeded)
                    {
                        return new ForbidResult();
                    }
                    _context.Update(opportunity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OpportunityExists(opportunityModel.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(opportunityModel);
        }

        // GET: Opportunities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opportunity = await _context.Opportunities
                .Include(o => o.Category)
                .Include(o => o.Organization)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (opportunity == null)
            {
                return NotFound();
            }
            AuthorizationResult authorizationResult = await _authorizationService
                .AuthorizeAsync(User, opportunity, new OpportunityCreatorRequirement());

            if (!authorizationResult.Succeeded)
            {
                return new ForbidResult();
            }
            opportunity.DateTime = TimeZoneInfo.ConvertTimeFromUtc(opportunity.DateTime, VollyConstants.TimeZoneInfo);

            return View(opportunity);
        }

        // POST: Opportunities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var opportunity = await _context.Opportunities.SingleOrDefaultAsync(m => m.Id == id);
            AuthorizationResult authorizationResult = await _authorizationService
                .AuthorizeAsync(User, opportunity, new OpportunityCreatorRequirement());

            if (!authorizationResult.Succeeded)
            {
                return new ForbidResult();
            }
            _context.Opportunities.Remove(opportunity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OpportunityExists(int id)
        {
            return _context.Opportunities.Any(e => e.Id == id);
        }
    }
}
