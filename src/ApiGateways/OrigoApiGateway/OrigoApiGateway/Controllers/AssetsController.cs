using AutoMapper;
using Common.Enums;
using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrigoApiGateway.Authorization;
using OrigoApiGateway.Exceptions;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.Asset;
using OrigoApiGateway.Models.BackendDTO;
using OrigoApiGateway.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
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
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;

        public AssetsController(ILogger<AssetsController> logger, IAssetServices assetServices, IStorageService storageService, IMapper mapper)
        {
            _logger = logger;
            _assetServices = assetServices;
            _storageService = storageService;
            _mapper = mapper;
        }

        [Route("customers/count")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<CustomerAssetCount>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanReadAsset)]
        public async Task<ActionResult<IList<CustomerAssetCount>>> GetAllCustomerItemCount()
        {
            try
            {
                // only SystemAdmin has access
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    return Forbid();
                }

                IList<CustomerAssetCount> assetCountList = await _assetServices.GetAllCustomerAssetsCountAsync();
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
        [ProducesResponseType(typeof(IList<OrigoAsset>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanReadAsset)]
        public async Task<ActionResult<IList<OrigoAsset>>> GetCustomerItemCount(Guid organizationId)
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
                var count = await _assetServices.GetAssetsCountAsync(organizationId);
                return Ok(new { organizationId, count });
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
        public async Task<ActionResult<CustomerAssetValue>> GetCustomerTotalBookValue(Guid organizationId)
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
                var totalBookValue = await _assetServices.GetCustomerTotalBookValue(organizationId);
                return Ok(new CustomerAssetValue(){ OrganizationId = organizationId, Amount = totalBookValue });
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
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
        public async Task<ActionResult> Get(Guid organizationId, [FromQuery(Name = "q")] string search = "", int page = 1, int limit = 1000)
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

                var assets = await _assetServices.GetAssetsForCustomerAsync(organizationId, search, page, limit);
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
        [ProducesResponseType(typeof(OrigoAsset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateAsset)]
        public async Task<ActionResult> SetAssetStatusOnAssets(Guid organizationId, [FromBody] UpdateAssetsStatus updatedAssetStatus)
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

        [Route("{assetId:Guid}/customers/{organizationId:guid}/Update")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoAsset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
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
        public async Task<ActionResult<IList<Label>>> GetLabelsForCustomer(Guid organizationId)
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
        public async Task<ActionResult<IList<Label>>> CreateLabelsForCustomer(Guid organizationId, IList<NewLabel> labels)
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
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
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
        public async Task<ActionResult<IList<Label>>> DeleteLabelsForCustomer(Guid organizationId, IList<Guid> labelGuids)
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
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
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
        public async Task<ActionResult<IList<Label>>> UpdateLabelsForCustomer(Guid organizationId, IList<Label> labels)
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
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
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
        public async Task<ActionResult<IList<OrigoAsset>>> AssignLabelsToAssets(Guid organizationId, AssetLabels assetLabels)
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
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
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
        public async Task<ActionResult<IList<OrigoAsset>>> UnAssignLabelsToAssets(Guid organizationId, AssetLabels assetLabels)
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
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
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

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid.TryParse(actor, out Guid callerId);

                // Input model for assigning asset to a user
                // Talk to frontend about using this model.
                AssignAssetToUser data = new AssignAssetToUser
                {
                    AssetId = assetId,
                    CallerId = callerId,
                    UserId = userId
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
        public async Task<ActionResult<IEnumerable<OrigoAssetCategory>>> GetAssetCategories(bool includeAttributeData = false)
        {
            var assetCategories = await _assetServices.GetAssetCategoriesAsync();

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
        public async Task<ActionResult<IEnumerable<AssetAuditLog>>> GetAssetAuditLog(Guid assetId)
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

        [Route("customers/{organizationId:guid}/upload")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult> UploadAssetFile(Guid organizationId, IFormFile file)
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
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
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

        [Route("customers/{organizationId:guid}/download")]
        [HttpGet]
        [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanReadAsset)]
        public async Task<ActionResult> DownloadAssetFile(Guid organizationId, string fileName)
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
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
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
        public async Task<ActionResult<IList<string>>> GetBlobFiles(Guid organizationId)
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
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
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
    }
}