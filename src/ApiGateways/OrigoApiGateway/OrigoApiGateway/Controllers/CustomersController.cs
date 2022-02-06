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
using Common.Enums;
using OrigoApiGateway.Models.BackendDTO;
using AutoMapper;

// ReSharper disable RouteTemplates.RouteParameterConstraintNotResolved

namespace OrigoApiGateway.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("origoapi/v{version:apiVersion}/[controller]")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class CustomersController : ControllerBase
    {
        private ILogger<CustomersController> Logger { get; }
        private ICustomerServices CustomerServices { get; }
        private readonly IMapper Mapper;

        public CustomersController(ILogger<CustomersController> logger, ICustomerServices customerServices, IMapper mapper)
        {
            Logger = logger;
            CustomerServices = customerServices;
            Mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<Organization>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(Permission.CanReadCustomer)]
        [Authorize(Roles = "SystemAdmin")]
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

        [Route("{organizationId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [PermissionAuthorize(Permission.CanReadCustomer)]
        public async Task<ActionResult<IList<Organization>>> Get(Guid organizationId)
        {
            try
            {
                // If role is not System admin, check access list
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                    if (accessList != null && (!accessList.Any() || !accessList.Contains(organizationId.ToString())))
                    {
                        return Forbid();
                    }
                }

                var customer = await CustomerServices.GetCustomerAsync(organizationId);
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
        [Authorize(Roles = "SystemAdmin,PartnerAdmin")]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanCreateCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<Organization>> CreateCustomer([FromBody] NewOrganization newCustomer)
        {
            try
            {
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid.TryParse(actor, out Guid callerId);

                var createdCustomer = await CustomerServices.CreateCustomerAsync(newCustomer, callerId);
                if (createdCustomer == null)
                {
                    return BadRequest();
                }

                return CreatedAtAction(nameof(CreateCustomer), new { id = createdCustomer.OrganizationId }, createdCustomer);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Authorize(Roles = "SystemAdmin,PartnerAdmin,CustomerAdmin")]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<Organization>> UpdateOrganization([FromBody] UpdateOrganization organizationToChange)
        {
            try
            {
                var organizationToChangeDTO = Mapper.Map<UpdateOrganizationDTO>(organizationToChange);


                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);
                organizationToChangeDTO.CallerId = callerId;

                var updateOrganization = await CustomerServices.UpdateOrganizationAsync(organizationToChangeDTO);
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

        [HttpPatch]
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Authorize(Roles = "SystemAdmin,PartnerAdmin,CustomerAdmin")]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<Organization>> PatchOrganization([FromBody] UpdateOrganization organizationToChange)
        {
            try
            {
                var organizationToChangeDTO = Mapper.Map<UpdateOrganizationDTO>(organizationToChange);

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);
                organizationToChangeDTO.CallerId = callerId;

                var updateOrganization = await CustomerServices.PatchOrganizationAsync(organizationToChangeDTO);
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
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Authorize(Roles = "SystemAdmin,PartnerAdmin")]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanDeleteCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<Organization>> DeleteOrganization(Guid organizationId)
        {
            try
            {
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);

                var deletedOrganization = await CustomerServices.DeleteOrganizationAsync(organizationId, callerId);
                if (deletedOrganization == null)
                {
                    return NotFound();
                }

                return Ok(deletedOrganization);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{organizationId:Guid}/assetCategory")]
        [HttpGet]
        [ProducesResponseType(typeof(OrigoCustomerAssetCategoryType), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(Permission.CanReadCustomer)]
        public async Task<ActionResult<IList<OrigoCustomerAssetCategoryType>>> GetAssetCategoryForCustomer(Guid organizationId)
        {
            try
            {
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role != PredefinedRole.SystemAdmin.ToString())
                {

                    // All roles have access to an organizations departments, as long as the organization is in the caller access list
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var assetCategoryLifecycleTypes = await CustomerServices.GetAssetCategoryForCustomerAsync(organizationId);
                return assetCategoryLifecycleTypes != null ? Ok(assetCategoryLifecycleTypes) : NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{organizationId:Guid}/assetCategory")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoCustomerAssetCategoryType), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<OrigoCustomerAssetCategoryType>> AddAssetCategoryForCustomer(Guid organizationId, NewCustomerAssetCategoryType customerAssetCategoryType)
        {
            try
            {
                // Only admin or manager roles are allowed to manage assets
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }

                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }
                Guid.TryParse(actor, out Guid callerId);

                var removedAssetCategory = await CustomerServices.AddAssetCategoryForCustomerAsync(organizationId, customerAssetCategoryType, callerId);
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

        [Route("{organizationId:Guid}/assetCategory")]
        [HttpDelete]
        [ProducesResponseType(typeof(OrigoCustomerAssetCategoryType), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<IList<OrigoCustomerAssetCategoryType>>> DeleteAssetCategoryForCustomer(Guid organizationId, NewCustomerAssetCategoryType customerAssetCategoryType)
        {
            try
            {
                // Only admin or manager roles are allowed to manage assets
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;

                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }

                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                Guid.TryParse(actor, out Guid callerId);

                var assetCategoryLifecycleTypes = await CustomerServices.RemoveAssetCategoryForCustomerAsync(organizationId, customerAssetCategoryType, callerId);
                return assetCategoryLifecycleTypes != null ? Ok(assetCategoryLifecycleTypes) : NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{organizationId:Guid}/modules")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<OrigoProductModule>), (int)HttpStatusCode.OK)]
        [PermissionAuthorize(Permission.CanReadCustomer)]
        public async Task<ActionResult<IList<OrigoProductModule>>> GetCustomerProductModule(Guid organizationId)
        {
            try
            {
                var productModules = await CustomerServices.GetCustomerProductModulesAsync(organizationId);
                return productModules != null ? Ok(productModules) : NotFound();
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("{organizationId:Guid}/modules")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoProductModule), (int)HttpStatusCode.OK)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<OrigoProductModule>> AddCustomerProductModule(Guid organizationId, NewCustomerProductModule productModule)
        {
            try
            {
                // Only admin or manager roles are allowed to manage assets
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }

                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }
                Guid.TryParse(actor, out Guid callerId);

                var productModules = await CustomerServices.AddProductModulesAsync(organizationId, productModule, callerId);
                return productModules != null ? Ok(productModules) : NotFound();
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("{organizationId:Guid}/modules")]
        [HttpDelete]
        [ProducesResponseType(typeof(OrigoProductModule), (int)HttpStatusCode.OK)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<OrigoProductModule>> DeleteCustomerProductModule(Guid organizationId, NewCustomerProductModule productModule)
        {
            try
            {
                // Only admin or manager roles are allowed to manage assets
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }

                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }
                Guid.TryParse(actor, out Guid callerId);

                var productModules = await CustomerServices.RemoveProductModulesAsync(organizationId, productModule, callerId);
                return productModules != null ? Ok(productModules) : NoContent();
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("webshopUrl")]
        [HttpGet]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public ActionResult<string> GetCustomerWebshopUrl()
        {
            return Ok("https://www.google.com/");
        }

        [HttpPost]
        [Route("partner")]
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Authorize(Roles = "SystemAdmin")]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanCreateCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<Organization>> CreatePartner([FromBody] NewOrganization newCustomer)
        {
            try
            {
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid.TryParse(actor, out Guid callerId);

                var createdCustomer = await CustomerServices.CreateCustomerAsync(newCustomer, callerId);
                if (createdCustomer == null)
                {
                    return BadRequest();
                }

                // create partner with new org as coupeld org

                var partner = await CustomerServices.CreatePartnerAsync(createdCustomer.OrganizationId, callerId);
                return CreatedAtAction(nameof(CreatePartner), new { Id = partner.ExternalId }, partner);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("{organizationId:guid}/partner")]
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Authorize(Roles = "SystemAdmin")]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanCreateCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<Organization>> CreatePartner(Guid organizationId)
        {
            try
            {
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid.TryParse(actor, out Guid callerId);

                var organization = await CustomerServices.GetCustomerAsync(organizationId);
                if (organization == null)
                    return NotFound();


                var createdPartner = await CustomerServices.CreatePartnerAsync(organizationId, callerId);
                if (createdPartner == null)
                {
                    return BadRequest();
                }

                return CreatedAtAction(nameof(CreatePartner), new { Id = createdPartner.ExternalId }, createdPartner);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("partner")]
        [ProducesResponseType(typeof(IList<Organization>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(Permission.CanReadCustomer)]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<ActionResult<IList<Organization>>> GetParners()
        {
            try
            {
                var partners = await CustomerServices.GetPartnersAsync();
                return partners != null ? Ok(partners) : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

    }
}