using AutoMapper;
using Common.Infrastructure;
using HardwareServiceOrderServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HardwareServiceOrder.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/orders")]
    [Tags("Service-orders")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Returned when the system encountered an unexpected problem.")]
    public class ServiceOrdersController : ControllerBase
    {
        private readonly IHardwareServiceOrderService _hardwareServiceOrderService;
        private readonly ILogger<ServiceOrdersController> _logger;
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
        public ServiceOrdersController(IHardwareServiceOrderService hardwareServiceOrderService,
                                       ILogger<ServiceOrdersController> logger,
                                       IMapper mapper,
                                       IApiRequesterService apiRequesterService)
        {
            _hardwareServiceOrderService = hardwareServiceOrderService;
            _logger = logger;
            _mapper = mapper;
        }


        [HttpGet("organization/{organizationId:Guid}")]
        public async Task<ActionResult> GetAllServiceOrders([FromRoute] int organizationId, [FromQuery] int serviceTypeId)
        {
            throw new NotImplementedException();
        }
    }
}
