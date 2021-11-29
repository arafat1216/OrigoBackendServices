using ProductCatalog.Domain.Generic;
using ProductCatalog.Domain.Products;
using ProductCatalog.Service.Infrastructure;
using ProductCatalog.Service.Infrastructure.Context;
using ProductCatalog.Service.Models.Database;
using System.Linq.Expressions;

namespace ProductCatalog.Service
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

        /*
        public async Task<IEnumerable<ProductGet>> GetAllPartnerProducts(Guid partnerId)
        {
            // TODO: Fix mapping and extra data (type and requirements)
            var results = await _unitOfWork.Products.FindAsync(e => e.PartnerId == partnerId, true);

            return results;
        }
        */

        #endregion


        #region Product Types

        public async Task<ProductTypeGet> AddProductTypeAsync(ProductTypePost productType)
        {
            if (!productType.Translations.Any(e => e.Language == "en"))
            {
                throw new ArgumentException("English translations is required, but not currently added");
            }

            var dbEntity = new ProductType(productType.Translations);

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
        public async Task<IEnumerable<ProductTypeGet>> GetProductTypesAsync(IEnumerable<string>? languages = null)
        {
            IEnumerable<ProductType>? entities;
            var results = new List<ProductTypeGet>();
            var missingTranslations = new List<ProductType>();

            // No filters -> Get all
            if (languages == null || !languages.Any())
                entities = await _unitOfWork.ProductTypes.GetAllAsync(true);
            // Single language -> Get it explicitly for better performance
            else if (languages.Count() == 1)
                entities = await _unitOfWork.ProductTypes.GetAllAsync(languages.First(), true);
            // Several languages -> Run a more expensive 'where in' condition
            else
                entities = await _unitOfWork.ProductTypes.GetAllAsync(languages, true);

            foreach (var entity in entities)
            {
                // Make sure we have a language result. If not, we need to fetch it in English.
                if (entity.Translations.Any())
                    results.Add(new ProductTypeGet(entity.Id, entity.Translations));
                else
                    missingTranslations.Add(entity);
            }

            var test = _unitOfWork.ProductTypes.FindAsync("en", 
                                                          e => missingTranslations.,
                                                          true);

            return results;
        }


        // TODO: Remove / rework
        public async Task AddProductTypeTranslationAsync(ProductTypeUpdate productType)
        {
            var dbEntity = await _unitOfWork.ProductTypes.GetByIdAsync(productType.Id);

            if (dbEntity is null)
                throw new ArgumentException("Item not found");

            var adapter = new EntityAdapter();
            _unitOfWork.ProductTypes.Update(dbEntity);

            // Convert and add
            foreach (var translation in productType.Translations)
            {
                var adapted = adapter.Convert<ProductTypeTranslation>(translation);
                dbEntity.Translations.Add(adapted);
            }
        }


        public async Task AddProductTypeTranslationAsync(int id, IEnumerable<Translation> translations)
        {
            var productType = await _unitOfWork.ProductTypes.GetByIdAsync(id);

            if (productType is null)
                throw new ArgumentException("Invalid ID");

            var adapter = new EntityAdapter();
            var adaptedResults = adapter.Convert<ProductTypeTranslation>(translations);

            foreach (var item in adaptedResults)
            {
                item.ProductTypeId = id;
                productType.Translations.Add(item);
            }

            //await _unitOfWork.ProductTypes.AddTranslationRangeAsync(adaptedResults);
            //productType.Translations.Add(adaptedResults);

            await _unitOfWork.SaveAsync();
        }


        #endregion
    }
}
