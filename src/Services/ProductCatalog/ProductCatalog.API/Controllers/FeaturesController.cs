using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Infrastructure;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;

namespace ProductCatalog.API.Controllers
{
    [ApiController]
    //[ApiVersion("2.0-RC1", Deprecated = false)]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [SwaggerTag("Actions for handling features, permission sets and corresponding translations.")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        ///     Resolves all permission-nodes for a given organization.
        /// </summary>
        /// <remarks>
        ///     Resolves and returns all permission nodes for a given organization.
        /// </remarks>
        /// <param name="organizationId"> The organization you are retrieving permission-nodes for. </param>
        /// <returns> A list containing all permission-nodes for the given organization. </returns>
        [HttpGet("organization/{organizationId}/permissions")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
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

    }
}
