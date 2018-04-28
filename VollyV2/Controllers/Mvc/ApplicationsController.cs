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
                .ToListAsync();

            foreach (Application application in applications)
            {
                application.DateTime =
                    TimeZoneInfo.ConvertTimeFromUtc(application.DateTime, VollyConstants.TimeZoneInfo);
            }
            return View(applications);
        }
    }
}
