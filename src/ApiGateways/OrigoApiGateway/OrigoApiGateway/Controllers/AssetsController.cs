﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
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
    [Route("/origoapi/v{version:apiVersion}/Customers/{customerId:guid}/[controller]")]
    public class AssetsController : ControllerBase
    {
        private readonly ILogger<AssetsController> _logger;
        private readonly IAssetServices _assetServices;

        public AssetsController(ILogger<AssetsController> logger, IAssetServices assetServices)
        {
            _logger = logger;
            _assetServices = assetServices;
        }


        [Route("{userId:Guid}")]
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

        [HttpPost]
        [ProducesResponseType(typeof(OrigoAsset), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CreateAsset(Guid customerId, [FromBody] NewAsset asset)
        {
            _logger.LogInformation($"Information from GetCustomerAsync {{userId}}");
            var createdAsset = await _assetServices.AddAssetForCustomerAsync(customerId, asset);
            if (createdAsset != null)
            {
                return CreatedAtAction(nameof(CreateAsset), new { id = createdAsset.Id }, createdAsset);
            }
            return BadRequest();
        }
    }
}