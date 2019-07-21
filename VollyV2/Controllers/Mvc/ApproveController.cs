using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VollyV2.Data;
using VollyV2.Data.Volly;
using VollyV2.Models.Views;

namespace VollyV2.Controllers.Mvc
{
    [Authorize(Roles = "PowerUser")]
    public class ApproveController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ApproveController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<OpportunityView> opportunityViews = _context.Opportunities
                .Include(o => o.Category)
                .Include(o => o.Community)
                .Include(o => o.Organization)
                .ThenInclude(o => o.Cause)
                .Include(o => o.Location)
                .Include(o => o.Occurrences)
                .ThenInclude(o => o.Applications)
                .AsNoTracking()
                .Where(o => !o.Approved)
                .Select(o => OpportunityView.FromOpportunity(o))
                .ToList();
            return View(opportunityViews);
        }

        public IActionResult Confirm(int id)
        {
            Opportunity opportunity = _context.Opportunities
                .Find(id);

            opportunity.Approved = true;

            _context.Opportunities.Update(opportunity);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}