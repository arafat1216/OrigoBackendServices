using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CustomerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Customer.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly ICustomerServices _customerServices;

        public CustomerController(ILogger<CustomerController> logger, ICustomerServices customerServices)
        {
            _logger = logger;
            _customerServices = customerServices;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ViewModels.Customer>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<CustomerServices.Models.Customer>>> Get()
        {
            var customers = await _customerServices.GetCustomersAsync();
            var customerList = new List<ViewModels.Customer>();
            if (customers != null)
            {
                foreach (var customer in customers)
                {
                    customerList.Add(new ViewModels.Customer()
                    {

                    });
                }

            }
            return Ok(customerList);
        }
    }
}
