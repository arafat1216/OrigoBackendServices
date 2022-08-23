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


        [HttpGet("test")]
        public async Task<ActionResult> Test()
        {
            HardwareServiceOrderServices.Models.CustomerServiceProvider customerServiceProvider = new()
            {
                CustomerId = Guid.NewGuid(),

                ApiPassword = null,
                ApiUserName = null,
                LastUpdateFetched = DateTimeOffset.UtcNow,
                ServiceProviderId = 1,
            };

            _context.Add(customerServiceProvider);
            _context.SaveChanges();

            ApiCredential apiCredential = new(customerServiceProvider.Id, 1, "username", "password");
            _context.Add(apiCredential);
            _context.SaveChanges();

            var results = _context.CustomerServiceProviders
                .Include(e => e.ApiCredentials)
                .ToList();

            return Ok(results);
        }


        [HttpPost]
        public async Task<ActionResult> AddCredential([FromQuery] int servicetypeid, int custserviceproviderid, string? username, string? password)
        {
            ApiCredential apiCredential = new(custserviceproviderid, servicetypeid, username, password);

            _context.Add(apiCredential);
            _context.SaveChanges();

            return Ok();
        }


        [HttpDelete]
        public async Task<ActionResult> DeleteCredential([FromQuery] int servicetypeid, int custserviceproviderid)
        {
            var provider = _context.CustomerServiceProviders
                .Include(e => e.ApiCredentials)
                .FirstOrDefault();

            ApiCredential? removeditem = provider.ApiCredentials.FirstOrDefault(e => e.ServiceTypeId == servicetypeid && e.CustomerServiceProviderId == custserviceproviderid);

            if (removeditem is not null)
            {
                //provider.ApiCredentials.Remove(removeditem);
                _context.Remove(removeditem);
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }


        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var result = _context.CustomerServiceProviders.Include(e => e.ApiCredentials).ToList();
            return Ok(result);
        }
    }
}

#endif
