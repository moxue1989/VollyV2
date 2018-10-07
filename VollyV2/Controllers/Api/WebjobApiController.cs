using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VollyV2.Data;
using VollyV2.Data.Volly;
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
           await _emailSender.SendEmailsAsync(new List<string>()
            {
                VollyConstants.AliceEmail,
                VollyConstants.MoEmail
            }, "Reminder Web job", "web job started");
           
            List<Occurrence> occurrences = _dbContext.Occurrences
                .Include(o => o.Applications)
                .ThenInclude(ao => ao.Application)
                .Include(o => o.Opportunity)
                .Where(delegate(Occurrence o)
                {
                    DateTime startTime = o.StartTime;
                    return startTime > DateTime.Now && startTime < DateTime.Now.AddDays(1);
                })
                .ToList();

            foreach (Occurrence occurrence in occurrences)
            {
                //                List<string> emailsForOccurrence = occurrence.Applications
                //                    .Select(a => a.Application.Email)
                //                    .ToList();

                List<string> emailsForOccurrence = new List<string>()
                {
                    VollyConstants.MoEmail,
                    VollyConstants.AliceEmail
                };

                await _emailSender.SendRemindersAsync(emailsForOccurrence, occurrence);
            }

            return Ok();
        }
    }
}