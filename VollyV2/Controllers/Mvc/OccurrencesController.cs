using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VollyV2.Data;
using VollyV2.Data.Volly;

namespace VollyV2.Controllers.Mvc
{
    public class OccurrencesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OccurrencesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Occurrences
        public async Task<IActionResult> Index()
        {
            List<Occurrence> occurences = await _context
                .Occurrences
                .Include(o => o.Opportunity)
                .ToListAsync();
            
            return View(occurences.Select(OccurrenceTimeZoneConverter.ConvertFromUtc()));
        }

        // GET: Occurrences/Create
        public IActionResult Create()
        {
            ViewData["OpportunityId"] = new SelectList(_context.Opportunities, "Id", "Name");
            return View();
        }

        // POST: Occurrences/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StartTime,EndTime,ApplicationDeadline,Openings,OpportunityId")] Occurrence occurrence)
        {
            if (ModelState.IsValid)
            {
                Occurrence occurrenceToadd = OccurrenceTimeZoneConverter.ConvertToUtc().Invoke(occurrence);
                _context.Add(occurrenceToadd);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OpportunityId"] = new SelectList(_context.Opportunities, "Id", "Name", occurrence.OpportunityId);
            return View(occurrence);
        }

        // GET: Occurrences/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var occurrence = await _context.Occurrences.SingleOrDefaultAsync(m => m.Id == id);

            if (occurrence == null)
            {
                return NotFound();
            }
            ViewData["OpportunityId"] = new SelectList(_context.Opportunities, "Id", "Name", occurrence.OpportunityId);
            return View(OccurrenceTimeZoneConverter.ConvertFromUtc().Invoke(occurrence));
        }

        // POST: Occurrences/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartTime,EndTime,ApplicationDeadline,Openings,OpportunityId")] Occurrence occurrence)
        {
            if (id != occurrence.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(OccurrenceTimeZoneConverter.ConvertToUtc().Invoke(occurrence));
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OccurrenceExists(occurrence.Id))
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
            ViewData["OpportunityId"] = new SelectList(_context.Opportunities, "Id", "Name", occurrence.OpportunityId);
            return View(occurrence);
        }

        // GET: Occurrences/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var occurrence = await _context.Occurrences
                .Include(o => o.Opportunity)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (occurrence == null)
            {
                return NotFound();
            }

            return View(OccurrenceTimeZoneConverter.ConvertFromUtc().Invoke(occurrence));
        }

        // POST: Occurrences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var occurrence = await _context.Occurrences.SingleOrDefaultAsync(m => m.Id == id);
            _context.Occurrences.Remove(occurrence);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OccurrenceExists(int id)
        {
            return _context.Occurrences.Any(e => e.Id == id);
        }
    }
}
