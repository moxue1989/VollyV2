using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VollyV2.Data;
using VollyV2.Data.Volly;
using VollyV2.Models.Volly;
using VollyV2.Services;
using Z.EntityFramework.Plus;

namespace VollyV2.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Apply")]
    public class ApplyApiController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmailSender _emailSender;

        public ApplyApiController(ApplicationDbContext context,
            IEmailSender emailSender)
        {
            _dbContext = context;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult GetModel()
        {
            ApplyModel applyModel = new ApplyModel()
            {
                OpportunityId = 1,
                Name = "first last",
                Email = "email@email.com",
                Message = "application message"
            };

            return Ok(applyModel);
        }

        [HttpPost]
        public async Task<IActionResult> Apply([FromBody]ApplyModel applyModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
                Application application = await applyModel.GetApplication(_dbContext);
                await _emailSender.SendApplicationConfirmAsync(application);
                return Ok(application);
        }
    }
}