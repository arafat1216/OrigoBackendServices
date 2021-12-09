using Microsoft.EntityFrameworkCore;
using ProductCatalog.Common.Exceptions;
using ProductCatalog.Infrastructure.Infrastructure;
using ProductCatalog.Infrastructure.Infrastructure.Context;
using ProductCatalog.Infrastructure.Models.Database;

namespace ProductCatalog.Infrastructure
{
    public class FeatureService
    {
        private readonly UnitOfWork _unitOfWork;

        public FeatureService()
        {
            string[] args = Array.Empty<string>();
            var context = new ProductCatalogContextFactory().CreateDbContext(args);

            _unitOfWork = new UnitOfWork(context);
        }

        // TODO: Take a look at this. We may need it for unit-testing and service runtime injection registration.
        internal FeatureService(ProductCatalogContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }


        public async Task<IEnumerable<string>> GetPermissionNodesAsync(Guid organizationId)
        {
            return await _unitOfWork.Features.GetPermissionNodesByOrganizationAsync(organizationId);
        }


        // TODO: This loop nightmare to validate/resolve the requirements may end up a bottleneck if we end up with larger datasets.
        // If so, this will likely need another revision to figure out a more optimal way of solving the checks!
        /// <summary>
        ///     Checks a list of featureIds, and tries to determine whether or not all requirements for the features has been fulfilled.
        /// </summary>
        /// <param name="newFeatureIds"> The list of feature IDs that is checked against each other. </param>
        /// <returns> Returns <see langword="true"/> if the configuration is valid, and <see langword="false"/> 
        ///     if any conflicting requirements was detected. </returns>
        /// <exception cref="EntityNotFoundException"> Thrown of one or more of the provided product IDs does not exist. </exception>
        public async Task<bool> ValidateFeatureListRequirements(IEnumerable<int> newFeatureIds)
        {
            var newFeatureIdsAsHash = newFeatureIds.ToHashSet();
            var newFeatures = await _unitOfWork.Features.GetAsync(filter: e => newFeatureIdsAsHash.Contains(e.Id));

            // Make sure all products actually exist by comparing the list counts.
            if (newFeatureIds.Count() != newFeatures.Count())
                throw new EntityNotFoundException("One or more of the IDs was not found!");

            var excludesAll = new HashSet<int>();
            var requiresAll = new HashSet<int>();

            try
            {
                foreach (var feature in newFeatures)
                {
                    // If needed, compare the requirement list, and make sure that at least one of the values are overlapping between the lists.
                    if (feature.RequiresOne.Any())
                    {
                        var hashSet = (from item in feature.RequiresOne
                                       select item.RequiresFeatureId
                                      ).ToHashSet();

                        if (!hashSet.Overlaps(newFeatureIdsAsHash))
                            throw new TaskCanceledException(); // Conflict: A 'RequiresOne' requirement went unfulfilled for the current productId.
                    }

                    // If needed, extract the values to prevent nested looping.
                    if (feature.Excludes.Any())
                    {
                        excludesAll.UnionWith(feature.ExcludesAsIds);
                    }

                    // If needed, extract the values to prevent nested looping.
                    if (feature.RequiresAll.Any())
                    {
                        requiresAll.UnionWith(feature.RequiresAllAsIds);
                    }
                }

                // Check the extracted values. If there are any overlaps between the sets, then a excluded product have been added, and the configuration is invalid.
                if (excludesAll.Overlaps(newFeatureIdsAsHash))
                    throw new TaskCanceledException(); // Conflict: One or more 'Excluded' requirements went unfulfilled. Either remove the excluded product(s), or the product(s) with the exclusion requirement

                // Check the extracted values, and make sure all required ids also exist in the new product list.
                // If it's not, we are missing a requirement, and the configuration is invalid.
                if (!requiresAll.IsSubsetOf(newFeatureIdsAsHash))
                    throw new TaskCanceledException(); // Conflict: One or more 'RequiresAll' requirements went unfulfilled. Please add the missing products.
            }
            catch (TaskCanceledException)
            {
                return false;
            }

            return true;
        }


    }
}
