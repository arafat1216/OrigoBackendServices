using AutoMapper;
using Common.Infrastructure;
using Common.Interfaces;
using HardwareServiceOrder.API.Filters;
using HardwareServiceOrderServices;
using HardwareServiceOrderServices.Models;
using Microsoft.AspNetCore.Mvc;

namespace HardwareServiceOrder.API.Controllers
{
    /// <summary>
    ///     A controller that handles service-orders.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/")]
    [Tags("Service-orders")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Returned when the system encountered an unexpected problem.")]
    [ServiceFilter(typeof(ErrorExceptionFilter))]
    public class ServiceOrderController : ControllerBase
    {
        // Dependency injections
        private readonly IHardwareServiceOrderService _hardwareServiceOrderService;
        private readonly ILogger<ServiceOrderController> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        ///     The default constructor. This is called/handled directly by ASP.NET using DI (dependency injection).
        /// </summary>
        /// <param name="hardwareServiceOrderService"> The injected <see cref="IHardwareServiceOrderService"/> instance. </param>
        /// <param name="logger"> The injected <see cref="ILogger"/> instance. </param>
        /// <param name="mapper"> The injected <see cref="IMapper"/> (automapper) instance. </param>
        /// <param name="apiRequesterService"> A injected <see cref="IApiRequesterService"/> instance. 
        ///     This records relevant information from the incoming HTTP-request, and makes this available outside the controller.
        ///     It must always be included in the controller's constructor, even if it's not utilized. </param>
        public ServiceOrderController(IHardwareServiceOrderService hardwareServiceOrderService,
                                      ILogger<ServiceOrderController> logger,
                                      IMapper mapper)
        {
            _hardwareServiceOrderService = hardwareServiceOrderService;
            _logger = logger;
            _mapper = mapper;
        }


        /// <summary>
        ///     Retrieve a service-order by it's ID.
        /// </summary>
        /// <remarks>
        ///     Retrieves a service-order that matches a given ID.
        /// </remarks>
        /// <param name="organizationId"> The organization that owns the service-order. </param>
        /// <param name="serviceOrderId"> The service-order ID that should be retrieved. </param>
        /// <returns> A task containing the appropriate action-result. </returns>
        [HttpGet("organization/{organizationId:Guid}/orders/{serviceOrderId:Guid}")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(HardwareServiceOrderDTO))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Returned if the ID was not found.")]
        public async Task<IActionResult> GetServiceOrderByIdAndOrganizationAsync([FromRoute] Guid organizationId, [FromRoute] Guid serviceOrderId)
        {
            var result = await _hardwareServiceOrderService.GetServiceOrderByIdAsync(serviceOrderId, organizationId);

            if (result is null)
                return NotFound();

            return Ok(result);
        }


        /// <summary>
        ///     Retrieves a paginated list that contains a organization's service-orders.
        /// </summary>
        /// <param name="organizationId"> The organization ID to retrieve service-orders for. </param>
        /// <param name="userId"> When provided, filters the results to only contain this user. </param>
        /// <param name="serviceTypeId"> When provided, filters the results to only contain this service-type. </param>
        /// <param name="activeOnly"> 
        ///     When <c><see langword="true"/></c>, only active/ongoing service-orders are retrieved. When <c><see langword="false"/></c>, the filter is ignored. </param>
        /// <param name="cancellationToken"> A injected <see cref="CancellationToken"/>. </param>
        /// <param name="page"> The paginated page that should be retrieved. </param>
        /// <param name="limit"> The number of items to retrieve per <paramref name="page"/>. </param>
        /// <param name="assetLifecycleId"> Filter the results to only contain this asset-lifecycle. When the value is <see langword="null"/>, the filter is ignored. </param>
        /// <param name="statusIds"> Filter the results to include items with the selected statuses. When the value is <see langword="null"/>, the filter is ignored. </param>
        /// <param name="search"> If a value is provided, a lightweight "contains" search is applied to a few select key-properties. </param>
        /// <returns> A task containing the appropriate action-result. </returns>
        [HttpGet("organization/{organizationId:Guid}/orders")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(PagedModel<HardwareServiceOrderDTO>))]
        public async Task<ActionResult> GetAllServiceOrdersForOrganizationAsync([FromRoute] Guid organizationId, [FromQuery] Guid? userId, [FromQuery][EnumDataType(typeof(ServiceTypeEnum))] int? serviceTypeId, CancellationToken cancellationToken, [FromQuery] bool activeOnly = false, [FromQuery] int page = 1, [FromQuery] int limit = 25, [FromQuery] Guid? assetLifecycleId = null, [FromQuery] HashSet<int>? statusIds = null, [FromQuery(Name = "q")] string? search = null)
        {
            PagedModel<HardwareServiceOrderDTO> results = await _hardwareServiceOrderService.GetAllServiceOrdersForOrganizationAsync(organizationId, userId, serviceTypeId, activeOnly, cancellationToken, page, limit, assetLifecycleId: assetLifecycleId, statusIds: statusIds, search: search);

            return Ok(results);
        }


    }
}
