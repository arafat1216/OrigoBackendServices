using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrigoApiGateway.Exceptions;
using OrigoApiGateway.Models.ProductCatalog;
using OrigoApiGateway.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OrigoApiGateway.Controllers
{

    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("origoapi/v{version:apiVersion}/products")]
    public class ProductCatalogController : ControllerBase
    {
        private readonly ILogger<ProductCatalogController> _logger;
        private readonly IProductCatalogServices _productCatalogServices;


        public ProductCatalogController(ILogger<ProductCatalogController> logger, IProductCatalogServices productCatalogServices)
        {
            _logger = logger;
            _productCatalogServices = productCatalogServices;
        }


        #region Features

        [HttpGet("organization/{organizationId}/permissions")]
        public async Task<ActionResult<IEnumerable<string>>> GetProductPermissionsByOrganization([FromRoute] Guid organizationId)
        {
            try
            {
                return Ok(await _productCatalogServices.GetProductPermissionsForOrganizationAsync(organizationId));
            }
            catch (MicroserviceErrorResponseException e)
            {
                return ExceptionResponseBuilder(e);
            }
        }

        #endregion


        #region Product Types

        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<ProductTypeGet>>> GetProductTypes()
        {
            try
            {
                return Ok(await _productCatalogServices.GetAllProductTypesAsync());
            }
            catch (MicroserviceErrorResponseException e)
            {
                return ExceptionResponseBuilder(e);
            }
        }

        #endregion


        #region Products

        [HttpGet("{productId}")]
        public async Task<ActionResult> GetProductById([FromRoute] int productId)
        {
            try
            {
                return Ok(await _productCatalogServices.GetProductByIdAsync(productId));
            }
            catch (MicroserviceErrorResponseException e)
            {
                return ExceptionResponseBuilder(e);
            }
        }


        [HttpGet("partner/{partnerId}")]
        public async Task<ActionResult<IEnumerable<ProductGet>>> GetProductsByPartner([FromRoute] Guid partnerId)
        {
            try
            {
                return Ok(await _productCatalogServices.GetAllProductsByPartnerAsync(partnerId));
            }
            catch (MicroserviceErrorResponseException e)
            {
                return ExceptionResponseBuilder(e);
            }
        }

        #endregion


        #region Orders


        [HttpPut("partner/{partnerId}/organization/{organizationId}")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status404NotFound, "One or more products does not exist, or is not available using the provided details.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "One or more of the product requirements is not fulfilled.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetCurrentProductOrdersByOrganization([FromRoute] Guid partnerId, [FromRoute] Guid organizationId, [FromBody] ProductOrdersPut productOrders)
        {
            try
            {
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                var parseSuccess = Guid.TryParse(actor, out Guid actorResult);

                // In the event that we cannon retrieve or parse the user's UUID, throw an exception and log the problem.
                if (!parseSuccess)
                {
                    _logger.LogError($"{nameof(GetCurrentProductOrdersByOrganization)} failed as the actor's claim could not be retrieved and/or parsed to a valid user UUID. Unique location ID: 'F7E971AF-3FF2-4259-AF0F-171A5A534B8F'");
                    return Problem(statusCode: 500);
                }

                var productOrdersDTO = new ProductOrdersDTO(productOrders, actorResult);

                await _productCatalogServices.ReplaceOrderedProductsAsync(partnerId, organizationId, productOrdersDTO);
                return NoContent();
            }
            catch (MicroserviceErrorResponseException e)
            {
                return ExceptionResponseBuilder(e);
            }
        }


        #endregion



        private ActionResult ExceptionResponseBuilder(MicroserviceErrorResponseException exception)
        {
            // Try and parse the error code, and set it to 500 if it's null.
            int statusCode = exception.StatusCode.HasValue ? (int)exception.StatusCode : 500;

            // If the status code was 207 (Multi Status), or it was 300 or above, then it's been explicitly thrown by the
            // internal micro-service, meaning we can forward it to the frontend.
            // Important: Error 500 is excluded, since this is the error-code thrown by exceptions.
            if (statusCode == 207 || (statusCode != 500 && statusCode >= 300))
            {
                return StatusCode(statusCode, exception.Message);
            }
            else
            {
                return Problem(statusCode: 500);
            }
        }
    }
}
