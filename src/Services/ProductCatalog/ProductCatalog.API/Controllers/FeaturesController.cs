using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Infrastructure;
using System.Text.Json;

namespace ProductCatalog.API.Controllers
{
    [ApiController]
    //[ApiVersion("2.0-RC1", Deprecated = false)]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class FeaturesController : ControllerBase
    {
        private readonly JsonSerializerOptions options = new JsonSerializerOptions()
        {
#if DEBUG
            WriteIndented = true,
#endif

            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
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


        /// <summary>
        /// this is a test
        /// </summary>
        /// <returns>some return here</returns>
        [HttpPost]
        public IActionResult test()
        {
            return Problem("details here", "instance", 55558, "type");
        }
    }
}
