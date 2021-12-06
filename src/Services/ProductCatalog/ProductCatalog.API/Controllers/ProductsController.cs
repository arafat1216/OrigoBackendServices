using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Domain.Generic;
using ProductCatalog.Domain.Products;
using ProductCatalog.Domain.ProductTypes;
using ProductCatalog.Infrastructure;
using System.Net;

namespace ProductCatalog.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductsController : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductGet>>> GetAllProductsAsync()
        {
            try
            {
                var service = new ProductService();
                var result = await service.GetAllProductsAsync(null);

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpGet("{productId}")]
        public async Task<ActionResult<ProductGet?>> GetProductAsync(int productId)
        {
            try
            {
                var service = new ProductService();
                var result = await service.GetProductAsync(productId);

                if (result is null)
                    return NotFound();
                else
                    return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpGet("partner/{partnerId}")]
        public async Task<ActionResult<IEnumerable<ProductGet>>> GetPartnerProductsAsync([FromRoute] Guid partnerId)
        {
            try
            {
                var service = new ProductService();
                var result = await service.GetAllProductsAsync(partnerId);

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


    }
}
