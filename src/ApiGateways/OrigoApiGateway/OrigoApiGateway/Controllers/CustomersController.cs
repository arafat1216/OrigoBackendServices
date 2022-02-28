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
using OrigoApiGateway.Models.SubscriptionManagement;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request;
using Common.Exceptions;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response;
using OrigoApiGateway.Models.SubscriptionManagement.Backend.Request;

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
        private ISubscriptionManagementService SubscriptionManagementService { get; }
        private readonly IMapper Mapper;

        public CustomersController(
            ILogger<CustomersController> logger,
            ICustomerServices customerServices,
            IMapper mapper,
            ISubscriptionManagementService subscriptionManagementService
            )
        {
            Logger = logger;
            CustomerServices = customerServices;
            Mapper = mapper;
            SubscriptionManagementService = subscriptionManagementService;
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
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
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
            catch (InvalidOrganizationNumberException exception)
            {
                return Conflict(exception.Message);
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

        [Route("userCount")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<CustomerUserCount>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(Permission.CanReadCustomer)]
        public async Task<ActionResult<IList<CustomerUserCount>>> GetOrganizationUsers()
        {
            try
            {
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role != PredefinedRole.SystemAdmin.ToString())
                {

                    // Only SystemAdmin has access to all organization user counts
                    return Forbid();
                }

                var organizationUserCounts = await CustomerServices.GetCustomerUsersAsync();
                return organizationUserCounts != null ? Ok(organizationUserCounts) : NotFound();
            }
            catch (Exception)
            {
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

        /// <summary>
        /// Get all operators a customer
        /// </summary>
        /// <param name="organizationId">Organization identifier</param>
        /// <returns></returns>
        [Route("{organizationId:Guid}/operators")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrigoOperator>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetAllOperatorsForCustomer(Guid organizationId)
        {
            var customersOperators = await SubscriptionManagementService.GetAllOperatorsForCustomerAsync(organizationId);
            return Ok(customersOperators);
        }

        /// <summary>
        /// Create operator list for customer
        /// </summary>
        /// <param name="organizationId">Organization identifier</param>
        /// <param name="operators">List of operator identifier</param>
        /// <returns></returns>
        [Route("{organizationId:Guid}/operators")]
        [HttpPost]
        public async Task<ActionResult> CreateOperatorListForCustomerAsync(Guid organizationId, [FromBody] List<int> operators)
        {

            var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
            Guid callerId;
            Guid.TryParse(actor, out callerId);

            NewOperatorListDTO newOperatorListDTO = new NewOperatorListDTO();
            newOperatorListDTO.Operators = operators;
            newOperatorListDTO.CallerId = callerId;

            await SubscriptionManagementService.AddOperatorForCustomerAsync(organizationId, newOperatorListDTO);

            return NoContent();
        }

        /// <summary>
        /// Delete customer's operator
        /// </summary>
        /// <param name="organizationId">Organization identifier</param>
        /// <param name="id">Operator identifier</param>
        /// <returns></returns>
        [Route("{organizationId:Guid}/operators/{id}")]
        [HttpDelete]
        public async Task<ActionResult> DeleteFromCustomersOperatorList(Guid organizationId, int id)
        {
            await SubscriptionManagementService.DeleteOperatorForCustomerAsync(organizationId, id);
            return NoContent();
        }

        [Route("{organizationId:Guid}/subscription-products")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<OrigoSubscriptionProduct>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IList<OrigoSubscriptionProduct>>> GetSubscriptionProductsForCustomer(Guid organizationId)
        {
            try
            {

                var subscriptionProductList = await SubscriptionManagementService.GetAllSubscriptionProductForCustomerAsync(organizationId);
                //if (subscriptionProductList == null)
                //{
                //    return BadRequest();
                //}
                return Ok(subscriptionProductList);
            }
            catch (Exception ex)
            {
                Logger.LogError("GetSubscriptionProductsForCustomer gateway", ex.Message);
                return BadRequest();
            }
        }

        [Route("{organizationId:Guid}/subscription-products")]
        [HttpPost]
        [ProducesResponseType(typeof(OrigoSubscriptionProduct), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<OrigoSubscriptionProduct>> CreateSubscriptionProductForCustomer(Guid organizationId, [FromBody] NewSubscriptionProduct newSubscriptionProduct)
        {
            try
            {
                var subscriptionProduct = await SubscriptionManagementService.AddSubscriptionProductForCustomerAsync(organizationId, newSubscriptionProduct);
                //if (subscriptionProduct == null)
                //{
                //    return BadRequest();
                //}
                return CreatedAtAction(nameof(CreateSubscriptionProductForCustomer), subscriptionProduct);
            }
            catch (Exception ex)
            {

                Logger.LogError("CreateSubscriptionProductForCustomer gateway", ex.Message);
                return BadRequest();
            }

        }

        [HttpPatch]
        [Route("{organizationId:Guid}/subscription-products/{subscriptionProductId}")]
        [ProducesResponseType(typeof(OrigoSubscriptionProduct), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<OrigoSubscriptionProduct>> UpdateOperatorSubscriptionProductForCustomer(Guid organizationId, int subscriptionProductId, [FromBody] UpdateSubscriptionProduct subscriptionProduct)
        {
            try
            {
                var updatedSubscriptionProducts = await SubscriptionManagementService.UpdateOperatorSubscriptionProductForCustomerAsync(organizationId, subscriptionProductId, subscriptionProduct);

                //return the updated subscription product
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.LogError("UpdateOperatorSubscriptionProductForCustomer gateway ", ex);
                return BadRequest("Unable to update subscription product");
            }
        }

        [Route("{organizationId:Guid}/subscription-products/{subscriptionProductId}")]
        [HttpDelete]
        [ProducesResponseType(typeof(OrigoSubscriptionProduct), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<OrigoSubscriptionProduct>> DeleteSubscriptionProductsForCustomer(Guid organizationId, int subscriptionProductId)
        {
            try
            {
                var subscriptionProductList = await SubscriptionManagementService.DeleteSubscriptionProductForCustomerAsync(organizationId, subscriptionProductId);
                //if (subscriptionProductList == null)
                //{
                //    return BadRequest();
                //}
                return Ok(subscriptionProductList);
            }
            catch (Exception ex)
            {
                Logger.LogError("DeleteSubscriptionProductsForCustomer gateway", ex.Message);
                return BadRequest();
            }
        }

        /// <summary>
        /// Get list of customer operator accounts
        /// </summary>
        /// <param name="organizationId">Organization identifier</param>
        /// <returns>list of customer operator accounts</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IList<OrigoCustomerOperatorAccount>), (int)HttpStatusCode.OK)]
        [Route("{organizationId:Guid}/operator-accounts")]
        public async Task<IActionResult> GetAllOperatorAccountsForCustomer(Guid organizationId)
        {
            var customerOperatorAccounts = await SubscriptionManagementService.GetAllOperatorAccountsForCustomerAsync(organizationId);

            return Ok(customerOperatorAccounts);
        }

        /// <summary>
        /// Setup customer account
        /// </summary>
        /// <param name="organizationId">Organization identifier</param>
        /// <param name="customerOperatorAccount">Details of customer operator account</param>
        /// <returns>new customer operator account</returns>
        [HttpPost]
        [ProducesResponseType(typeof(OrigoCustomerOperatorAccount), (int)HttpStatusCode.OK)]
        [Route("{organizationId:Guid}/operator-accounts")]
        public async Task<IActionResult> AddOperatorAccountForCustomer(Guid organizationId, [FromBody] NewOperatorAccount customerOperatorAccount)
        {
            var operatorAccount = await SubscriptionManagementService.AddOperatorAccountForCustomerAsync(organizationId, customerOperatorAccount);

            return Ok(operatorAccount);
        }

        /// <summary>
        /// Delete customer operator account
        /// </summary>
        /// <param name="organizationId">Organization identifier</param>
        /// <param name="accountNumber">Account number</param>
        /// <param name="operatorId">Operator id</param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(typeof(OrigoCustomerOperatorAccount), (int)HttpStatusCode.OK)]
        [Route("{organizationId:Guid}/operator-accounts")]
        public async Task<IActionResult> DeleteOperatorAccountForCustomer(Guid organizationId, [FromQuery] string accountNumber, [FromQuery] int operatorId)
        {
            await SubscriptionManagementService.DeleteOperatorAccountForCustomerAsync(organizationId, accountNumber, operatorId);

            return Ok();
        }

        /// <summary>
        /// Creates a transfer subscription order
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        [Route("{organizationId:Guid}/subscription-transfer-to-business")]
        [HttpPost]
        public async Task<ActionResult> TransferSubscriptionToBusiness(Guid organizationId, [FromBody] TransferToBusinessSubscriptionOrder order)
        {
            var response = await SubscriptionManagementService.TransferToBusinessSubscriptionOrderForCustomerAsync(organizationId, order);
            return Ok(response);
        }

        [Route("{organizationId:Guid}/subscription-transfer-to-private")]
        [HttpPost]
        public async Task<ActionResult> TransferSubscriptionToPrivate(Guid organizationId, [FromBody] TransferToPrivateSubscriptionOrder order)
        {
            var response = await SubscriptionManagementService.TransferToPrivateSubscriptionOrderForCustomerAsync(organizationId, order);
            return Ok(response);
        }

        [Route("{organizationId:Guid}/subscription-orders")]
        [ProducesResponseType(typeof(IList<OrigoSubscriptionOrderListItem>), (int)HttpStatusCode.OK)]
        [HttpGet]
        public async Task<ActionResult> GetSubscriptionOrders(Guid organizationId)
        {
            var origoSubscriptionOrderList = new List<OrigoSubscriptionOrderListItem>
            {
                new OrigoSubscriptionOrderListItem{
                    CreatedDate = new DateTime(2022, 1, 2),
                    NewSubscriptionOrderOwnerName = "My Company",
                    TransferDate = new DateTime(2022, 1, 8),
                    OrderType = "TransferToBusiness",
                    CreatedBy = "Kari Nordmann",
                    PhoneNumber = "919999999"
                },
                new OrigoSubscriptionOrderListItem{
                    CreatedDate = new DateTime(2022, 1, 12),
                    NewSubscriptionOrderOwnerName = "Another Company",
                    TransferDate = new DateTime(2022, 1, 19),
                    OrderType = "TransferToBusiness",
                    CreatedBy = "Lene Hansen",
                    PhoneNumber = "45454545"
                },
                new OrigoSubscriptionOrderListItem{
                    CreatedDate = new DateTime(2022, 1, 3),
                    NewSubscriptionOrderOwnerName = "Ola Nordmann",
                    TransferDate = new DateTime(2022, 1, 4),
                    OrderType = "TransferToPrivate",
                    CreatedBy = "Kari Nordmann",
                    PhoneNumber = "44443333"
                }
            };
            return Ok(origoSubscriptionOrderList);
        }


    }
}