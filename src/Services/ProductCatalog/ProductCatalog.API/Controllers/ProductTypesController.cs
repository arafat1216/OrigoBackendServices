﻿using Microsoft.AspNetCore.Mvc;
using ProductCatalog.API.Filters;
using ProductCatalog.Common.ProductTypes;
using ProductCatalog.Infrastructure;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;

namespace ProductCatalog.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [SwaggerTag("Actions for handling product-types and their translations.")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    [ServiceFilter(typeof(ErrorExceptionFilter))]
    public class ProductTypesController : ControllerBase
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
        ///     Retrieves all product-types.
        /// </summary>
        /// <remarks>
        ///     Retrieves a list of all product-types. 
        ///     
        /// 
        ///     If any language-codes has been provided in <code><paramref name="languages"/></code>, then only translations matching
        ///     the provided values will be returned. If no values is provided, then all languages will be included.
        /// </remarks>
        /// <param name="languages" example="[en,nb]"> If provided, only translations for the provided languages is included in the response.
        ///     Otherwise all languages is included. </param>
        /// <returns> A collection of all product-types. </returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductTypeGet>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductTypeGet>>> GetAllAsync([FromQuery] IEnumerable<string>? languages)
        {
            var result = await new ProductService().GetAllProductTypesAsync(languages);
            return Ok(result);
        }


        /// <summary>
        ///     Creates a new product-type.
        /// </summary>
        /// <remarks>
        ///     Creates a new product-type along with corresponding translations.
        ///     
        ///     
        ///     The English ("<code>en</code>) translation must always be added.
        /// </remarks>
        /// <param name="productType"> The new product-type that should be added. </param>
        /// <returns> The newly created object. </returns>
        [HttpPost]
        [ProducesResponseType(typeof(ProductTypeGet), StatusCodes.Status201Created)]
        public async Task<ActionResult<ProductTypeGet>> AddAsync(ProductTypePost productType)
        {
            var result = await new ProductService().AddProductTypeAsync(productType);
            // TODO: Fix a proper Created() response that includes the 'Location' header.
            return StatusCode(StatusCodes.Status201Created, result);
        }


        /// <summary>
        ///     Retrieves a product-type by it's ID.
        /// </summary>
        /// <remarks>
        ///     Retrieve a single product-type based on it's ID.
        /// </remarks>
        /// <param name="id"> The ID of the product that should be retrieved. </param>
        /// <returns> If found, the corresponding product-type. </returns>
        /// <response code="404"> The product ID was not found. </response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductTypeGet), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductTypeGet>> GetByIdTypeAsync([FromRoute] int id)
        {

            var result = await new ProductService().GetProductTypeAsync(id);

            if (result is null)
                return NotFound();
            else
                return Ok(result);

        }


        /// <summary>
        ///     Adds or updates a product-type translation.
        /// </summary>
        /// <remarks>
        ///     Used to create new translations. If the language already exist, it will update the existing entry with the new details.
        /// </remarks>
        /// <param name="id"> The ID for the product-type the translations belongs to. </param>
        /// <param name="productType"> The object that contains the new translations. </param>
        [HttpPatch("{id}")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> AddTranslationAsync([FromRoute] int id, [FromBody] ProductTypePost productType)
        {
            await new ProductService().AddOrUpdateProductTypeTranslationAsync(id, productType.Translations, productType.UpdatedBy);
            return NoContent();
        }


#if DEBUG
        /// <summary>
        ///     Insert test-data to the database.
        /// </summary>
        /// <remarks>
        ///     THIS IS A POTENTIALLY DANGEROUS OPERATION ONLY INTENDED FOR USE IN DEVELOPMENT ENVIRONMENTS!
        ///     
        /// 
        ///     Populates the database with various dummy data.
        /// </remarks>
        /// <returns></returns>
        [HttpPatch("/populate/testdata")]
        [SwaggerOperation(
            Tags = new[] { "Data Population" }
        )]
        public async Task AddDummyDataAsync()
        {
            await new PopulateData().PopulateDummyData();
        }
#endif
    }
}
