#nullable enable

using AutoMapper;
using Common.Enums;
using Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.Asset.Backend;
using OrigoApiGateway.Services;
using System.Security.Claims;

namespace OrigoApiGateway.Controllers
{
    /// <summary>
    ///     Backoffice administration APIs used for handling/configuring assets.
    /// </summary>
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("origoapi/v{version:apiVersion}/backoffice/assets")]
    [Tags("Assets: Backoffice")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Returned when the user is not authenticated.")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Returned if the system encounter an unexpected problem.")]
    public class AssetsBackofficeController : ControllerBase
    {
        private readonly IAssetServices _assetServices;
        private readonly IMapper _mapper;
        private readonly ILogger<AssetsBackofficeController> _logger;

        public AssetsBackofficeController(ILogger<AssetsBackofficeController> logger, IAssetServices assetServices, IMapper mapper)
        {
            _logger = logger;
            _assetServices = assetServices;
            _mapper = mapper;
        }


        /// <summary>
        ///     Search for assets.
        /// </summary>
        /// <remarks>
        /// A simple quick-search that looks through all <c>Assets</c> available for the user, and retrieves the ones that matches the given criteria.
        /// The search is performed only applied to a few key-properties:
        /// <para>
        /// <b>Supported properties:</b>
        /// <list type="bullet">
        ///     <item>IMEI</item>
        ///     <item>Serial number</item>
        ///     <item>Name of the contract-holder (the user)</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="search"> The value to search for. </param>
        /// <param name="page"> The current page number. </param>
        /// <param name="limit"> The highest number of items that can be added in a single page. </param>
        /// <param name="includeImeis">
        ///     When <c><see langword="true"/></c>, the <c>IMEI</c> property is loaded/included in the retrieved data. 
        ///     <para>This property will not be included unless it's explicitly requested. </para>
        /// </param>
        /// <param name="includeLabels">
        ///     When <c><see langword="true"/></c>, the <c>Labels</c> property is loaded/included in the retrieved data. 
        ///     <para>This property will not be included unless it's explicitly requested. </para>
        /// </param>
        /// <param name="includeContractHolderUser">
        ///     When <c><see langword="true"/></c>, information about the user is loaded/included in the retrieved data. 
        ///     <para>This property will not be included unless it's explicitly requested. </para>
        /// </param>
        /// <returns> The asynchronous task. The task results contains the corresponding <see cref="ActionResult{TValue}"/>. </returns>
        [Route("search/quicksearch")]
        [Authorize(Roles = "SystemAdmin,PartnerAdmin")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(PagedModel<HardwareSuperType>))]
        [HttpGet]
        public async Task<IActionResult> AssetQuickSearch([FromQuery(Name = "q")][Required][MinLength(3)] string search, [FromQuery] int page = 1, [FromQuery][Range(1, 100)] int limit = 25, [FromQuery] bool includeImeis = false, [FromQuery] bool includeLabels = false, [FromQuery] bool includeContractHolderUser = false)
        {
            AssetSearchParameters searchParameters = new()
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

                searchParameters.CustomerIds = customerList;
            }

            var results = await _assetServices.AssetAdvancedSearch(searchParameters, page, limit, includeImeis: includeImeis, includeLabels: includeLabels, includeContractHolderUser: includeContractHolderUser);
            return Ok(results);
        }
    }
}
