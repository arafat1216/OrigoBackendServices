using AutoMapper;
using Common.Enums;
using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigoApiGateway.Authorization;
using OrigoApiGateway.Exceptions;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.Asset;
using OrigoApiGateway.Models.BackendDTO;
using OrigoApiGateway.Services;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
// ReSharper disable RouteTemplates.RouteParameterConstraintNotResolved
// ReSharper disable RouteTemplates.ControllerRouteParameterIsNotPassedToMethods

namespace OrigoApiGateway.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    // Assets should only be available through a given customer
    [Route("/origoapi/v{version:apiVersion}/[controller]")]
    public class AssetsController : ControllerBase
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger<AssetsController> _logger;
        private readonly IAssetServices _assetServices;
        private readonly ICustomerServices _customerServices;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;

        public AssetsController(ILogger<AssetsController> logger, IAssetServices assetServices, IStorageService storageService, IMapper mapper, ICustomerServices customerServices)
        {
            _logger = logger;
            _assetServices = assetServices;
            _storageService = storageService;
            _mapper = mapper;
            _customerServices = customerServices;
        }

        [Route("customers/count/pagination")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<CustomerAssetCount>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanReadAsset)]
        public async Task<ActionResult<PagedModel<CustomerAssetCount>>> GetAllCustomerItemCountPagination([FromQuery] int page = 1, [Range(1, 100)] int limit = 25)
        {
            try
            {
                var customerList = new List<Guid>();

                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.PartnerAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    foreach (var customerId in accessList)
                    {
                        if (Guid.TryParse(customerId, out var customerIdGuid))
                        {
                            customerList.Add(customerIdGuid);
                        }
                    }
                }
                else if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    return Forbid();
                }
                var assetCountList = await _assetServices.GetAllCustomerAssetsCountAsync(customerList, page, limit);
                return Ok(assetCountList);

            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [Obsolete("Should be removed after frontend adapt to pagination")]
        [Route("customers/count")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<CustomerAssetCount>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanReadAsset)]
        public async Task<ActionResult<IList<CustomerAssetCount>>> GetAllCustomerItemCount()
        {
            try
            {
                var customerList = new List<Guid>();

                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.PartnerAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    foreach (var customerId in accessList)
                    {
                        if (Guid.TryParse(customerId, out var customerIdGuid))
                        {
                            customerList.Add(customerIdGuid);
                        }
                    }
                }
                else if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    return Forbid();
                }
                var assetCountList = await _assetServices.GetAllCustomerAssetsCountAsync(customerList);
                return Ok(assetCountList);

            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [Route("customers/{organizationId:guid}/count")]
        [HttpGet]
        [ProducesResponseType(typeof(CustomerAssetCount), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanReadAsset)]
        public async Task<ActionResult<CustomerAssetCount>> GetCustomerItemCount([FromRoute] Guid organizationId, [FromQuery] Guid? departmentId, [FromQuery] AssetLifecycleStatus? assetLifecycleStatus)
        {
            try
            {
                // All roles have access, as long as customer is in their accessList
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }
                var count = await _assetServices.GetAssetsCountAsync(organizationId, departmentId, assetLifecycleStatus);
                return Ok(new CustomerAssetCount() { OrganizationId = organizationId, Count = count });
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [Route("customers/{organizationId:guid}/total-book-value")]
        [HttpGet]
        [ProducesResponseType(typeof(CustomerAssetValue), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanReadAsset)]
        public async Task<ActionResult<CustomerAssetValue>> GetCustomerTotalBookValue([FromRoute] Guid organizationId)
        {
            try
            {
                // All roles have access, as long as customer is in their accessList
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var currency = await _customerServices.GetCurrencyByCustomer(organizationId);
                var totalBookValue = await _assetServices.GetCustomerTotalBookValue(organizationId);
                return Ok(new CustomerAssetValue() { OrganizationId = organizationId, Amount = totalBookValue, Currency = currency });
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [Route("customers/{organizationId:guid}/{userId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<OrigoAsset>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanReadAsset)]
        public async Task<ActionResult<IList<OrigoAsset>>> Get([FromRoute] Guid organizationId, [FromRoute] Guid userId,
            [FromQuery] bool includeAsset = true, bool includeImeis = true, bool includeContractHolderUser = true)
        {
            try
            {
                // All roles have access, as long as customer is in their accessList
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var assets = await _assetServices.GetAssetsForUserAsync(organizationId, userId, includeAsset, includeImeis, includeContractHolderUser);
                if (assets == null)
                {
                    return NotFound();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                return Ok(JsonSerializer.Serialize<object>(assets, options));
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [Route("customers/{organizationId:guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(PagedModel<HardwareSuperType>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanReadAsset)]
        public async Task<ActionResult> Get([FromRoute] Guid organizationId, CancellationToken cancellationToken, [FromQuery] FilterOptionsForAsset filterOptions, [FromQuery(Name = "q")] string search = "", [FromQuery] int page = 1, [FromQuery][Range(1, 100)] int limit = 25,
            [FromQuery] bool includeAsset = true, bool includeImeis = true, bool includeLabels = true, bool includeContractHolderUser = true)
        {
            try
            {

                // Only admin or manager roles are allowed to see all assets
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    filterOptions.UserId = "me";
                }

                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();

                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                if ((role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString()) && accessList != null)
                {
                    filterOptions.Department ??= new List<Guid?>();
                    foreach (var departmentId in accessList)

                    {
                        if (Guid.TryParse(departmentId, out var departmentGuid))
                        {
                            filterOptions.Department.Add(departmentGuid);

                        }
                    }
                }

                filterOptions.UserId = filterOptions.UserId == "me" ? HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value : null;

                var assets = await _assetServices.GetAssetsForCustomerAsync(organizationId, cancellationToken, filterOptions, search, page: page, limit: limit, includeAsset: includeAsset, includeImeis: includeImeis, includeLabels: includeLabels, includeContractHolderUser: includeContractHolderUser);
                if (assets == null)
                {
                    return NotFound();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                return Ok(JsonSerializer.Serialize(assets, options));
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [Route("customers/{organizationId:guid}/lifecycle-setting")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<LifeCycleSetting>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanReadAsset)]
        public async Task<ActionResult> GetLifeCycleSettingByCustomer([FromRoute] Guid organizationId)
        {
            try
            {
                // Only admin or manager roles are allowed to see all assets
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }
                var currency = await _customerServices.GetCurrencyByCustomer(organizationId);
                var settings = await _assetServices.GetLifeCycleSettingByCustomer(organizationId, currency);
                if (settings == null)
                {
                    return NotFound();
                }

                return Ok(settings);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [Route("customers/{organizationId:guid}/lifecycle-setting")]
        [HttpPost]
        [ProducesResponseType(typeof(LifeCycleSetting), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanReadAsset)]
        public async Task<ActionResult> SetLifeCycleSetting([FromRoute] Guid organizationId, [FromBody] NewLifeCycleSetting setting)
        {
            try
            {
                // Only admin or manager roles are allowed to manage assets
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }

                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);
                var currency = await _customerServices.GetCurrencyByCustomer(organizationId);
                var createdSetting = await _assetServices.SetLifeCycleSettingForCustomerAsync(organizationId, setting, currency, callerId);

                if (createdSetting != null)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true
                    };
                    return CreatedAtAction(nameof(SetLifeCycleSetting), new { id = createdSetting.Id }, JsonSerializer.Serialize<object>(createdSetting, options));
                }
                return BadRequest();
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("customers/{organizationId:guid}/dispose-setting")]
        [HttpGet]
        [ProducesResponseType(typeof(DisposeSetting), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanReadAsset)]
        public async Task<ActionResult> GetDisposeSettingByCustomer([FromRoute] Guid organizationId)
        {
            try
            {
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }
                var settings = await _assetServices.GetDisposeSettingByCustomer(organizationId);
                if (settings == null)
                {
                    return NotFound();
                }

                return Ok(settings);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [Route("customers/{organizationId:guid}/dispose-setting")]
        [HttpPost]
        [ProducesResponseType(typeof(DisposeSetting), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanReadAsset)]
        public async Task<ActionResult> SetDisposeSetting([FromRoute] Guid organizationId, [FromBody] NewDisposeSetting setting)
        {
            try
            {
                // Only admin or manager roles are allowed to manage assets
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }

                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);
                var createdSetting = await _assetServices.SetDisposeSettingForCustomerAsync(organizationId, setting, callerId);

                if (createdSetting != null)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true
                    };
                    return CreatedAtAction(nameof(SetDisposeSetting), new { id = createdSetting.Id }, JsonSerializer.Serialize<object>(createdSetting, options));
                }
                return BadRequest();
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("customers/{organizationId:guid}/return-location")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<ReturnLocation>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> GetReturnLocationsByCustomer([FromRoute] Guid organizationId)
        {
            try
            {
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }
                var locations = await _assetServices.GetReturnLocationsByCustomer(organizationId);
                if (locations == null)
                {
                    return NotFound();
                }
                var allOfficeLocations = await _customerServices.GetAllCustomerLocations(organizationId);
                foreach (var location in locations)
                {
                    location.Location = allOfficeLocations.FirstOrDefault(x => x.Id == location.LocationId);
                }

                return Ok(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [Route("customers/{organizationId:guid}/return-location")]
        [HttpPost]
        [ProducesResponseType(typeof(ReturnLocation), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> AddReturnLocationsByCustomer([FromRoute] Guid organizationId, [FromBody] NewReturnLocation data)
        {
            try
            {
                // Only admin or manager roles are allowed to see all assets
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }
                var allOfficeLocations = await _customerServices.GetAllCustomerLocations(organizationId);

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);

                var location = await _assetServices.AddReturnLocationsByCustomer(organizationId, data, allOfficeLocations, callerId);
                if (location == null)
                {
                    return NotFound();
                }

                location.Location = allOfficeLocations.FirstOrDefault(x => x.Id == location.LocationId);
                return Ok(location);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [Route("customers/{organizationId:guid}/return-location/{returnLocationId:guid}")]
        [HttpPut]
        [ProducesResponseType(typeof(IList<ReturnLocation>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> UpdateReturnLocationsByCustomer([FromRoute] Guid organizationId, [FromRoute] Guid returnLocationId, [FromBody] NewReturnLocation data)
        {
            try
            {
                // Only admin or manager roles are allowed to see all assets
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }
                var allOfficeLocations = await _customerServices.GetAllCustomerLocations(organizationId);
                if (!allOfficeLocations.Select(x => x.Id).Contains(data.LocationId))
                {
                    return BadRequest();
                }
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);

                var location = await _assetServices.UpdateReturnLocationsByCustomer(organizationId, returnLocationId, data, callerId);
                if (location == null)
                {
                    return NotFound();
                }

                location.Location = allOfficeLocations.FirstOrDefault(x => x.Id == location.LocationId);
                return Ok(location);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [Route("customers/{organizationId:guid}/return-location/{returnLocationId:guid}")]
        [HttpDelete]
        [ProducesResponseType(typeof(IList<ReturnLocation>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> RemoveReturnLocationsByCustomer([FromRoute] Guid organizationId, [FromRoute] Guid returnLocationId)
        {
            try
            {
                // Only admin or manager roles are allowed to see all assets
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var locations = await _assetServices.DeleteReturnLocationsByCustomer(organizationId, returnLocationId);
                if (locations == null)
                {
                    return NotFound();
                }
                var allOfficeLocations = await _customerServices.GetAllCustomerLocations(organizationId);

                foreach (var location in locations)
                {
                    location.Location = allOfficeLocations.FirstOrDefault(x => x.Id == location.LocationId);
                }

                return Ok(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [Route("{assetId:guid}/customers/{organizationId:guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<OrigoAsset>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanReadAsset)]
        public async Task<ActionResult<OrigoAsset>> GetAsset([FromRoute] Guid organizationId, [FromRoute] Guid assetId,
            [FromQuery] bool includeAsset = true, bool includeImeis = true, bool includeLabels = true, bool includeContractHolderUser = true)
        {
            try
            {
                FilterOptionsForAsset filterOptions = null;

                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();

                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                if (role == PredefinedRole.EndUser.ToString())
                {
                    filterOptions = new FilterOptionsForAsset();
                    filterOptions.UserId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value ?? null;
                }

                if ((role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString()) && accessList != null)
                {
                    filterOptions = new FilterOptionsForAsset();
                    filterOptions.Department ??= new List<Guid?>();

                    foreach (var departmentId in accessList)
                    {
                        if (Guid.TryParse(departmentId, out var departmentGuid))
                        {
                            filterOptions.Department.Add(departmentGuid);

                        }
                    }
                }

                var asset = await _assetServices.GetAssetForCustomerAsync(organizationId, assetId, filterOptions, includeAsset, includeImeis, includeLabels, includeContractHolderUser);
                if (asset == null)
                {
                    return NotFound();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                return Ok(JsonSerializer.Serialize<object>(asset, options));
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [Route("customers/{organizationId:guid}")]
        [HttpPost]
        [ProducesResponseType(typeof(OrigoAsset), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanCreateAsset)]
        public async Task<ActionResult> CreateAsset([FromRoute] Guid organizationId, [FromBody] NewAsset asset)
        {
            try
            {
                // Only admin or manager roles are allowed to manage assets
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }

                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                //Mapping from frontend model to a backend DTO
                var newAssetDTO = _mapper.Map<NewAssetDTO>(asset);

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);
                newAssetDTO.CallerId = callerId; // Guid.Empty if tryparse failed.

                var createdAsset = await _assetServices.AddAssetForCustomerAsync(organizationId, newAssetDTO);
                if (createdAsset != null)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true
                    };
                    return CreatedAtAction(nameof(CreateAsset), new { id = createdAsset.Id }, JsonSerializer.Serialize<object>(createdAsset, options));
                }
                return BadRequest();
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("customers/{organizationId:guid}/assetStatus")]
        [HttpPatch]
        [Obsolete("Do not call. Will be deleted in the future")]
        [ProducesResponseType(typeof(OrigoAsset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateAsset)]
        public async Task<ActionResult> SetAssetStatusOnAssets([FromRoute] Guid organizationId, [FromBody] UpdateAssetsStatus updatedAssetStatus)
        {
            try
            {
                // Only admin or manager roles are allowed to manage assets
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }

                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid.TryParse(actor, out Guid callerId);
                if (!updatedAssetStatus.AssetGuidList.Any())
                    return BadRequest("No assets selected.");

                var updatedAssets = await _assetServices.UpdateStatusOnAssets(organizationId, updatedAssetStatus, callerId);
                if (updatedAssets == null)
                {
                    return NotFound();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                return Ok(JsonSerializer.Serialize<object>(updatedAssets, options));
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Unable to change status on assets");
            }
        }

        [Route("customers/{organizationId:guid}/make-available")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoAsset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateAsset)]
        public async Task<ActionResult> MakeAssetAvailable([FromRoute] Guid organizationId, [FromBody] MakeAssetAvailable data)
        {
            try
            {
                // Only admin or manager roles are allowed to manage assets
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }

                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid.TryParse(actor, out Guid callerId);
                if (data.AssetLifeCycleId == Guid.Empty)
                    return BadRequest("No asset selected.");

                var updatedAssets = await _assetServices.MakeAssetAvailableAsync(organizationId, data, callerId);
                if (updatedAssets == null)
                {
                    return NotFound();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                return Ok(JsonSerializer.Serialize<object>(updatedAssets, options));
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Unable to make the asset available");
            }
        }


        [Route("customers/{organizationId:guid}/return-device")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoAsset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> ReturnAssetAsync([FromRoute] Guid organizationId, [FromBody] ReturnAsset data)
        {
            try
            {

                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                var department = new List<Guid?>();
                if ((role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString()) && accessList != null)
                {
                    foreach (var departmentId in accessList)
                    {
                        if (Guid.TryParse(departmentId, out var departmentGuid))
                        {
                            department.Add(departmentGuid);
                        }
                    }
                }
                else if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid.TryParse(actor, out Guid callerId);
                if (data.AssetId == Guid.Empty)
                    return BadRequest("No asset selected.");

                var updatedAssets = await _assetServices.ReturnDeviceAsync(organizationId, data.AssetId, role, department, data.ReturnLocationId, callerId);
                if (updatedAssets == null)
                {
                    return NotFound();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                return Ok(JsonSerializer.Serialize<object>(updatedAssets, options));
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Unable to process 'Return Device' for this Asset Id:{data.AssetId}");
            }
        }

        [Route("customers/{organizationId:guid}/buyout-device")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoAsset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> BuyoutDeviceAsync([FromRoute] Guid organizationId, [FromBody] BuyoutDevice data)
        {
            try
            {
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                var department = new List<Guid?>();
                if ((role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString()) && accessList != null)
                {
                    foreach (var departmentId in accessList)
                    {
                        if (Guid.TryParse(departmentId, out var departmentGuid))
                        {
                            department.Add(departmentGuid);
                        }
                    }
                }
                else if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid.TryParse(actor, out Guid callerId);
                if (data.AssetId == Guid.Empty)
                    return BadRequest("No asset selected.");
                var org = await _customerServices.GetCustomerAsync(organizationId);
                if (string.IsNullOrEmpty(org.PayrollContactEmail))
                    throw new BadHttpRequestException($"Payroll responsible email need to set first to do buyout for CustomerId: {organizationId}");

                var updatedAssets = await _assetServices.BuyoutDeviceAsync(organizationId, data.AssetId, role, department, org.PayrollContactEmail, callerId);
                if (updatedAssets == null)
                {
                    return NotFound();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                return Ok(JsonSerializer.Serialize<object>(updatedAssets, options));
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Unable to process 'Buyout Device' for this Asset Id:{data.AssetId}");
            }
        }

        /// <summary>
        /// Requesting buyout to perform on last working day
        /// </summary>
        /// <param name="organizationId">Customer Identifier</param>
        /// <param name="data">Buyout device details</param>
        /// <returns>Updated Asset Details</returns>
        [Route("customers/{organizationId:guid}/pending-buyout")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoAsset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> PendingBuyoutDeviceAsync([FromRoute] Guid organizationId, [FromBody] BuyoutDevice data)
        {
            try
            {
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                var department = new List<Guid?>();
                if ((role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString()) && accessList != null)
                {
                    foreach (var departmentId in accessList)
                    {
                        if (Guid.TryParse(departmentId, out var departmentGuid))
                        {
                            department.Add(departmentGuid);
                        }
                    }
                }
                else if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid.TryParse(actor, out Guid callerId);
                if (data.AssetId == Guid.Empty)
                    return BadRequest("No asset selected.");

                var currency = await _customerServices.GetCurrencyByCustomer(organizationId);
                var updatedAssets = await _assetServices.PendingBuyoutDeviceAsync(organizationId, data.AssetId, role, department, currency, callerId);
                if (updatedAssets == null)
                {
                    return NotFound();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                return Ok(JsonSerializer.Serialize<object>(updatedAssets, options));
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return Unauthorized(ex.Message);
            }

            catch (PendingBuyoutException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Unable to process 'Pending Buyout' for this Asset Id:{data.AssetId}");
            }
        }

        [Route("customers/{organizationId:guid}/report-device")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoAsset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> ReportDeviceAsync([FromRoute] Guid organizationId, [FromBody] ReportDevice data)
        {
            try
            {
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                var department = new List<Guid?>();
                if ((role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString()) && accessList != null)
                {
                    foreach (var departmentId in accessList)
                    {
                        if (Guid.TryParse(departmentId, out var departmentGuid))
                        {
                            department.Add(departmentGuid);
                        }
                    }
                }
                else if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid.TryParse(actor, out Guid callerId);
                if (data.AssetId == Guid.Empty)
                    return BadRequest("No asset selected.");

                var updatedAssets = await _assetServices.ReportDeviceAsync(organizationId, data, role, department, callerId);
                if (updatedAssets == null)
                {
                    return NotFound();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                return Ok(JsonSerializer.Serialize<object>(updatedAssets, options));
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Unable to process 'Report Device' for this Asset Id:{data.AssetId}");
            }
        }

        [Route("{assetId:Guid}/customers/{organizationId:guid}/re-assignment-personal")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoAsset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateAsset)]
        public async Task<ActionResult> ReAssignAssetPersonal([FromRoute] Guid assetId, [FromRoute] Guid organizationId, [FromBody] ReAssignmentPersonal data)
        {
            try
            {
                // Only admin or manager roles are allowed to manage assets
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }

                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid.TryParse(actor, out Guid callerId);
                if (data.DepartmentId == Guid.Empty)
                    return BadRequest("No Department selected.");
                if (data.UserId == Guid.Empty)
                    return BadRequest("No User selected.");

                var postData = _mapper.Map<ReassignedToUserDTO>(data);
                postData.CallerId = callerId;
                var updatedAssets = await _assetServices.ReAssignAssetToUser(organizationId, assetId, postData);
                if (updatedAssets == null)
                {
                    return NotFound();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                return Ok(JsonSerializer.Serialize<object>(updatedAssets, options));
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Unable to Re-Assign Personal For the asset ");
            }
        }

        [Route("{assetId:Guid}/customers/{organizationId:guid}/re-assignment-nonpersonal")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoAsset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateAsset)]
        public async Task<ActionResult> ReAssignAssetNonPersonal([FromRoute] Guid assetId, [FromRoute] Guid organizationId, [FromBody] ReAssignmentNonPersonal data)
        {
            try
            {
                // Only admin or manager roles are allowed to manage assets
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }

                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid.TryParse(actor, out Guid callerId);
                if (data.DepartmentId == Guid.Empty)
                    return BadRequest("No Department selected.");
                var postData = _mapper.Map<ReassignedToDepartmentDTO>(data);
                postData.CallerId = callerId;
                var updatedAssets = await _assetServices.ReAssignAssetToDepartment(organizationId, assetId, postData);
                if (updatedAssets == null)
                {
                    return NotFound();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                return Ok(JsonSerializer.Serialize<object>(updatedAssets, options));
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Unable to Re-Assign Non-Personal For the asset ");
            }
        }


        [Route("{assetId:Guid}/customers/{organizationId:guid}/Update")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoAsset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateAsset)]
        public async Task<ActionResult> UpdateAsset([FromRoute] Guid organizationId, [FromRoute] Guid assetId, [FromBody] OrigoUpdateAsset asset)
        {
            try
            {
                // Only admin or manager roles are allowed to manage assets
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }

                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                //Mapping from frontend model to a backend DTO
                var origoUpdateAssetDTO = _mapper.Map<OrigoUpdateAssetDTO>(asset);

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid.TryParse(actor, out Guid callerId);
                origoUpdateAssetDTO.CallerId = callerId;

                var updatedAsset = await _assetServices.UpdateAssetAsync(organizationId, assetId, origoUpdateAssetDTO);
                if (updatedAsset == null)
                {
                    return NotFound();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                return Ok(JsonSerializer.Serialize<object>(updatedAsset, options));
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("customers/{organizationId:guid}/labels")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<Label>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(Permission.CanReadCustomer)]
        public async Task<ActionResult<IList<Label>>> GetLabelsForCustomer([FromRoute] Guid organizationId)
        {
            try
            {
                // All roles have access, as long as customer is in their accessList
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }
                var labels = await _assetServices.GetCustomerLabelsAsync(organizationId);

                return Ok(labels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in GetLabelsForCustomer");
                return BadRequest();
            }
        }

        [Route("customers/{organizationId:guid}/labels")]
        [HttpPost]
        [ProducesResponseType(typeof(IList<Label>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<IList<Label>>> CreateLabelsForCustomer([FromRoute] Guid organizationId, [FromBody] IList<NewLabel> labels)
        {
            try
            {
                // Only admin or manager roles are allowed to update customer labels
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId); // callerId is empty if tryparse fails.

                AddLabelsData data = new AddLabelsData
                {
                    NewLabels = labels,
                    CallerId = callerId
                };

                var createdLabels = await _assetServices.CreateLabelsForCustomerAsync(organizationId, data);

                return Ok(createdLabels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in CreateLabelsForCustomer");
                return BadRequest();
            }
        }

        [Route("customers/{organizationId:guid}/labels")]
        [HttpDelete]
        [ProducesResponseType(typeof(IList<Label>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<IList<Label>>> DeleteLabelsForCustomer([FromRoute] Guid organizationId, [FromBody] IList<Guid> labelGuids)
        {
            try
            {
                // Only admin or manager roles are allowed to update customer labels
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                bool valid = Guid.TryParse(actor, out Guid callerId);
                if (!valid)
                    callerId = Guid.Empty;

                DeleteCustomerLabelsData data = new DeleteCustomerLabelsData
                {
                    LabelGuids = labelGuids,
                    CallerId = callerId
                };

                var createdLabels = await _assetServices.DeleteCustomerLabelsAsync(organizationId, data);

                return Ok(createdLabels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in DeleteLabelsForCustomer");
                return BadRequest();
            }
        }

        [Route("customers/{organizationId:guid}/labels")]
        [HttpPatch]
        [ProducesResponseType(typeof(IList<Label>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<IList<Label>>> UpdateLabelsForCustomer([FromRoute] Guid organizationId, IList<Label> labels)
        {
            try
            {
                // Only admin or manager roles are allowed to update customer labels
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid.TryParse(actor, out Guid callerId);

                UpdateCustomerLabelsData data = new UpdateCustomerLabelsData
                {
                    Labels = labels,
                    CallerId = callerId
                };

                var createdLabels = await _assetServices.UpdateLabelsForCustomerAsync(organizationId, data);

                return Ok(createdLabels);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("customers/{organizationId:guid}/labels/assign")]
        [HttpPost]
        [ProducesResponseType(typeof(IList<OrigoAsset>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadAsset, Permission.CanUpdateAsset)]
        public async Task<ActionResult<IList<OrigoAsset>>> AssignLabelsToAssets([FromRoute] Guid organizationId, [FromBody] AssetLabels assetLabels)
        {
            try
            {
                // Only admin or manager roles are allowed to set labels on assets
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                //Mapping from frontend model to a backend DTO
                var assetLabelsDTO = _mapper.Map<AssetLabelsDTO>(assetLabels);

                // Get caller of endpoint
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid.TryParse(actor, out Guid callerId);
                assetLabelsDTO.CallerId = callerId;

                var updatedAssets = await _assetServices.AssignLabelsToAssets(organizationId, assetLabelsDTO);
                if (updatedAssets == null)
                {
                    return NotFound();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                return Ok(JsonSerializer.Serialize<object>(updatedAssets, options));

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("customers/{organizationId:guid}/labels/unassign")]
        [HttpPost]
        [ProducesResponseType(typeof(IList<OrigoAsset>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadAsset, Permission.CanUpdateAsset)]
        public async Task<ActionResult<IList<OrigoAsset>>> UnAssignLabelsToAssets([FromRoute] Guid organizationId, [FromBody] AssetLabels assetLabels)
        {
            try
            {
                // Only admin or manager roles are allowed to set labels on assets
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var assetLabelsDTO = _mapper.Map<AssetLabelsDTO>(assetLabels);

                // Get caller of endpoint
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                bool valid = Guid.TryParse(actor, out Guid callerId);
                if (!valid)
                    callerId = Guid.Empty;
                assetLabelsDTO.CallerId = callerId;

                var updatedAssets = await _assetServices.UnAssignLabelsFromAssets(organizationId, assetLabelsDTO);
                if (updatedAssets == null)
                {
                    return NotFound();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                return Ok(JsonSerializer.Serialize<object>(updatedAssets, options));

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("lifecycles")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<OrigoAssetLifecycle>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(Permission.CanReadAsset)]
        public async Task<ActionResult> GetLifecycles()
        {
            try
            {
                var lifecycles = await _assetServices.GetLifecycles();
                if (lifecycles == null)
                {
                    return NotFound();
                }
                return Ok(lifecycles);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [Route("min-buyout-price")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<MinBuyoutPrice>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(Permission.CanReadAsset)]
        public async Task<ActionResult> GetBaseMinBuyoutPrice([FromQuery] string? country, [FromQuery] int? assetCategoryId)
        {
            try
            {
                var allMinBuyoutPrices = await _assetServices.GetBaseMinBuyoutPrice(country, assetCategoryId);
                if (allMinBuyoutPrices == null)
                {
                    return NotFound();
                }
                return Ok(allMinBuyoutPrices);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }



        [Route("{assetId:Guid}/customers/{organizationId:guid}/ChangeLifecycleType/{newLifecycleType:int}")]
        [HttpPost]
        [ProducesResponseType(typeof(OrigoAsset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateAsset)]
        public async Task<ActionResult> ChangeLifecycleTypeOnAsset([FromRoute] Guid organizationId, [FromRoute] Guid assetId, [FromRoute] int newLifecycleType)
        {
            try
            {
                // Only admin or manager roles are allowed to manage assets
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }

                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid.TryParse(actor, out Guid callerId);

                // talk to frontend and make this an input model on their part.
                // for now, we fill this in here.
                UpdateAssetLifecycleType data = new UpdateAssetLifecycleType
                {
                    AssetId = assetId,
                    CallerId = callerId,
                    LifecycleType = newLifecycleType
                };

                var updatedAsset = await _assetServices.ChangeLifecycleType(organizationId, data.AssetId, data);
                if (updatedAsset == null)
                {
                    return NotFound();
                }
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                return Ok(JsonSerializer.Serialize<object>(updatedAsset, options));
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        /// <summary>
        /// Assign a department or a user to an Asset Life cycle
        /// </summary>
        /// <param name="assetId"></param>
        /// <param name="asset">Needs to have either a departmentId or userId. Can not have id for both and can not be null at the same time.</param>
        /// <param name="organizationId"></param>
        [Route("{assetId:Guid}/customers/{organizationId:guid}/assign")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoAsset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateAsset)]
        public async Task<ActionResult> AssignAsset([FromRoute] Guid organizationId, [FromRoute] Guid assetId, [FromBody] AssignAsset asset)
        {
            try
            {
                // Only admin or manager roles are allowed to manage assets
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }

                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid.TryParse(actor, out Guid callerId);

                AssignAssetToUser data = new AssignAssetToUser
                {
                    AssetId = assetId,
                    CallerId = callerId,
                    UserId = asset.UserId,
                    DepartmentId = asset.DepartmentId
                };

                var assignedAsset = await _assetServices.AssignAsset(organizationId, assetId, data);
                if (assignedAsset == null)
                {
                    return NotFound();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                return Ok(JsonSerializer.Serialize<object>(assignedAsset, options));
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [Route("categories")]
        [HttpGet]
        [ProducesResponseType(typeof(OrigoAssetCategory), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [PermissionAuthorize(Permission.CanReadAsset)]
        public async Task<ActionResult<IEnumerable<OrigoAssetCategory>>> GetAssetCategories([FromQuery] string? language = "EN", [FromQuery] bool includeAttributeData = false)
        {
            var assetCategories = await _assetServices.GetAssetCategoriesAsync(language);

            if (includeAttributeData)
            {
                foreach (OrigoAssetCategory category in assetCategories)
                {
                    category.AssetCategoryAttributes = _assetServices.GetAssetCategoryAttributesForCategory(category.AssetCategoryId);
                }
            }

            if (assetCategories == null)
            {
                return NotFound();
            }
            return Ok(assetCategories);
        }

        [Route("auditLog/{assetId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<AssetAuditLog>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [PermissionAuthorize(Permission.CanReadAsset)]
        public async Task<ActionResult<IEnumerable<AssetAuditLog>>> GetAssetAuditLog([FromRoute] Guid assetId)
        {
            try
            {
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid.TryParse(userId, out Guid callerId);

                var assetAuditLog = await _assetServices.GetAssetAuditLog(assetId, callerId, role);
                return Ok(assetAuditLog);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [Route("customers/{organizationId:guid}/upload")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult> UploadAssetFile([FromRoute] Guid organizationId, [FromForm] IFormFile file)
        {
            try
            {
                // Only admin or manager roles are allowed to import assets
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                await _storageService.UploadAssetsFileAsync(organizationId, file);
                return Ok();
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Azure.RequestFailedException ex)
            {
                return BadRequest("RequestFailedException: Could not upload file to azure: " + ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Exception: Could not upload file due to unknown error: " + ex.Message);
            }
        }

        [Route("customers/{organizationId:guid}/import")]
        [HttpPost]
        [ProducesResponseType(typeof(AssetValidationResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanCreateAsset)]
        public async Task<ActionResult> ImportAssetFile([FromRoute] Guid organizationId, [FromForm] IFormFile assetImportFile, [FromQuery] bool validateOnly = true)
        {
            try
            {
                // Only admin or manager roles are allowed to import assets
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                return Ok(await _assetServices.ImportAssetsFileAsync(organizationId, assetImportFile, validateOnly));
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Azure.RequestFailedException ex)
            {
                return BadRequest("RequestFailedException: Could not import file: " + ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Exception: Could not import file due to unknown error: " + ex.Message);
            }
        }

        [Route("customers/{organizationId:guid}/download")]
        [HttpGet]
        [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanReadAsset)]
        public async Task<ActionResult> DownloadAssetFile([FromRoute] Guid organizationId, [FromQuery] string fileName)
        {
            try
            {
                // Only admin or manager roles are allowed to download files
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var fileStream = await _storageService.GetAssetsFileAsStreamAsync(organizationId, fileName);

                return File(fileStream, "text/html", fileName);
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Azure.RequestFailedException ex)
            {
                return BadRequest("RequestFailedException: Could not download file from azure: " + ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Exception: Could not download file due to unknown error: " + ex.Message);
            }
        }

        [Route("customers/{organizationId:guid}/blob_files")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanReadAsset)]
        public async Task<ActionResult<IList<string>>> GetBlobFiles([FromRoute] Guid organizationId)
        {
            try
            {
                // Only admin or manager roles are allowed to view all files
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var blobList = await _storageService.GetBlobsAsync(organizationId);
                return Ok(blobList);
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Azure.RequestFailedException ex)
            {
                return BadRequest("RequestFailedException: Could not get files from azure with the following message: " + ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Exception: Could not retrieve files due to unknown exception: " + ex.Message);
            }
        }

        [Route("customers/{organizationId:guid}/assets-counter")]
        [HttpGet]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanReadAsset)]
        public async Task<ActionResult<OrigoCustomerAssetsCounter>> GetAssetLifecycleCounters([FromRoute] Guid organizationId, [FromQuery] FilterOptionsForAsset filter)
        {
            try
            {
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (role == PredefinedRole.EndUser.ToString())
                {
                    filter.UserId = "me";
                }

                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();

                if (role != PredefinedRole.SystemAdmin.ToString())
                {

                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }

                }

                if ((role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString()) && accessList != null)
                {
                    filter.Department ??= new List<Guid?>();
                    foreach (var departmentId in accessList)

                    {
                        if (Guid.TryParse(departmentId, out var departmentGuid))
                        {
                            filter.Department.Add(departmentGuid);

                        }
                    }
                }

                filter.UserId = filter.UserId == "me" ? HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value : null;

                var assetCount = await _assetServices.GetAssetLifecycleCountersAsync(organizationId, filter);


                return Ok(assetCount);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("customers/{organizationId:guid}/activate")]
        [HttpPatch]
        [ProducesResponseType(typeof(IList<OrigoAsset>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateAsset)]
        public async Task<ActionResult> ActivateAssetStatusOnAssetLifecycle([FromRoute] Guid organizationId, [FromBody] ChangeAssetStatus assetLifecycles)
        {
            try
            {
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }

                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid.TryParse(actor, out Guid callerId);
                if (!assetLifecycles.AssetLifecycleId.Any())
                    return BadRequest("No assets selected.");

                var changedAssetStatusDTO = _mapper.Map<ChangeAssetStatusDTO>(assetLifecycles);
                changedAssetStatusDTO.CallerId = callerId;

                var activatedAsset = await _assetServices.ActivateAssetStatusOnAssetLifecycle(organizationId, changedAssetStatusDTO);

                return Ok(activatedAsset);
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Unable to change status on assets");
            }
        }
        [Route("customers/{organizationId:guid}/deactivate")]
        [HttpPatch]
        [ProducesResponseType(typeof(IList<HardwareSuperType>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateAsset)]
        public async Task<ActionResult> DeactivateAssetStatusOnAssetLifecycle([FromRoute] Guid organizationId, [FromBody] ChangeAssetStatus assetLifecycles)
        {
            try
            {
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }

                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid.TryParse(actor, out Guid callerId);
                if (!assetLifecycles.AssetLifecycleId.Any())
                    return BadRequest("No assets selected.");

                var changedAssetStatusDTO = _mapper.Map<ChangeAssetStatusDTO>(assetLifecycles);
                changedAssetStatusDTO.CallerId = callerId;

                var deactivatedAsset = await _assetServices.DeactivateAssetStatusOnAssetLifecycle(organizationId, changedAssetStatusDTO);

                return Ok(deactivatedAsset);
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Unable to change status on assets");
            }
        }
    }
}