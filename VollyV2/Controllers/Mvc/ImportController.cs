using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VollyV2.Controllers.Mvc
{
    [Authorize(Roles = "Admin")]
    public class ImportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ImportOrganizations(IFormFile postedFile)
        {
            TempData["message"] = "no file uploaded";
            return RedirectToAction("Index");
        }
    }
}