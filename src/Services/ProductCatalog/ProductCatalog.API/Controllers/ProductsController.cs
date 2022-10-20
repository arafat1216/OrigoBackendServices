using Microsoft.AspNetCore.Mvc;
using ProductCatalog.API.Filters;
using ProductCatalog.Common.Products;
using ProductCatalog.Infrastructure;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;

namespace ProductCatalog.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [SwaggerTag("Actions for handling products and their translations.")]
    [ServiceFilter(typeof(ErrorExceptionFilter))]
    public class ProductsController : ControllerBase
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
        ///     Retrieves a product based on it's ID.
        /// </summary>
        /// <remarks>
        ///     Retrieves a single product by it's <code><paramref name="productId"/></code>.
        /// </remarks>
        /// <param name="productId"> The ID for the product. </param>
        /// <returns> If found, the corresponding product. </returns>
        [HttpGet("{productId}")]
        [ProducesResponseType(typeof(ProductGet), StatusCodes.Status200OK)]
        public async Task<ActionResult<ProductGet?>> GetByIdAsync(int productId)
        {
            var result = await new ProductService().GetByIdAsync(productId);

            if (result is null)
                return NotFound();
            else
                return Ok(result);

        }


        /// <summary>
        ///     Retrieves all products.
        /// </summary>
        /// <remarks>
        ///     Retrieves a list of all products in the system.
        /// </remarks>
        /// <returns> A collection of all products. </returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductGet>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductGet>>> GetAllAsync()
        {
            var result = await new ProductService().GetAllAsync(null);
            return Ok(result);
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
        [ProducesResponseType(typeof(IEnumerable<ProductGet>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductGet>>> GetAllByPartnerAsync([FromRoute] Guid partnerId)
        {
            var result = await new ProductService().GetAllAsync(partnerId);
            return Ok(result);
        }

    }
}
