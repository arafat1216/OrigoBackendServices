﻿using Microsoft.AspNetCore.Mvc;
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
using System.IdentityModel.Tokens.Jwt;

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

        [Route("webshopUserCheck")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> CheckAndProvisionWebShopUser()
        {
            // TODO: revisit security of this endpoint
            try
            {
                if (!HttpContext.Request.Headers.ContainsKey("Authorization"))
                    return Forbid();

                string email = "";

                if (string.IsNullOrWhiteSpace(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value))
                {
                    var bearerToken = HttpContext.Request.Headers["Authorization"];
                    if (System.Net.Http.Headers.AuthenticationHeaderValue.TryParse(bearerToken, out var headerValue))
                    {
                        var scheme = headerValue.Scheme; // "Bearer"
                        var parameter = headerValue.Parameter; // Token

                        var handler = new JwtSecurityTokenHandler();

                        var jsonToken = ((JwtSecurityToken)handler.ReadToken(parameter));

                        email = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                    }
                }
                else
                    email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                if (string.IsNullOrWhiteSpace(email))
                    return Forbid();

                // Get Okta user profile
                var oktaProfile = await CustomerServices.GetOktaUserProfileByEmail(email);
                // Okta user profile: https://developer.okta.com/docs/reference/api/users/#response-example-12
                // org number from organizationNumber attribute in profile.

                return Ok();
            }
            catch (Exception ex)
            {
                Logger.LogError("{0}", ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}