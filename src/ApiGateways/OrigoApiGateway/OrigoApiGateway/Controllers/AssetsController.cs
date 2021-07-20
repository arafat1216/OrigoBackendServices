using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrigoApiGateway.Models;
using OrigoApiGateway.Services;
// ReSharper disable RouteTemplates.RouteParameterConstraintNotResolved
// ReSharper disable RouteTemplates.ControllerRouteParameterIsNotPassedToMethods

namespace OrigoApiGateway.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    //[Authorize]
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


        [Route("customers/{customerId:guid}/{userId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<OrigoAsset>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IList<OrigoAsset>>> Get(Guid customerId, Guid userId)
        {
            try
            {
                var assets = await _assetServices.GetAssetsForUserAsync(customerId, userId);
                if (assets == null)
                {
                    return NotFound();
                }

                return Ok(assets);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("customers/{customerId:guid}/search")]
        [HttpGet]
        [ProducesResponseType(typeof(OrigoPagedAssets), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<OrigoPagedAssets>> SearchForAsset(Guid customerId, string search, int page = 1, int limit = 50)
        {
            try
            {
                var origoPagedAssets = await _assetServices.SearchForAssetsForCustomerAsync(customerId, search, page, limit);
                if (origoPagedAssets == null)
                {
                    return NotFound();
                }

                return Ok(origoPagedAssets);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("customers/{customerId:guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<OrigoAsset>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IList<OrigoAsset>>> Get(Guid customerId)
        {
            try
            {
                var assets = await _assetServices.GetAssetsForCustomerAsync(customerId);
                if (assets == null)
                {
                    return NotFound();
                }

                return Ok(assets);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{assetId:guid}/customers/{customerId:guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<OrigoAsset>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<OrigoAsset>> GetAsset(Guid customerId, Guid assetId)
        {
            try
            {
                var asset = await _assetServices.GetAssetForCustomerAsync(customerId, assetId);
                if (asset == null)
                {
                    return NotFound();
                }

                return Ok(asset);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("customers/{customerId:guid}")]
        [HttpPost]
        [ProducesResponseType(typeof(OrigoAsset), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CreateAsset(Guid customerId, [FromBody] NewAsset asset)
        {
            try
            {
                var createdAsset = await _assetServices.AddAssetForCustomerAsync(customerId, asset);
                if (createdAsset != null)
                {
                    return CreatedAtAction(nameof(CreateAsset), new { id = createdAsset.Id }, createdAsset);
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{assetId:Guid}/customers/{customerId:guid}/Activate/{isActive:bool}")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoAsset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> SetActiveStatusOnAsset(Guid customerId, Guid assetId, bool isActive)
        {
            try
            {
                var updatedAsset = await _assetServices.UpdateActiveStatus(customerId, assetId, isActive);
                if (updatedAsset == null)
                {
                    return NotFound();
                }

                return Ok(updatedAsset);

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{assetId:Guid}/customers/{customerId:guid}/Update")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoAsset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> UpdateAsset(Guid customerId, Guid assetId, [FromBody] OrigoUpdateAsset asset)
        {
            try
            {
                var updatedAsset = await _assetServices.UpdateAssetAsync(customerId, assetId, asset);
                if (updatedAsset == null)
                {
                    return NotFound();
                }

                return Ok(updatedAsset);
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
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{assetId:Guid}/customers/{customerId:guid}/ChangeLifecycleType/{newLifecycleType:int}")]
        [HttpPost]
        [ProducesResponseType(typeof(OrigoAsset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> ChangeLifecycleTypeOnAsset(Guid customerId, Guid assetId, int newLifecycleType)
        {
            try
            {
                var updatedAsset = await _assetServices.ChangeLifecycleType(customerId, assetId, newLifecycleType);
                if (updatedAsset == null)
                {
                    return NotFound();
                }
                return Ok(updatedAsset);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{assetId:Guid}/customers/{customerId:guid}/assign")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoAsset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> AssignAsset(Guid customerId, Guid assetId, Guid? userId)
        {
            try
            {
                var assignedAsset = await _assetServices.AssignAsset(customerId, assetId, userId);
                if (assignedAsset == null)
                {
                    return NotFound();
                }

                return Ok(assignedAsset);

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("categories")]
        [HttpGet]
        [ProducesResponseType(typeof(OrigoAssetCategory), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
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
        public async Task<ActionResult<IEnumerable<AssetAuditLog>>> GetAssetAuditLogMock(Guid assetId)
        {
            try
            {
                var assetAuditLog = await _assetServices.GetAssetAuditLog(assetId);
                return Ok(assetAuditLog);
            }
            catch (Exception)
            {
                return BadRequest();
            }
           
        }
    }
}