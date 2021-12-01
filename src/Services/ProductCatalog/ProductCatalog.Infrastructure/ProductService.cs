using Microsoft.AspNetCore.Http;
using ProductCatalog.Domain.Generic;
using ProductCatalog.Domain.Products;
using ProductCatalog.Domain.ProductTypes;
using ProductCatalog.Infrastructure.Infrastructure;
using ProductCatalog.Infrastructure.Infrastructure.Context;
using ProductCatalog.Infrastructure.Models.Database;
using System.Linq.Expressions;

namespace ProductCatalog.Infrastructure
{
    public class ProductService
    {
        private readonly UnitOfWork _unitOfWork;


        public ProductService()
        {
            string[] args = Array.Empty<string>();
            var context = new ProductCatalogContextFactory().CreateDbContext(args);

            _unitOfWork = new UnitOfWork(context);
        }

        // TODO: Take a look at this. We may need it for unit-testing and service runtime injection registration.
        internal ProductService(ProductCatalogContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }

        #region Products

        /// <summary>
        ///     Returns all products. An optional partnerId can be provided to filter the results to a specific partner.
        /// </summary>
        /// <param name="partnerId"> The partner ID that should be filtered on, or <see langword="null"/> if no filters should be added. </param>
        /// <returns> A list of all matching products. </returns>
        public async Task<IEnumerable<ProductGet>> GetAllProductsAsync(Guid? partnerId)
        {
            IEnumerable<Product> results;

            if (partnerId is null)
                results = await _unitOfWork.Products.GetAsync(asNoTracking: true);
            else
                results = await _unitOfWork.Products.GetAsync(filter: e => e.PartnerId == partnerId, asNoTracking: true);

            return new EntityAdapter().ToDTO(results);
        }


        public async Task<ProductGet?> GetProductAsync(int id)
        {
            var result = await _unitOfWork.Products.GetByIdAsync(id);

            if (result is null)
                return null;
            else
                return new EntityAdapter().ToDTO(result);
        }


        #endregion


        #region Product Types

        public async Task<ProductTypeGet?> GetProductTypeAsync(int id)
        {
            var result = await _unitOfWork.ProductTypes.GetByIdAsync(id);

            if (result is null)
                return null;
            else
                return new EntityAdapter().ToDTO(result);
        }


        public async Task<ProductTypeGet> AddProductTypeAsync(ProductTypePost productType)
        {
            if (!productType.Translations.Any(e => e.Language == "en"))
            {
                throw new ArgumentException("English translations is required, but not currently added");
            }

            var dbEntity = new ProductType(productType.Translations, productType.UpdatedBy);

            await _unitOfWork.ProductTypes.AddAsync(dbEntity);
            await _unitOfWork.SaveAsync();

            var result = new ProductTypeGet(dbEntity.Id, dbEntity.Translations);
            return result;
        }


        /// <summary>
        ///     Retrieve a list containing every <see cref="ProductType"/>.
        /// </summary>
        /// <param name="languages"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ProductTypeGet>> GetAllProductTypesAsync(IEnumerable<string>? languages = null)
        {
            IEnumerable<ProductType>? entities;
            var results = new List<ProductTypeGet>();

            // No filters -> Get all
            if (languages == null || !languages.Any())
                entities = await _unitOfWork.ProductTypes.GetAsync(asNoTracking: true);
            // Single language -> Get it explicitly for better performance
            else if (languages.Count() == 1)
                entities = await _unitOfWork.ProductTypes.GetAsync(language: languages.First(), asNoTracking: true);
            // Several languages -> Run a more expensive 'where in' condition
            else
                entities = await _unitOfWork.ProductTypes.GetAsync(languages: languages, asNoTracking: true);

            foreach (var entity in entities)
            {
                results.Add(new ProductTypeGet(entity.Id, entity.Translations));
            }

            return results;
        }


        /// <summary>
        ///     Add new translations to a product type. If the translation already exist, it is updated.
        /// </summary>
        /// <param name="id"> The product-type ID the translations is added to. </param>
        /// <param name="translations"> A list of all new or updated translations. </param>
        /// <returns> The task representing the async operation. </returns>
        /// <exception cref="ArgumentException"> Thrown if no entities with the provided <paramref name="id"/> was found. </exception>
        public async Task AddOrUpdateProductTypeTranslationAsync(int id, IEnumerable<Translation> translations, Guid updatedBy)
        {
            var productType = await _unitOfWork.ProductTypes.GetByIdAsync(id);

            if (productType is null)
                throw new ArgumentException("ID not found");

            foreach (var translation in translations)
            {
                ProductTypeTranslation? existingTranslation = productType.Translations.FirstOrDefault(e => e.Language == translation.Language);

                // It don't exist -> Add it
                if (existingTranslation is null)
                {
                    var entity = new ProductTypeTranslation(id, translation.Language, translation.Name, translation.Description, updatedBy);
                    productType.Translations.Add(entity);
                }
                // It exists -> Update it
                else
                {
                    existingTranslation.Name = translation.Name;
                    existingTranslation.Description = translation.Description;
                }
            }

            _unitOfWork.ProductTypes.Update(productType);
            await _unitOfWork.SaveAsync();
        }


        #endregion
    }
}
