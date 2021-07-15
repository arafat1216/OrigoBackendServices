using Customer.API.ViewModels;
using CustomerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Customer.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerServices _customerServices;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ILogger<CustomersController> logger, ICustomerServices customerServices)
        {
            _logger = logger;
            _customerServices = customerServices;
        }

        [Route("{customerId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(ViewModels.Customer), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ViewModels.Customer>> Get(Guid customerId)
        {
            var customer = await _customerServices.GetCustomerAsync(customerId);
            if (customer == null) return NotFound();
            var foundCustomer = new ViewModels.Customer
            {
                Id = customer.CustomerId,
                CompanyName = customer.CompanyName,
                OrgNumber = customer.OrgNumber,
                CompanyAddress = new Address(customer.CompanyAddress),
                CustomerContactPerson = new ContactPerson(customer.CustomerContactPerson)
            };
            return Ok(foundCustomer);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ViewModels.Customer>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ViewModels.Customer>>> Get()
        {
            var customers = await _customerServices.GetCustomersAsync();
            var customerList = new List<ViewModels.Customer>();
            if (customers != null)
                customerList.AddRange(customers.Select(customer => new ViewModels.Customer
                {
                    Id = customer.CustomerId,
                    CompanyName = customer.CompanyName,
                    OrgNumber = customer.OrgNumber,
                    CompanyAddress = new Address(customer.CompanyAddress),
                    CustomerContactPerson = new ContactPerson(customer.CustomerContactPerson)
                }));

            return Ok(customerList);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ViewModels.Customer), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<ViewModels.Customer>> CreateCustomer([FromBody] NewCustomer customer)
        {
            var companyAddress = new CustomerServices.Models.Address(customer.CompanyAddress.Street,
                customer.CompanyAddress.PostCode, customer.CompanyAddress.City, customer.CompanyAddress.Country);
            var contactPerson = new CustomerServices.Models.ContactPerson(customer.CustomerContactPerson.FullName,
                customer.CustomerContactPerson.Email, customer.CustomerContactPerson.PhoneNumber);
            var newCustomer = new CustomerServices.Models.Customer(Guid.NewGuid(), customer.CompanyName,
                customer.OrgNumber, companyAddress, contactPerson);

            var updatedCustomer = await _customerServices.AddCustomerAsync(newCustomer);
            var updatedCustomerView = new ViewModels.Customer
            {
                Id = updatedCustomer.CustomerId,
                CompanyName = updatedCustomer.CompanyName,
                OrgNumber = updatedCustomer.OrgNumber,
                CompanyAddress = new Address(updatedCustomer.CompanyAddress),
                CustomerContactPerson = new ContactPerson(updatedCustomer.CustomerContactPerson)
            };

            return CreatedAtAction(nameof(CreateCustomer), new { id = updatedCustomerView.Id }, updatedCustomerView);
        }

        [Route("{customerId:Guid}/modules")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<ProductModuleGroup>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IList<ProductModuleGroup>>> AddCustomerModules(Guid customerId)
        {
            var productGroup = await _customerServices.GetCustomerProductModulesAsync(customerId);
            if (productGroup == null) return NotFound();
            var customerProductModules = new List<ProductModuleGroup>();
            customerProductModules.AddRange(productGroup.Select(module => new ProductModuleGroup() { ProductModuleGroupId = module.ProductModuleGroupId, Name = module.Name }));

            return Ok(customerProductModules);
        }

        [Route("{customerId:Guid}/modules/{moduleGroupId:Guid}/add")]
        [HttpPost]
        [ProducesResponseType(typeof(ProductModuleGroup), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ProductModuleGroup>> AddCustomerModules(Guid customerId, Guid moduleGroupId)
        {
            var productGroup = await _customerServices.AddProductModulesAsync(customerId, moduleGroupId);
            if (productGroup == null) return NotFound();
            ProductModuleGroup moduleGroup = new ProductModuleGroup()
            {
                ProductModuleGroupId = productGroup.ProductModuleGroupId,
                Name = productGroup.Name
            };

            return Ok(moduleGroup);
        }

        [Route("{customerId:Guid}/modules/{moduleGroupId:Guid}/remove")]
        [HttpPost]
        [ProducesResponseType(typeof(ProductModuleGroup), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ProductModuleGroup>> RemoveCustomerModules(Guid customerId, Guid moduleGroupId)
        {
            var productGroup = await _customerServices.RemoveProductModulesAsync(customerId, moduleGroupId);
            if (productGroup == null) return NotFound();
            ProductModuleGroup moduleGroup = new ProductModuleGroup()
            {
                ProductModuleGroupId = productGroup.ProductModuleGroupId,
                Name = productGroup.Name
            };

            return Ok(moduleGroup);
        }
    }
}