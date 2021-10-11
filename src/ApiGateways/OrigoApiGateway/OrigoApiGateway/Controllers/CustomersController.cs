using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrigoApiGateway.Models;
using OrigoApiGateway.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using OrigoApiGateway.Authorization;
using System.Linq;
using System.Security.Claims;

// ReSharper disable RouteTemplates.RouteParameterConstraintNotResolved

namespace OrigoApiGateway.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    //[Authorize(AuthenticationSchemes = OktaDefaults.ApiAuthenticationScheme)]
    [Route("origoapi/v{version:apiVersion}/[controller]")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class CustomersController : ControllerBase
    {
        private ILogger<CustomersController> Logger { get; }
        private ICustomerServices CustomerServices { get; }

        public CustomersController(ILogger<CustomersController> logger, ICustomerServices customerServices)
        {
            Logger = logger;
            CustomerServices = customerServices;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<Organization>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        //[PermissionAuthorize(Permission.CanReadCustomer)]
        //[Authorize(Roles = "GroupAdmin,PartnerAdmin")]
        //[PermissionAuthorize(PermissionOperator.And, Permission.CanCreateCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<IList<Organization>>> Get()
        {
            try
            {
                var customers = await CustomerServices.GetCustomersAsync();
                return customers != null ? Ok(customers) : NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{customerId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        //[PermissionAuthorize(Permission.CanReadCustomer)]
        public async Task<ActionResult<IList<Organization>>> Get(Guid customerId)
        {
            try
            {
                //var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;
                //if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.CustomerAdmin.ToString() || role == PredefinedRole.GroupAdmin.ToString())
                //{
                //    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList").Value;
                //    if (accessList == null || !accessList.Any() || !accessList.Contains(customerId.ToString()))
                //    {
                //        return Forbid();
                //    }
                //}

                var customer = await CustomerServices.GetCustomerAsync(customerId);
                return customer != null ? Ok(customer) : NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Organization>> CreateCustomer([FromBody] NewOrganization newCustomer)
        {
            try
            {
                var createdCustomer = await CustomerServices.CreateCustomerAsync(newCustomer);
                if (createdCustomer == null)
                {
                    return BadRequest();
                }

                return CreatedAtAction(nameof(CreateCustomer), new { id = createdCustomer.Id }, createdCustomer);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Organization>> UpdateOrganization([FromBody] UpdateOrganization organizationToChange)
        {
            try
            {
                var updateOrganization = await CustomerServices.UpdateOrganizationAsync(organizationToChange);
                if (updateOrganization == null)
                {
                    return BadRequest();
                }

                return Ok(updateOrganization);
            }
            catch(Exception ex)
            {
                Logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [HttpPatch]
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Organization>> PatchOrganization([FromBody] UpdateOrganization organizationToChange)
        {
            try
            {
                var updateOrganization = await CustomerServices.PatchOrganizationAsync(organizationToChange);
                if (updateOrganization == null)
                {
                    return BadRequest();
                }

                return Ok(updateOrganization);
            }
            catch (Exception ex)
            {
                Logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [Route("{organizationId:Guid}")]
        [HttpDelete]
        [ProducesResponseType(typeof(Organization), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Organization>> DeleteOrganization(Guid organizationId)
        {
            try
            {
                var deletedOrganization = await CustomerServices.DeleteOrganizationAsync(organizationId);
                if (deletedOrganization == null)
                {
                    return NotFound();
                }

                return Ok(deletedOrganization);
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }

        [Route("{customerId:Guid}/assetCategory")]
        [HttpGet]
        [ProducesResponseType(typeof(OrigoCustomerAssetCategoryType), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IList<OrigoCustomerAssetCategoryType>>> GetAssetCategoryForCustomer(Guid customerId)
        {
            try
            {
                var assetCategoryLifecycleTypes = await CustomerServices.GetAssetCategoryForCustomerAsync(customerId);
                return assetCategoryLifecycleTypes != null ? Ok(assetCategoryLifecycleTypes) : NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{customerId:Guid}/assetCategory")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoCustomerAssetCategoryType), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<OrigoCustomerAssetCategoryType>> AddAssetCategoryForCustomer(Guid customerId, NewCustomerAssetCategoryType customerAssetCategoryType)
        {
            try
            {
                var removedAssetCategory = await CustomerServices.AddAssetCategoryForCustomerAsync(customerId, customerAssetCategoryType);
                if (removedAssetCategory == null)
                {
                    return NotFound();
                }

                return Ok(removedAssetCategory);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [Route("{customerId:Guid}/assetCategory")]
        [HttpDelete]
        [ProducesResponseType(typeof(OrigoCustomerAssetCategoryType), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IList<OrigoCustomerAssetCategoryType>>> DeleteAssetCategoryForCustomer(Guid customerId, NewCustomerAssetCategoryType customerAssetCategoryType)
        {
            try
            {
                var assetCategoryLifecycleTypes = await CustomerServices.RemoveAssetCategoryForCustomerAsync(customerId, customerAssetCategoryType);
                return assetCategoryLifecycleTypes != null ? Ok(assetCategoryLifecycleTypes) : NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{customerId:Guid}/modules")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<OrigoProductModule>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IList<OrigoProductModule>>> GetCustomerProductModule(Guid customerId)
        {
            try
            {
                var productModules = await CustomerServices.GetCustomerProductModulesAsync(customerId);
                return productModules != null ? Ok(productModules) : NotFound();
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("{customerId:Guid}/modules")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoProductModule), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<OrigoProductModule>> AddCustomerProductModule(Guid customerId, NewCustomerProductModule productModule)
        {
            try
            {
                var productModules = await CustomerServices.AddProductModulesAsync(customerId, productModule);
                return productModules != null ? Ok(productModules) : NotFound();
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("{customerId:Guid}/modules")]
        [HttpDelete]
        [ProducesResponseType(typeof(OrigoProductModule), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<OrigoProductModule>> DeleteCustomerProductModule(Guid customerId, NewCustomerProductModule productModule)
        {
            try
            {
                var productModules = await CustomerServices.RemoveProductModulesAsync(customerId, productModule);
                return productModules != null ? Ok(productModules) : NoContent();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}