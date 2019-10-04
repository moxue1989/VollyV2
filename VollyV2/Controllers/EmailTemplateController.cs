using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VollyV2.Extensions;

namespace VollyV2.Controllers
{
    public class EmailTemplateController : Controller
    {
        public IActionResult Newsletter()
        {
            return View();
        }
    }
}