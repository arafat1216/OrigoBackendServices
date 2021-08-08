using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;
using Common.Exceptions;
using Customer.API.ViewModels;
using CustomerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        [ProducesResponseType(typeof(ViewModels.Customer), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
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
        [ProducesResponseType(typeof(IEnumerable<ViewModels.Customer>), (int) HttpStatusCode.OK)]
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
        [ProducesResponseType(typeof(ViewModels.Customer), (int) HttpStatusCode.Created)]
        public async Task<ActionResult<ViewModels.Customer>> CreateCustomer([FromBody] NewCustomer customer)
        {
            var updatedCustomer = await _customerServices.AddCustomerAsync(customer.CompanyName, customer.OrgNumber,
                customer.CustomerContactPerson?.FullName, customer.CustomerContactPerson?.Email,
                customer.CustomerContactPerson?.PhoneNumber, customer.CompanyAddress?.Street,
                customer.CompanyAddress?.PostCode, customer.CompanyAddress?.City, customer.CompanyAddress?.Country);
            var updatedCustomerView = new ViewModels.Customer
            {
                Id = updatedCustomer.CustomerId,
                CompanyName = updatedCustomer.CompanyName,
                OrgNumber = updatedCustomer.OrgNumber,
                CompanyAddress = new Address(updatedCustomer.CompanyAddress),
                CustomerContactPerson = new ContactPerson(updatedCustomer.CustomerContactPerson)
            };

            return CreatedAtAction(nameof(CreateCustomer), new {id = updatedCustomerView.Id}, updatedCustomerView);
        }

        [Route("{customerId:Guid}/assetCategoryLifecycleTypes")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AssetCategoryLifecycleType>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<AssetCategoryLifecycleType>>>
            GetAllCustomerAssetCategoryLifecycleTypes(Guid customerId)
        {
            var assetCategoryLifecycleTypes =
                await _customerServices.GetAllAssetCategoryLifecycleTypesForCustomerAsync(customerId);
            if (assetCategoryLifecycleTypes == null) return NotFound();
            var customerAssetCategories = new List<AssetCategoryLifecycleType>();
            customerAssetCategories.AddRange(assetCategoryLifecycleTypes.Select(lifecycle =>
                new AssetCategoryLifecycleType
                {
                    CustomerId = lifecycle.CustomerId,
                    AssetCategoryId = lifecycle.AssetCategoryId,
                    LifecycleType = lifecycle.LifecycleType,
                    Name = Enum.GetName(typeof(LifecycleType), lifecycle.LifecycleType)
                }));
            return Ok(customerAssetCategories);
        }

        [Route("{customerId:Guid}/assetCategoryLifecycleTypes/{assetCategoryId:Guid}/add/{lifecycle:int}")]
        [HttpPost]
        [ProducesResponseType(typeof(AssetCategoryLifecycleType), (int) HttpStatusCode.Created)]
        public async Task<ActionResult<AssetCategoryLifecycleType>> AddCustomerAssetCategoryLifecycleType(
            Guid customerId, Guid assetCategoryId, int lifecycle)
        {
            try
            {
                // Check if given int is within valid range of values
                if (!Enum.IsDefined(typeof(LifecycleType), lifecycle))
                {
                    var arr = Enum.GetValues(typeof(LifecycleType));
                    var errorMessage = new StringBuilder(string.Format(
                        "The given value for lifecycle: {0} is out of bounds.\nValid options for lifecycle are:\n",
                        lifecycle));
                    foreach (LifecycleType e in arr) errorMessage.Append($"    -{(int) e} ({e})\n");
                    throw new InvalidLifecycleTypeException(errorMessage.ToString());
                }

                var newAssetCategoryLifecycleType =
                    await _customerServices.AddAssetCategoryLifecycleTypeForCustomerAsync(customerId, assetCategoryId,
                        lifecycle);
                var assetCategoryLifecycleTypeView = new AssetCategoryLifecycleType
                {
                    CustomerId = newAssetCategoryLifecycleType.CustomerId,
                    AssetCategoryId = newAssetCategoryLifecycleType.AssetCategoryId,
                    LifecycleType = newAssetCategoryLifecycleType.LifecycleType,
                    Name = Enum.GetName(typeof(LifecycleType), lifecycle)
                };

                return CreatedAtAction(nameof(AddCustomerAssetCategoryLifecycleType), assetCategoryLifecycleTypeView);
            }
            catch (InvalidLifecycleTypeException exception)
            {
                return UnprocessableEntity(exception.Message);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{customerId:Guid}/assetCategoryLifecycleTypes/{assetCategoryId:Guid}/remove/{lifecycle:int}")]
        [HttpPost]
        [ProducesResponseType(typeof(AssetCategoryLifecycleType), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<AssetCategoryLifecycleType>> RemoveCustomerAssetCategoryLifecycleType(
            Guid customerId, Guid assetCategoryId, int lifecycle)
        {
            try
            {
                // Check if given int is within valid range of values
                if (!Enum.IsDefined(typeof(LifecycleType), lifecycle))
                {
                    var arr = Enum.GetValues(typeof(LifecycleType));
                    var errorMessage = new StringBuilder(string.Format(
                        "The given value for lifecycle: {0} is out of bounds.\nValid options for lifecycle are:\n",
                        lifecycle));
                    foreach (LifecycleType e in arr) errorMessage.Append($"    -{(int) e} ({e})\n");
                    throw new InvalidLifecycleTypeException(errorMessage.ToString());
                }

                var deletedAssetCategoryLifecycle =
                    await _customerServices.RemoveAssetCategoryLifecycleTypeForCustomerAsync(customerId,
                        assetCategoryId, lifecycle);
                var assetCategoryLifecycleTypeView = new AssetCategoryLifecycleType
                {
                    CustomerId = deletedAssetCategoryLifecycle.CustomerId,
                    AssetCategoryId = deletedAssetCategoryLifecycle.AssetCategoryId,
                    Name = Enum.GetName(typeof(LifecycleType), deletedAssetCategoryLifecycle.LifecycleType),
                    LifecycleType = deletedAssetCategoryLifecycle.LifecycleType
                };

                return Ok(assetCategoryLifecycleTypeView);
            }
            catch (InvalidLifecycleTypeException exception)
            {
                return UnprocessableEntity(exception.Message);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{customerId:Guid}/assetCategories")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AssetCategoryType>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<AssetCategoryType>>> GetAllCustomerAssetCategory(Guid customerId)
        {
            var assetCategoryLifecycleTypes = await _customerServices.GetAssetCategoryTypes(customerId);
            if (assetCategoryLifecycleTypes == null) return NotFound();
            var customerAssetCategories = new List<AssetCategoryType>();
            customerAssetCategories.AddRange(assetCategoryLifecycleTypes.Select(category => new AssetCategoryType
            {
                CustomerId = category.ExternalCustomerId,
                AssetCategoryId = category.AssetCategoryId,
                LifecycleTypes = new List<AssetCategoryLifecycleType>()
            }));
            return Ok(customerAssetCategories);
        }

        [Route("{customerId:Guid}/assetCategory/{assetCategoryId:Guid}/add")]
        [HttpPost]
        [ProducesResponseType(typeof(AssetCategoryType), (int) HttpStatusCode.Created)]
        public async Task<ActionResult<AssetCategoryType>> AddCustomerAssetCategory(Guid customerId,
            Guid assetCategoryId)
        {
            try
            {
                var assetCategories = await _customerServices.AddAssetCategoryType(customerId, assetCategoryId);
                var assetCategory = new AssetCategoryType
                {
                    CustomerId = assetCategories.ExternalCustomerId,
                    AssetCategoryId = assetCategories.AssetCategoryId,
                    LifecycleTypes = new List<AssetCategoryLifecycleType>()
                };

                return Ok(assetCategory);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{customerId:Guid}/assetCategory/{assetCategoryId:Guid}/remove")]
        [HttpPost]
        [ProducesResponseType(typeof(AssetCategoryType), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<AssetCategoryType>> RemoveCustomerAssetCategory(Guid customerId,
            Guid assetCategoryId)
        {
            try
            {
                var assetCategories = await _customerServices.RemoveAssetCategoryType(customerId, assetCategoryId);
                var assetCategory = new AssetCategoryType
                {
                    CustomerId = assetCategories.ExternalCustomerId,
                    AssetCategoryId = assetCategories.AssetCategoryId,
                    LifecycleTypes = new List<AssetCategoryLifecycleType>()
                };

                return Ok(assetCategory);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{customerId:Guid}/modules/groups")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<ProductModuleGroup>), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<IList<ProductModuleGroup>>> GetCustomerProductModuleGroups(Guid customerId)
        {
            var productGroup = await _customerServices.GetCustomerProductModuleGroupsAsync(customerId);
            if (productGroup == null) return NotFound();
            var customerProductModules = new List<ProductModuleGroup>();
            customerProductModules.AddRange(productGroup.Select(module => new ProductModuleGroup
            {
                ProductModuleGroupId = module.ProductModuleGroupId, Name = module.Name
            }));

            return Ok(customerProductModules);
        }

        [Route("{customerId:Guid}/modules/groups/{moduleGroupId:Guid}/add")]
        [HttpPost]
        [ProducesResponseType(typeof(ProductModuleGroup), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<ProductModuleGroup>> AddCustomerProductModuleGroups(Guid customerId,
            Guid moduleGroupId)
        {
            var productGroup = await _customerServices.AddProductModuleGroupsAsync(customerId, moduleGroupId);
            if (productGroup == null) return NotFound();
            var moduleGroup = new ProductModuleGroup
            {
                ProductModuleGroupId = productGroup.ProductModuleGroupId, Name = productGroup.Name
            };

            return Ok(moduleGroup);
        }

        [Route("{customerId:Guid}/modules/groups/{moduleGroupId:Guid}/remove")]
        [HttpPost]
        [ProducesResponseType(typeof(ProductModuleGroup), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<ProductModuleGroup>> RemoveCustomerProductModuleGroups(Guid customerId,
            Guid moduleGroupId)
        {
            var productGroup = await _customerServices.RemoveProductModuleGroupsAsync(customerId, moduleGroupId);
            if (productGroup == null) return NotFound();
            var moduleGroup = new ProductModuleGroup
            {
                ProductModuleGroupId = productGroup.ProductModuleGroupId, Name = productGroup.Name
            };

            return Ok(moduleGroup);
        }

        [Route("{customerId:Guid}/modules")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<ProductModule>), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<IList<ProductModule>>> GetCustomerProductModules(Guid customerId)
        {
            var productGroup = await _customerServices.GetCustomerProductModulesAsync(customerId);
            if (productGroup == null) return NotFound();
            var customerProductModules = new List<ProductModule>();
            customerProductModules.AddRange(productGroup.Select(module => new ProductModule
            {
                ProductModuleId = module.ProductModuleId,
                Name = module.Name,
                ProductModuleGroup = module.ProductModuleGroup.Select(groups => new ProductModuleGroup
                {
                    Name = groups.Name, ProductModuleGroupId = groups.ProductModuleGroupId
                }).ToList()
            }));

            return Ok(customerProductModules);
        }

        [Route("{customerId:Guid}/modules/{moduleId:Guid}/add")]
        [HttpPost]
        [ProducesResponseType(typeof(ProductModule), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<ProductModule>> AddCustomerProductModules(Guid customerId, Guid moduleId)
        {
            var productGroup = await _customerServices.AddProductModulesAsync(customerId, moduleId);
            if (productGroup == null) return NotFound();
            var moduleGroup = new ProductModule
            {
                ProductModuleId = productGroup.ProductModuleId,
                Name = productGroup.Name,
                ProductModuleGroup = productGroup.ProductModuleGroup.Select(groups => new ProductModuleGroup
                {
                    Name = groups.Name, ProductModuleGroupId = groups.ProductModuleGroupId
                }).ToList()
            };

            return Ok(moduleGroup);
        }

        [Route("{customerId:Guid}/modules/{moduleId:Guid}/remove")]
        [HttpPost]
        [ProducesResponseType(typeof(ProductModule), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<ProductModule>> RemoveCustomerModules(Guid customerId, Guid moduleId)
        {
            var productGroup = await _customerServices.RemoveProductModulesAsync(customerId, moduleId);
            if (productGroup == null) return NotFound();
            var moduleGroup = new ProductModule
            {
                ProductModuleId = productGroup.ProductModuleId,
                Name = productGroup.Name,
                ProductModuleGroup = productGroup.ProductModuleGroup.Select(groups => new ProductModuleGroup
                {
                    Name = groups.Name, ProductModuleGroupId = groups.ProductModuleGroupId
                }).ToList()
            };

            return Ok(moduleGroup);
        }
    }
}