using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VollyV2.Data;
using VollyV2.Data.Volly;
using VollyV2.Models;
using VollyV2.Models.Volly;

namespace VollyV2.Controllers.Mvc
{
    [Authorize]
    public class VolunteerHoursController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public VolunteerHoursController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            string userId = _userManager.GetUserId(User);

            List<Application> applications = await _context
                .Applications
                .Include(a => a.Opportunity)
                .ThenInclude(o => o.Organization)
                .Include(a => a.VolunteerHours)
                .Include(a => a.Occurrences)
                .ThenInclude(ao => ao.Occurrence)
                .AsNoTracking()
                .Where(a => a.UserId == userId)
                .ToListAsync();

            List<VolunteerHoursModel> otherVolunteerHours = _context.VolunteerHours
                .Where(v => v.Application == null && v.UserId == userId)
                .AsNoTracking().AsEnumerable()
                .Select(VolunteerHoursModel.FromVolunteerHours)
                .ToList();

            List<VolunteerHoursModel> models = applications
                .Select(VolunteerHoursModel.FromApplication)
                .ToList();


            models.AddRange(otherVolunteerHours);

            ViewData["VolunteerHours"] = models;
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            List<VolunteerHours> hours = await _context.VolunteerHours
                .AsNoTracking()
                .Include(a => a.User)
                .ThenInclude(u => u.Company)
                .Include(h => h.Application)
                .ThenInclude(a => a.Opportunity)
                .ToListAsync();

            ViewData["VolunteerHours"] = hours;
            return View();
        }

        [HttpPost]
        public IActionResult Index(VolunteerHoursModel model)
        {
            if (ModelState.IsValid)
            {
                model.CreateOrUpdate(_context, _userManager.GetUserId(User));
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteerHours = await _context.VolunteerHours
                .Include(o => o.Application)
                .ThenInclude(o => o.Opportunity)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (volunteerHours == null)
            {
                return NotFound();
            }

            return View(volunteerHours);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var volunteerHours = await _context.VolunteerHours.SingleOrDefaultAsync(m => m.Id == id);
            _context.VolunteerHours.Remove(volunteerHours);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}