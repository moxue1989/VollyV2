using System.Diagnostics;
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
            return RedirectToAction("UnderConstruction");
        }

        public IActionResult Index()
        {
            return RedirectToAction("UnderConstruction");
        }

        public IActionResult Opportunities(int Id = 1)
        {
            return RedirectToAction("UnderConstruction");
        }

        public IActionResult Track()
        {
            return RedirectToAction("UnderConstruction");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Track([Bind("Email,OpportunityName,OrganizationName,Date,Hours")]
            TrackHoursModel trackHoursModel)
        {
            return RedirectToAction("UnderConstruction");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply([Bind("ApplyModel")] MapModel mapModel)
        {
            return RedirectToAction("UnderConstruction");
        }

        private async Task<ApplicationUser> GetUser(string email)
        {
            bool isLoggedin = User.Identity.IsAuthenticated;
            return _dbContext.Users.FirstOrDefault(user => user.Email == email) ??
                   await new UserCreator(_userManager, _signInManager, _emailSender).CreateUser(email, null, !isLoggedin);
        }

        public IActionResult Contact()
        {
            return RedirectToAction("UnderConstruction");
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
            return RedirectToAction("UnderConstruction");
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
            return RedirectToAction("UnderConstruction");
        }

        public IActionResult Organizations()
        {
            return RedirectToAction("UnderConstruction");
        }

        public IActionResult Chat()
        {
            return RedirectToAction("UnderConstruction");
        }

        public IActionResult UnderConstruction()
        {
            return View();
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}