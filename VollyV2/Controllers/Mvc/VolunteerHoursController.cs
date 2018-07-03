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
                .Where(v => v.Application == null)
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

        [HttpPost]
        public IActionResult Index(VolunteerHoursModel model)
        {
            if (ModelState.IsValid)
            {
                model.CreateOrUpdate(_context, _userManager.GetUserId(User));
            }
            return RedirectToAction("Index");
        }
    }
}