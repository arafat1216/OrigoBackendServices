using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Domain.Orders;
using ProductCatalog.Domain.Products;
using ProductCatalog.Domain.ProductTypes;
using ProductCatalog.Infrastructure;

namespace ProductCatalog.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class OrdersController : ControllerBase
    {
        [HttpGet]
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


        [HttpPut("organization/{organizationId}/")]
        public async Task<ActionResult> ReplaceOrderedProducts([FromRoute] Guid organizationId, [FromBody] UpdateProductOrders updateProductOrders)
        {
            try
            {
                var service = new OrderService();
                await service.UpdateOrderedProductsAsync(organizationId, updateProductOrders);

                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception e)
            {
                var ff = e.GetType();

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


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
