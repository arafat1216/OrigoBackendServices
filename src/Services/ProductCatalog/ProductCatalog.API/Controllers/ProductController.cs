using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Domain.Generic;
using ProductCatalog.Domain.Products;
using ProductCatalog.Service;
using System.Net;

namespace ProductCatalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        [HttpGet("product-type")]
        public async Task<IEnumerable<ProductTypeGet>> GetAllProductTypes([FromQuery] IEnumerable<string>? languages)
        {
            var service = new ProductService();
            return await service.GetProductTypesAsync(languages);
        }


        [HttpPost("product-type")]
        public async Task<ProductTypeGet> AddProductType(ProductTypePost productType)
        {
            return await new ProductService().AddProductTypeAsync(productType);
        }

        [HttpPost("product-type/{id}/translation")]
        public async Task AddProductTypeTranslation([FromRoute] int id, List<Translation> translations)
        {
            await new ProductService().AddProductTypeTranslationAsync(id, translations);
        }
    }
}
