using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HardwareRepair.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class HardwareRepairController : ControllerBase
    {
        [Route("{customerId:Guid}/settings")]
        [HttpPost]
        [SwaggerOperation(Tags = new[] { "Settings" })]
        public async Task<IActionResult> ConfigureSettings(Guid customerId)
        {
            return Ok();
        }

        [Route("{customerId:Guid}/settings")]
        [HttpGet]
        [SwaggerOperation(Tags = new[] { "Settings" })]
        public async Task<IActionResult> GetSettings(Guid customerId)
        {
            return Ok();
        }
    }
}
