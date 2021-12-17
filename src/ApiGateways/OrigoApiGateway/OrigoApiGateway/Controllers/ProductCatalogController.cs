using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrigoApiGateway.Exceptions;
using OrigoApiGateway.Models.ProductCatalog;
using OrigoApiGateway.Services;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OrigoApiGateway.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("origoapi/v{version:apiVersion}/products")]
    [SwaggerTag("Actions for handling features, permission sets and corresponding translations.")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status504GatewayTimeout)]
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

        /// <summary>
        ///     Resolves all permission-nodes for a given organization.
        /// </summary>
        /// <remarks>
        ///     Resolves and returns all permission nodes for a given organization.
        /// </remarks>
        /// <param name="organizationId"> The organization you are retrieving permission-nodes for. </param>
        /// <returns> A list containing all permission-nodes for the given organization. </returns>
        [HttpGet("organization/{organizationId}/permissions")]
        [SwaggerOperation(
            Tags = new[] { "Product Catalog: Features" }
        )]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
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


        /// <summary>
        ///     Retrieves all product-types.
        /// </summary>
        /// <remarks>
        ///     Retrieves a list of all product-types.
        /// </remarks>
        /// <returns> A collection of all product-types. </returns>
        [HttpGet("types")]
        [SwaggerOperation(
            Tags = new[] { "Product Catalog: Product Types" }
        )]
        [ProducesResponseType(typeof(IEnumerable<ProductTypeGet>), StatusCodes.Status200OK)]
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

        /// <summary>
        ///     Retrieves a product based on it's ID.
        /// </summary>
        /// <remarks>
        ///     Retrieves a single product by it's <code><paramref name="productId"/></code>.
        /// </remarks>
        /// <param name="productId"> The ID for the product. </param>
        /// <returns> If found, the corresponding product. </returns>
        [HttpGet("{productId}")]
        [SwaggerOperation(
            Tags = new[] { "Product Catalog: Products" }
        )]
        [ProducesResponseType(typeof(ProductGet), StatusCodes.Status200OK)]
        public async Task<ActionResult<ProductGet>> GetProductById([FromRoute] int productId)
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


        /// <summary>
        ///     Retrieves all products for a partner.
        /// </summary>
        /// <remarks>
        ///     Retrieves a list of all products in the system that belongs to a specific partner.
        /// </remarks>
        /// <param name="partnerId"> The ID for the partner. </param>
        /// <returns> Returns a collection containing all products for the partner. </returns>
        [HttpGet("partner/{partnerId}")]
        [SwaggerOperation(
            Tags = new[] { "Product Catalog: Products" }
        )]
        [ProducesResponseType(typeof(IEnumerable<ProductGet>), StatusCodes.Status200OK)]
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


        /// <summary>
        ///     Lists all products that has been ordered by an organization, filtered by a partner-ID.
        /// </summary>
        /// <remarks>
        ///     Retrieves all products that is currently ordered by a specific organization. This is filtered based on 
        ///     the provided <code><paramref name="organizationId"/></code> and <code><paramref name="partnerId"/></code>.
        /// </remarks>
        /// <param name="partnerId"> The partner the orders are restricted to. </param>
        /// <param name="organizationId"> The organization to retrieve orders for. </param>
        /// <returns> A collection containing all corresponding products. </returns>
        [HttpGet("partner/{partnerId}/organization/{organizationId}")]
        [SwaggerOperation(
            Tags = new[] { "Product Catalog: Orders" }
        )]
        [ProducesResponseType(typeof(IEnumerable<ProductGet>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductGet>>> GetOrderedProductsByPartnerAndOrganizationAsync([FromRoute] Guid partnerId, [FromRoute] Guid organizationId)
        {
            try
            {
                return Ok(await _productCatalogServices.GetOrderedProductsByPartnerAndOrganizationAsync(partnerId, organizationId));
            }
            catch (MicroserviceErrorResponseException e)
            {
                return ExceptionResponseBuilder(e);
            }
        }


        /// <summary>
        ///     Replaces the existing product-orders for an organization.
        /// </summary>
        /// <remarks>
        ///     Replace all current product-orders for a given organization, and replaces them with a new configuration.
        ///     
        /// 
        ///     This only affects products for the provided <code><paramref name="partnerId"/></code>. 
        ///     Products that belongs to other partners is not affected.
        /// </remarks>
        /// <param name="partnerId"> The partner that is placing the order. </param>
        /// <param name="organizationId"> The organization that is updated with a new product-configuration. </param>
        /// <param name="productOrders"> The object that contains the new order-details. </param>
        /// <returns> The <see cref="ActionResult"/>. </returns>
        /// 
        /// <response code="404"> One or more products does not exist, or is not available using the provided details. </response>
        /// <response code="409"> One or more of the product requirements is not fulfilled. </response>
        [HttpPut("partner/{partnerId}/organization/{organizationId}")]
        [SwaggerOperation(
            Tags = new[] { "Product Catalog: Orders" }
        )]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> ReplaceOrderedProductsAsync([FromRoute] Guid partnerId, [FromRoute] Guid organizationId, [FromBody] ProductOrdersPut productOrders)
        {
            try
            {
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                var parseSuccess = Guid.TryParse(actor, out Guid actorResult);

                // In the event that we cannon retrieve or parse the user's UUID, throw an exception and log the problem.
                if (!parseSuccess)
                {
                    _logger.LogError($"{nameof(ReplaceOrderedProductsAsync)} failed as the actor's claim could not be retrieved and/or parsed to a valid user UUID. Unique location ID: 'F7E971AF-3FF2-4259-AF0F-171A5A534B8F'");
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


        /// <summary>
        ///     Parses a provided <see cref="MicroserviceErrorResponseException"/>, and attempts to creates a corresponding <see cref="ActionResult"/> response.
        ///     The response will contain the proper status-code and message.
        /// </summary>
        /// <param name="exception"> The exception that is used to generate the response. </param>
        /// <returns> A <see cref="ActionResult"/> that corresponds to the exception information. </returns>
        private ActionResult ExceptionResponseBuilder(MicroserviceErrorResponseException exception)
        {
            if (exception.StatusCode.HasValue && exception.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                _logger.LogError(exception, $"Encountered an unexpected exception.\nUnique location ID: 731B4BE3-D358-4104-AAF7-96041BD07102.\nMessage:\n{exception.Message}");

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
