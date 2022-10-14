using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Common.Exceptions;
using ProductCatalog.Common.Orders;
using ProductCatalog.Common.Products;
using ProductCatalog.Infrastructure;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;

namespace ProductCatalog.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [SwaggerTag("Actions for placing, changing and handling product-orders.")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class OrdersController : ControllerBase
    {
        private readonly JsonSerializerOptions options = new JsonSerializerOptions()
        {
#if DEBUG
            WriteIndented = true,
#endif

            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };


        /// <summary>
        ///     Lists all active orders.
        /// </summary>
        /// <remarks>
        ///     List every order that is currently active.
        /// </remarks>
        /// <returns> A collection of all active orders. </returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderGet>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderGet>>> GetOrdersAsync()
        {
            try
            {
                var result = await new OrderService().GetOrders(null, null);

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        ///     Lists all products that also has active orders.
        /// </summary>
        /// <remarks>
        ///     Lists all products that currently has an active order. The result includes data from all partners and organizations.
        /// </remarks>
        /// <param name="includeTranslations">
        ///     When <c><see langword="true"/></c>, the <c>Translations</c> property is loaded/included in the retrieved data. 
        ///     
        ///     <para>
        ///     This property contains all localization-values for the product. 
        /// </para></param>
        /// <returns> A collection containing all corresponding products. </returns>
        [HttpGet("products")]
        [ProducesResponseType(typeof(IEnumerable<ProductGet>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductGet>>> GetOrderedProductsAsync([FromQuery] bool includeTranslations = true)
        {
            try
            {
                var result = await new OrderService().GetOrderedProductsAsync(includeTranslations, null, null);

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        ///     Lists all products that also has active orders, filtered by partner.
        /// </summary>
        /// <remarks>
        ///     Lists all products that currently has an active order. The result only includes data from the partner provided 
        ///     in <code><paramref name="partnerId"/></code>.
        /// </remarks>
        /// <param name="partnerId"> The partner used for filtering product-orders. </param>
        /// <param name="includeTranslations">
        ///     When <c><see langword="true"/></c>, the <c>Translations</c> property is loaded/included in the retrieved data. 
        ///     
        ///     <para>
        ///     This property contains all localization-values for the product. 
        /// </para></param>
        /// <returns> A collection containing all corresponding products. </returns>
        [HttpGet("products/partner/{partnerId}")]
        [ProducesResponseType(typeof(IEnumerable<ProductGet>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductGet>>> GetOrderedProductsByPartnerAsync([FromRoute] Guid partnerId, [FromQuery] bool includeTranslations = true)
        {
            try
            {
                var result = await new OrderService().GetOrderedProductsAsync(includeTranslations, null, partnerId);

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        ///     Lists all products that has been ordered by an organization.
        /// </summary>
        /// <remarks>
        ///     Retrieves all products that is currently ordered by a specific organization. This is filtered based on 
        ///     the provided <code><paramref name="organizationId"/></code>.
        /// </remarks>
        /// <param name="organizationId"> The organization to retrieve orders for. </param>
        /// <param name="includeTranslations">
        ///     When <c><see langword="true"/></c>, the <c>Translations</c> property is loaded/included in the retrieved data. 
        ///     
        ///     <para>
        ///     This property contains all localization-values for the product. 
        /// </para></param>
        /// <returns> A collection of all corresponding products. </returns>
        [HttpGet("products/organization/{organizationId}/")]
        [ProducesResponseType(typeof(IEnumerable<ProductGet>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductGet>>> GetOrderedProductsByOrganizationAsync([FromRoute] Guid organizationId, [FromQuery] bool includeTranslations = true)
        {
            try
            {
                var result = await new OrderService().GetOrderedProductsAsync(includeTranslations, organizationId, null);

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        ///     Lists all products that has been ordered by an organization, filtered by a partner-ID.
        /// </summary>
        /// <remarks>
        ///     Retrieves all products that is currently ordered by a specific organization. This is filtered based on 
        ///     the provided <code><paramref name="organizationId"/></code> and <code><paramref name="partnerId"/></code>.
        /// </remarks>
        /// <param name="partnerId"> The partner the orders are restricted to. </param>
        /// <param name="organizationId"> The organization to retrieve orders for. </param>
        /// <param name="includeTranslations">
        ///     When <c><see langword="true"/></c>, the <c>Translations</c> property is loaded/included in the retrieved data. 
        ///     
        ///     <para>
        ///     This property contains all localization-values for the product. 
        /// </para></param>
        /// <returns> A collection containing all corresponding products. </returns>
        [HttpGet("partner/{partnerId}/organization/{organizationId}")]
        [ProducesResponseType(typeof(IEnumerable<ProductGet>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductGet>>> GetOrderedProductsByPartnerAndOrganizationAsync([FromRoute] Guid partnerId, [FromRoute] Guid organizationId, [FromQuery] bool includeTranslations = true)
        {
            try
            {
                var result = await new OrderService().GetOrderedProductsAsync(includeTranslations, organizationId, partnerId);

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> ReplaceOrderedProductsAsync([FromRoute] Guid partnerId, [FromRoute] Guid organizationId, [FromBody] UpdateProductOrders productOrders)
        {
            try
            {
                await new OrderService().UpdateOrderedProductsAsync(organizationId, partnerId, productOrders);

                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (EntityNotFoundException)
            {
                return StatusCode(StatusCodes.Status404NotFound, "One or more products was not found.");
            }
            catch (RequirementNotFulfilledException)
            {
                return StatusCode(StatusCodes.Status409Conflict, "The product-requirements was not fulfilled.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
