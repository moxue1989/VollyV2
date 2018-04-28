using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VollyV2.Data;
using VollyV2.Data.Volly;
using VollyV2.Models.Volly;

namespace VollyV2.Controllers.Mvc
{
    [Authorize(Roles = "Admin")]
    public class OpportunitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OpportunitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Opportunities
        public async Task<IActionResult> Index()
        {
            List<Opportunity> opportunities = await _context.Opportunities
                .Include(o => o.Category)
                .Include(o => o.Organization)
                .ToListAsync();

            opportunities.ForEach(a => a.DateTime = TimeZoneInfo.ConvertTimeFromUtc(a.DateTime, VollyConstants.TimeZoneInfo));
        
            return View(opportunities);
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
            opportunity.DateTime = TimeZoneInfo.ConvertTimeFromUtc(opportunity.DateTime, VollyConstants.TimeZoneInfo);

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
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Address,DateTime,CategoryId,OrganizationId,ImageUrl")] OpportunityModel model)
        {
            if (ModelState.IsValid)
            {
                Opportunity opportunity = model.GetOpportunity(_context);
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

            return View(OpportunityModel.FromOpportunity(_context, opportunity));
        }

        // POST: Opportunities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Address,DateTime,CategoryId,OrganizationId,ImageUrl")] OpportunityModel opportunityModel)
        {
            if (id != opportunityModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Opportunity opportunity = new Opportunity();
                try
                {
                    opportunity = opportunityModel.GetOpportunity(_context);
                    _context.Update(opportunity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OpportunityExists(opportunity.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
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
            opportunity.DateTime = TimeZoneInfo.ConvertTimeFromUtc(opportunity.DateTime, VollyConstants.TimeZoneInfo);

            return View(opportunity);
        }

        // POST: Opportunities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var opportunity = await _context.Opportunities.SingleOrDefaultAsync(m => m.Id == id);
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
