using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VollyV2.Data;
using VollyV2.Data.Volly;
using VollyV2.Models;
using VollyV2.Models.CSVModels;
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

        public IActionResult Index()
        {
            string userId = _userManager.GetUserId(User);

            List<VolunteerHoursModel> models = _context.VolunteerHours
                .Include(a => a.User)
                .Where(v => v.UserId == userId)
                .AsNoTracking().AsEnumerable()
                .Select(VolunteerHoursModel.FromVolunteerHours)
                .ToList();

            ViewData["VolunteerHours"] = models;
            return View();
        }
       
        [Authorize(Roles = "Admin")]
        public IActionResult Export()
        {
            List<VolunteerHoursCSVModel> hours = _context.VolunteerHours
                .AsNoTracking()
                .Include(a => a.User)
                .ThenInclude(u => u.Company)
                .AsNoTracking().AsEnumerable()
                .Select(VolunteerHoursCSVModel.FromVolunteerHours)
                .ToList();

            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            CsvWriter csvWriter = new CsvWriter(writer);

            csvWriter.Configuration.RegisterClassMap<VolunteerHoursCSVModelMap>();
            csvWriter.WriteRecords(hours);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            return File(
                stream,
                "application/octet-stream",
                String.Format("VolunteerHours_{0}.csv", DateTime.Today.ToShortDateString()));
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

        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            List<VolunteerHoursModel> hours = _context.VolunteerHours
                .AsNoTracking()
                .Include(a => a.User)
                .ThenInclude(u => u.Company)
                .AsNoTracking().AsEnumerable()
                .Select(VolunteerHoursModel.FromVolunteerHours)
                .ToList();

            ViewData["VolunteerHours"] = hours;
            ViewData["Users"] = new SelectList(_context.ApplicationUser.ToList(),
                "Id", "Email");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Admin(VolunteerHoursModel model)
        {
            if (ModelState.IsValid)
            {
                model.CreateOrUpdate(_context, model.UserId);
            }
            return RedirectToAction("Admin");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteerHours = await _context.VolunteerHours
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