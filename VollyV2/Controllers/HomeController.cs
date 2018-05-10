using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VollyV2.Data;
using VollyV2.Data.Volly;
using VollyV2.Models;
using VollyV2.Models.Volly;
using VollyV2.Services;

namespace VollyV2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmailSender _emailSender;

        public HomeController(ApplicationDbContext dbContext, 
            IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Opportunities()
        {
            MapModel mapModel = new MapModel
            {
                CategoriesList = new SelectList(_dbContext.Categories
                    .OrderBy(c => c.Name)
                    .ToList(), "Id", "Name"),

                CausesList = new SelectList(_dbContext.Causes
                    .OrderBy(c => c.Name)
                    .ToList(), "Id", "Name"),

                ApplyModel = new ApplyModel()
            };

            return View(mapModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply([Bind("ApplyModel")] MapModel mapModel)
        {
            if (ModelState.IsValid)
            {
                ApplyModel applyModel = mapModel.ApplyModel;
                Application application = applyModel.GetApplication(_dbContext);
                _dbContext.Applications.Add(application);
                await _dbContext.SaveChangesAsync();
                await _emailSender.SendApplicationConfirmAsync(application);
                return Ok();
            }
            return Error();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = TempData["Message"];
            TempData.Clear();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact([Bind("Name,Email,Message")] ContactUsModel contactUsModel)
        {
            if (ModelState.IsValid)
            {
                List<string> emails = new List<String>()
                {
                    VollyConstants.AliceEmail,
                    VollyConstants.VollyAdminEmail,
                    VollyConstants.MoEmail
                };
                await _emailSender.SendEmailsAsync(emails, "Message From: " + contactUsModel.Name, contactUsModel.GetEmailMessage());
                TempData["Message"] = "Thank you, your message has been sent!";
                return RedirectToAction("Contact");
            }
            return View(contactUsModel);
        }

        public IActionResult Organizations()
        {
            MapModel mapModel = new MapModel
            {
                CausesList = new SelectList(_dbContext.Causes
                    .OrderBy(c => c.Name)
                    .ToList(), "Id", "Name")
            };
            return View(mapModel);
        }

        public IActionResult Chat()
        {
            return View();
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
