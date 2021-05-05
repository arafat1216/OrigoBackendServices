using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Customer.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ILogger<CustomerController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<CustomerServices.Models.Customer> Get()
        {
            return null;
        }
    }
}
