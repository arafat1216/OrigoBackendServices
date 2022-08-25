#if DEBUG

using AutoMapper;
using HardwareServiceOrder.API.ViewModels;
using HardwareServiceOrderServices;
using HardwareServiceOrderServices.Infrastructure;
using HardwareServiceOrderServices.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace HardwareServiceOrder.API.Controllers
{
    /// <summary>
    ///     A temporary controller used for testing during development.
    ///     All data and methods in this controller is purely for local use, and can be freely removed if needed.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S2737:\"catch\" clauses should do more than rethrow", Justification = "<Pending>")]
    public class TestController : ControllerBase
    {
        // Dependency injections
        private readonly IProviderFactory _providerFactory;
        private readonly HardwareServiceOrderContext _context;
        private readonly IMapper _mapper;
        private readonly IApiRequesterService _apiRequesterService;


        public TestController(IProviderFactory providerFactory, HardwareServiceOrderContext dbContext, IMapper mapper, IApiRequesterService apiRequesterService)
        {
            _providerFactory = providerFactory;
            _context = dbContext;
            _mapper = mapper;
            _apiRequesterService = apiRequesterService;
        }


        // Please don't delete this one. It's a helper that's used for debugging when we need to fetch the injected caller-ID.
        private object GetResponseObject()
        {
            return new
            {
                AuthenticatedUserId = _apiRequesterService.AuthenticatedUserId
            };
        }


        [HttpGet("customer-service-provider")]
        public async Task<ActionResult> GetAllCustomerServiceProvidersAsync()
        {
            var results = await _context.CustomerServiceProviders
                                       .Include(e => e.ApiCredentials)
                                       .ToListAsync();

            var mapped = _mapper.Map<IEnumerable<CustomerServiceProviderDto>>(results);

            return Ok(mapped);
        }


    }
}

#endif
