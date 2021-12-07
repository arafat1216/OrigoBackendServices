using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Domain.Exceptions;
using ProductCatalog.Domain.Orders;
using ProductCatalog.Domain.Products;
using ProductCatalog.Domain.ProductTypes;
using ProductCatalog.Infrastructure;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductCatalog.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class OrdersController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ProductGet>>> GetAllOrders()
        {
            try
            {
                var service = new OrderService();
                var result = await service.GetOrderedProductsAsync(null, null);

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpGet("partner/{partnerId}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ProductGet>>> GetAllOrdersByPartner([FromRoute] Guid partnerId)
        {
            try
            {
                var service = new OrderService();
                var result = await service.GetOrderedProductsAsync(null, partnerId);

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        // TODO: Rework to support partner ID
        [HttpPut("partner/{partnerId}/organization/{organizationId}")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status404NotFound, "One or more products does not exist, or is not available using the provided details.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "One or more of the product requirements is not fulfilled.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ReplaceOrderedProducts([FromRoute] Guid partnerId, [FromRoute] Guid organizationId, [FromBody] UpdateProductOrders updateProductOrders)
        {
            try
            {
                var service = new OrderService();
                await service.UpdateOrderedProductsAsync(organizationId, partnerId, updateProductOrders);

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


        // TODO: Add a duplicate version that supports partner ID
        [HttpGet("organization/{organizationId}/")]
        public async Task<ActionResult<IEnumerable<ProductGet>>> GetOrderedProducts([FromRoute] Guid organizationId)
        {
            try
            {
                var service = new OrderService();
                var result = await service.GetOrderedProductsAsync(organizationId, null);

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }
    }
}
