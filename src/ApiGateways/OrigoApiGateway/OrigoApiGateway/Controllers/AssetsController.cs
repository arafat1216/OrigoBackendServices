using Common.Enums;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrigoApiGateway.Authorization;
using OrigoApiGateway.Models;
using OrigoApiGateway.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
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

        public AssetsController(ILogger<AssetsController> logger, IAssetServices assetServices)
        {
            _logger = logger;
            _assetServices = assetServices;
        }

        [Route("customers/{organizationId:guid}/search")]
        [HttpGet]
        [ProducesResponseType(typeof(OrigoPagedAssets), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanReadAsset)]
        public async Task<ActionResult<OrigoPagedAssets>> SearchForAsset(Guid organizationId, string search, int page = 1, int limit = 50)
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
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var origoPagedAssets = await _assetServices.SearchForAssetsForCustomerAsync(organizationId, search, page, limit);
                if (origoPagedAssets == null)
                {
                    return NotFound();
                }

                return Ok(origoPagedAssets);
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
        public async Task<ActionResult<IList<OrigoAsset>>> Get(Guid organizationId, Guid userId)
        {
            try
            {
                // All roles have access, as long as customer is in their accessList
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }
                var assets = await _assetServices.GetAssetsForUserAsync(organizationId, userId);
                if (assets == null)
                {
                    return NotFound();
                }

                var options = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Auto
                };
                return Ok(JsonConvert.SerializeObject(assets, options));
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [Route("customers/{organizationId:guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<OrigoAsset>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanReadAsset)]
        public async Task<ActionResult<IList<OrigoAsset>>> Get(Guid organizationId)
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
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var assets = await _assetServices.GetAssetsForCustomerAsync(organizationId);
                if (assets == null)
                {
                    return NotFound();
                }

                var options = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Auto
                };
                return Ok(JsonConvert.SerializeObject(assets, options));
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
        public async Task<ActionResult<OrigoAsset>> GetAsset(Guid organizationId, Guid assetId)
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
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var asset = await _assetServices.GetAssetForCustomerAsync(organizationId, assetId);
                if (asset == null)
                {
                    return NotFound();
                }

                var options = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Auto
                };
                return Ok(JsonConvert.SerializeObject(asset, options));
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
        public async Task<ActionResult> CreateAsset(Guid organizationId, [FromBody] NewAsset asset)
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
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var createdAsset = await _assetServices.AddAssetForCustomerAsync(organizationId, asset);
                if (createdAsset != null)
                {
                    var options = new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        TypeNameHandling = TypeNameHandling.Auto
                    };
                    return CreatedAtAction(nameof(CreateAsset), new { id = createdAsset.Id }, JsonConvert.SerializeObject(createdAsset, options));
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [Route("{assetId:Guid}/customers/{organizationId:guid}/assetStatus/{assetStatus:int}")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoAsset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateAsset)]
        public async Task<ActionResult> SetAssetStatusOnAsset(Guid organizationId, Guid assetId, int assetStatus)
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
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var updatedAsset = await _assetServices.UpdateAssetStatus(organizationId, assetId, assetStatus);
                if (updatedAsset == null)
                {
                    return NotFound();
                }
                var options = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Auto
                };
                return Ok(JsonConvert.SerializeObject(updatedAsset, options));
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [Route("{assetId:Guid}/customers/{organizationId:guid}/Activate/{isActive:bool}")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoAsset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateAsset)]
        public async Task<ActionResult> SetActiveStatusOnAsset(Guid organizationId, Guid assetId, bool isActive)
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
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var updatedAsset = await _assetServices.GetAssetForCustomerAsync(organizationId, assetId);
                if (updatedAsset == null)
                {
                    return NotFound();
                }
                updatedAsset.IsActive = isActive;
                var options = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Auto
                };
                return Ok(JsonConvert.SerializeObject(updatedAsset, options));

            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [Route("{assetId:Guid}/customers/{organizationId:guid}/Update")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoAsset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateAsset)]
        public async Task<ActionResult> UpdateAsset(Guid organizationId, Guid assetId, [FromBody] OrigoUpdateAsset asset)
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
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var updatedAsset = await _assetServices.UpdateAssetAsync(organizationId, assetId, asset);
                if (updatedAsset == null)
                {
                    return NotFound();
                }

                var options = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Auto
                };
                return Ok(JsonConvert.SerializeObject(updatedAsset, options));
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
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

        [Route("{assetId:Guid}/customers/{organizationId:guid}/ChangeLifecycleType/{newLifecycleType:int}")]
        [HttpPost]
        [ProducesResponseType(typeof(OrigoAsset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateAsset)]
        public async Task<ActionResult> ChangeLifecycleTypeOnAsset(Guid organizationId, Guid assetId, int newLifecycleType)
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
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var updatedAsset = await _assetServices.ChangeLifecycleType(organizationId, assetId, newLifecycleType);
                if (updatedAsset == null)
                {
                    return NotFound();
                }
                var options = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Auto
                };
                return Ok(JsonConvert.SerializeObject(updatedAsset, options));
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [Route("{assetId:Guid}/customers/{organizationId:guid}/assign")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoAsset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateAsset)]
        public async Task<ActionResult> AssignAsset(Guid organizationId, Guid assetId, Guid? userId)
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
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var assignedAsset = await _assetServices.AssignAsset(organizationId, assetId, userId);
                if (assignedAsset == null)
                {
                    return NotFound();
                }

                var options = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Auto
                };
                return Ok(JsonConvert.SerializeObject(assignedAsset, options));
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
        public async Task<ActionResult<IEnumerable<OrigoAssetCategory>>> GetAssetCategories()
        {
            var assetCategories = await _assetServices.GetAssetCategoriesAsync();
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
        public async Task<ActionResult<IEnumerable<AssetAuditLog>>> GetAssetAuditLogMock(Guid assetId)
        {
            try
            {
                var assetAuditLog = await _assetServices.GetAssetAuditLog(assetId);
                return Ok(assetAuditLog);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }
    }
}