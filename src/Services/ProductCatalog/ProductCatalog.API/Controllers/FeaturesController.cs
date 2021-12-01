using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Infrastructure;
using System.Net;

namespace ProductCatalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeaturesController : ControllerBase
    {
        [HttpGet("organization/{organizationId}/permissions")]
        public async Task<ActionResult<IEnumerable<string>>> GetFeaturePermissionsByOrganizationAsync([FromRoute] Guid organizationId)
        {
            try
            {
                var result = await new FeatureService().GetPermissionNodesAsync(organizationId);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public IActionResult test()
        {
            return Problem("details here", "instance", 55558, "type");
        }
    }
}
