using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Domain.Generic;
using ProductCatalog.Domain.ProductTypes;
using ProductCatalog.Infrastructure;

namespace ProductCatalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypesController : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductTypeGet>>> GetAllProductTypesAsync([FromQuery] IEnumerable<string>? languages)
        {
            try
            {
                var service = new ProductService();
                var result = await service.GetAllProductTypesAsync(languages);

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpPost]
        public async Task<ActionResult<ProductTypeGet>> AddProductTypeAsync(ProductTypePost productType)
        {
            try
            {
                var service = new ProductService();
                var result = await service.AddProductTypeAsync(productType);

                return StatusCode(StatusCodes.Status201Created, result);

                // TODO: Fix a proper Created() response.
                //return Created();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ProductTypeGet>> GetSingleProductType([FromRoute] int id)
        {
            try
            {
                var service = new ProductService();
                var result = service.GetProductTypeAsync(id);

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


        [HttpPatch("{id}")]
        public async Task<ActionResult> AddProductTypeTranslationAsync([FromRoute] int id, [FromBody] ProductTypePost productType)
        {
            try
            {
                var service = new ProductService();
                await service.AddOrUpdateProductTypeTranslationAsync(id, productType.Translations, productType.UpdatedBy);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


#if DEBUG
        [HttpPatch("/populate")]
        public async Task AddDummyData()
        {
            await new PopulateData().Populate();
        }
#endif
    }
}
