using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VollyV2.Data;
using VollyV2.Models.Volly;
using VollyV2.Services;

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
                PhoneNumber = "7771119999",
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
            ApplicationView application = await applyModel.GetApplication(_dbContext, null);
            await _emailSender.SendApplicationConfirmAsync(application);
            return Ok(application);
        }
    }
}