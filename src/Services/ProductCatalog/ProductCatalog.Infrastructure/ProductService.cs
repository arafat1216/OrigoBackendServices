using Microsoft.AspNetCore.Http;
using ProductCatalog.Domain.Exceptions;
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
        public async Task<IEnumerable<ProductGet>> GetAllAsync(Guid? partnerId)
        {
            IEnumerable<Product> results;

            if (partnerId is null)
                results = await _unitOfWork.Products.GetAsync(asNoTracking: true);
            else
                results = await _unitOfWork.Products.GetAsync(filter: e => e.PartnerId == partnerId, asNoTracking: true);

            return new EntityAdapter().ToDTO(results);
        }


        public async Task<ProductGet?> GetByIdAsync(int id)
        {
            var result = await _unitOfWork.Products.GetByIdAsync(id);

            if (result is null)
                return null;
            else
                return new EntityAdapter().ToDTO(result);
        }


        // TODO: This loop nightmare to validate/resolve the requirements may end up a bottleneck if we end up with larger datasets.
        // If so, this will likely need another revision to figure out a more optimal way of solving the checks!
        /// <summary>
        ///     Checks a list of productIds, and tries to determine if all products is available for the partner, 
        ///     and if they are available, whether or not all product-requirements has been fulfilled.
        /// </summary>
        /// <param name="newProductIds"> The list of product IDs that is checked against each other. </param>
        /// <param name="partnerId"> The partner we are validating the requirements for. </param>
        /// <returns> Returns <see langword="true"/> if the configuration is valid, and <see langword="false"/> 
        ///     if any conflicting requirements was detected. </returns>
        /// <exception cref="EntityNotFoundException"> Thrown of one or more of the provided product IDs does not exist. 
        ///     This may be because one or more products is not available for the given partner. </exception>
        public async Task<bool> ValidateProductListRequirements(IEnumerable<int> newProductIds, Guid partnerId)
        {
            var newProductIdsAsHash = newProductIds.ToHashSet();
            var newProducts = await _unitOfWork.Products.GetAsync(filter: e => newProductIdsAsHash.Contains(e.Id)
                                                                               && e.PartnerId == partnerId);

            // Make sure all products actually exist by comparing the list counts.
            if (newProductIds.Count() != newProducts.Count())
                throw new EntityNotFoundException("One or more of the IDs was not found!");

            var excludesAll = new HashSet<int>();
            var requiresAll = new HashSet<int>();

            try
            {
                foreach (var product in newProducts)
                {
                    // If needed, compare the requirement list, and make sure that at least one of the values are overlapping between the lists.
                    if (product.RequiresOne.Any())
                    {
                        var hashSet = (from item in product.RequiresOne
                                       select item.RequiresProductId
                                      ).ToHashSet();

                        if (!hashSet.Overlaps(newProductIdsAsHash))
                            throw new TaskCanceledException(); // Conflict: A 'RequiresOne' requirement went unfulfilled for the current productId.
                    }

                    // If needed, extract the values to prevent nested looping.
                    if (product.Excludes.Any())
                    {
                        excludesAll.UnionWith(product.ExcludesAsIds);
                    }

                    // If needed, extract the values to prevent nested looping.
                    if (product.RequiresAll.Any())
                    {
                        requiresAll.UnionWith(product.RequiresAllAsIds);
                    }
                }

                // Check the extracted values. If there are any overlaps between the sets, then a excluded product have been added, and the configuration is invalid.
                if (excludesAll.Overlaps(newProductIdsAsHash))
                    throw new TaskCanceledException(); // Conflict: One or more 'Excluded' requirements went unfulfilled. Either remove the excluded product(s), or the product(s) with the exclusion requirement

                // Check the extracted values, and make sure all required ids also exist in the new product list.
                // If it's not, we are missing a requirement, and the configuration is invalid.
                if (!requiresAll.IsSubsetOf(newProductIdsAsHash))
                    throw new TaskCanceledException(); // Conflict: One or more 'RequiresAll' requirements went unfulfilled. Please add the missing products.
            }
            catch (TaskCanceledException)
            {
                return false;
            }

            return true;
        }


        #endregion


        #region Product Types

        public async Task<ProductTypeGet?> GetProductTypeAsync(int id, IEnumerable<string>? languages = null)
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
