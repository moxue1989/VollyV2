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
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApplicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Applications
        public async Task<IActionResult> Index()
        {
            IEnumerable<Application> applications = await _context.Applications
                .Include(a => a.Opportunity)
                .Include(a => a.Occurrences)
                .ThenInclude(o => o.Occurrence)
                .Include(a => a.User)
                .AsNoTracking()
                .ToListAsync();

            IEnumerable<ApplicationView> applicationViews = applications
                .Select(ApplicationView.FromApplication);
            return View(applicationViews);
        }

        // GET: Applications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _context.Applications
                .Include(a => a.Opportunity)
                .Include(a => a.Occurrences)
                .ThenInclude(o => o.Occurrence)
                .Include(a => a.User)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.Id == id);
            if (application == null)
            {
                return NotFound();
            }

            return View(ApplicationView.FromApplication(application));
        }

        // POST: Applications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var application = await _context.Applications.SingleOrDefaultAsync(m => m.Id == id);
            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
