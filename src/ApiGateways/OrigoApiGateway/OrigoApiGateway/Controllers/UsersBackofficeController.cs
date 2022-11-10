#nullable enable

using Common.Enums;
using Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.Customer.Backend;
using OrigoApiGateway.Services;
using System.Security.Claims;

namespace OrigoApiGateway.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Tags("Users: Backoffice")]
    [Route("origoapi/v{version:apiVersion}/backoffice/users")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Returned when the user is not authenticated.")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Returned if the system encounter an unexpected problem.")]
    public class UsersBackofficeController : ControllerBase
    {
        private readonly IUserServices _userServices;


        public UsersBackofficeController(IUserServices userServices)
        {
            _userServices = userServices;
        }


        /// <summary>
        ///     Search for users.
        /// </summary>
        /// <remarks>
        /// A simple quick-search that looks through all users that is accessible to the caller, and retrieves the ones that matches
        /// the given criteria. The search is performed only applied to a few key-properties:
        /// <para>
        /// <b>Supported properties:</b>
        /// <list type="bullet">
        ///     <item>Name</item>
        ///     <item>E-mail</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="search"> The value to search for. </param>
        /// <param name="page"> The current page number. </param>
        /// <param name="limit"> The highest number of items that can be added in a single page. </param>
        /// <param name="cancellationToken"> A injected <see cref="CancellationToken"/>. </param>
        /// <param name="includeUserPreferences">
        ///     When <c><see langword="true"/></c>, information about the users preferences is loaded/included in the retrieved data. 
        ///     <para>This property will not be included unless it's explicitly requested. </para>
        /// </param>
        /// <param name="includeDepartmentInfo">
        ///     When <c><see langword="true"/></c>, the users department information is loaded/included in the retrieved data. 
        ///     <para>This property will not be included unless it's explicitly requested. </para>
        /// </param>
        /// <param name="includeOrganizationDetails">
        ///     When <c><see langword="true"/></c>, the users organization details is loaded/included in the retrieved data. 
        ///     <para>This property will not be included unless it's explicitly requested. </para>
        /// </param>
        /// <param name="includeRoleDetails">
        ///     When <c><see langword="true"/></c>, the users role details is loaded/included in the retrieved data. 
        ///     <para>This property will not be included unless it's explicitly requested. </para>
        /// </param>
        /// <returns> The asynchronous task. The task results contains the corresponding <see cref="ActionResult{TValue}"/>. </returns>
        [HttpGet]
        [Route("search/quicksearch")]
        [Authorize(Roles = "SystemAdmin,PartnerAdmin")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(PagedModel<OrigoUser>))]
        public async Task<ActionResult<PagedModel<OrigoUser>>> UserQuickSearch([FromQuery(Name = "q")]
                                                                               [Required][MinLength(3)] string search,
                                                                               CancellationToken cancellationToken,
                                                                               [FromQuery] int page = 1,
                                                                               [FromQuery][Range(1, 100)] int limit = 25,
                                                                               [FromQuery] bool includeUserPreferences = false,
                                                                               [FromQuery] bool includeDepartmentInfo = false,
                                                                               [FromQuery] bool includeOrganizationDetails = false,
                                                                               [FromQuery] bool includeRoleDetails = false)
        {
            UserSearchParameters searchParameters = new()
            {
                QuickSearch = search,
                QuickSearchSearchType = StringSearchType.Contains
            };

            // If it's a partner-admin, extract the organizations from their access list, and add it to the search-parameters
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == PredefinedRole.PartnerAdmin.ToString())
            {
                var customerList = new HashSet<Guid>();
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();

                foreach (var customerId in accessList)
                {
                    if (Guid.TryParse(customerId, out var customerIdGuid))
                    {
                        customerList.Add(customerIdGuid);
                    }
                }

                searchParameters.OrganizationIds = customerList;
            }

            var results = await _userServices.UserAdvancedSearch(searchParameters, page, limit, cancellationToken, includeUserPreferences: includeUserPreferences, includeDepartmentInfo: includeDepartmentInfo, includeOrganizationDetails: includeOrganizationDetails, includeRoleDetails: includeRoleDetails);
            return Ok(results);
        }
    }
}
