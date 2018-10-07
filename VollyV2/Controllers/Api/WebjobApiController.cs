using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VollyV2.Data;
using VollyV2.Services;

namespace VollyV2.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/WebjobApi")]
    public class WebjobApiController : Controller
    {
        private ApplicationDbContext _dbContext;
        private IEmailSender _emailSender;

        public WebjobApiController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _dbContext = context;
            _emailSender = emailSender;
        }

        [HttpPost]
        public async Task<IActionResult> RunJob()
        {
            await _emailSender.SendEmailAsync("mo_xue1989@yahoo.ca", "TestWebJob", "web job started");
            return Ok();
        }
    }
}