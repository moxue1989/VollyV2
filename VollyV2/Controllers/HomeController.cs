﻿using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        [TempData] public string Message { get; set; }

        public HomeController(ApplicationDbContext dbContext,
            IEmailSender emailSender,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _dbContext = dbContext;
            _emailSender = emailSender;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Index()
        {
            var images = _dbContext.OpportunityImages
                .AsNoTracking()
                .OrderByDescending(o => o.Id)
                .Take(10)
                .ToList();
            CarouselModel model = new CarouselModel() { opportunityImages = images };
            return View(model);
        }

        public IActionResult Opportunities(int Id = 1)
        {
            MapModel mapModel = new MapModel
            {
                CategoriesList = new SelectList(_dbContext.Categories
                    .OrderBy(c => c.Name)
                    .ToList(), "Id", "Name"),

                CausesList = new SelectList(_dbContext.Causes
                    .OrderBy(c => c.Name)
                    .ToList(), "Id", "Name"),

                OrganizationList = new SelectList(_dbContext.Organizations
                    .Where(o => o.Opportunities.Count > 0)
                    .OrderBy(c => c.Name)
                    .AsNoTracking()
                    .ToList(), "Id", "Name"),

                CommunityList = new SelectList(_dbContext.Communities
                    .OrderBy(c => c.Name)
                    .ToList(), "Id", "Name"),

                ApplyModel = new ApplyModel()
            };

            ViewData["OpportunityId"] = Id;
            return View(mapModel);
        }

        public IActionResult Track()
        {
            ViewData["Message"] = Message;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Track([Bind("Email,OpportunityName,OrganizationName,Date,Hours")]
            TrackHoursModel trackHoursModel)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = _userManager.Users.FirstOrDefault(u => u.Email == trackHoursModel.Email);
                if (user == null)
                {
                    ViewData["Error"] = "No Email Found";
                }
                else
                {
                    VolunteerHours volunteerHours = new VolunteerHours()
                    {
                        User = user,
                        OpportunityName = trackHoursModel.OpportunityName,
                        OrganizationName = trackHoursModel.OrganizationName,
                        DateTime = trackHoursModel.Date,
                        Hours = trackHoursModel.Hours
                    };
                    _dbContext.VolunteerHours.Add(volunteerHours);
                    await _dbContext.SaveChangesAsync();
                    Message = "Hours successfully submitted! Thank you!";
                    return RedirectToAction("Track");
                }
            }

            return View(trackHoursModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply([Bind("ApplyModel")] MapModel mapModel)
        {
            if (ModelState.IsValid)
            {
                ApplyModel applyModel = mapModel.ApplyModel;
                var user = await GetUser(applyModel.Email);
                ApplicationView application = await applyModel.GetApplication(_dbContext, user);
                await _emailSender.SendApplicationConfirmAsync(application);
                return Ok();
            }

            return Error();
        }

        private async Task<ApplicationUser> GetUser(string email)
        {
            bool isLoggedin = User.Identity.IsAuthenticated;
            return _dbContext.Users.FirstOrDefault(user => user.Email == email) ??
                   await new UserCreator(_userManager, _signInManager, _emailSender).CreateUser(email, null, !isLoggedin);
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = Message;
            return View();
        }

        private bool IsRecaptchaValid()
        {
            var result = false;
            var requestUri = string.Format(
                VollyConstants.RecaptchaPOSTUrl,
                VollyConstants.RecaptchaSecret,
                Request.Form["g-recaptcha-response"]
                );
            var request = (HttpWebRequest)WebRequest.Create(requestUri);

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    JObject jResponse = JObject.Parse(stream.ReadToEnd());
                    var isSuccess = jResponse.Value<bool>("success");
                    result = isSuccess ? true : false;
                }
            }
            return result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactUsModel contactUsModel)
        {
            if (ModelState.IsValid)
            {
                if (IsRecaptchaValid() && !contactUsModel.TripCheck)
                {
                    var ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                    await _emailSender.SendEmailsAsync(
                        VollyConstants.AllEmails,
                        "Message From: " + contactUsModel.Name,
                        contactUsModel.GetEmailMessage(ip));
                    TempData["Message"] = "Thank you, your message has been sent!";
                    return RedirectToAction(nameof(Contact));
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }

            }

            return View(contactUsModel);
        }

        public IActionResult Suggestion()
        {
            ViewData["Message"] = Message;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Suggestion([Bind("Name,Message")] SuggestionModel suggestionModel)
        {
            if (ModelState.IsValid)
            {
                var suggestionModelName = suggestionModel.Name ?? "annonomous";
                await _emailSender.SendEmailsAsync(VollyConstants.AllEmails, "Suggestion from " + suggestionModelName,
                    suggestionModel.GetEmailMessage());
                Message = "Thank you, your message has been sent!";
                return RedirectToAction("Suggestion");
            }

            return View(suggestionModel);
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