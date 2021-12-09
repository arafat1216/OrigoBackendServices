using ProductCatalog.Common.Generic;
using ProductCatalog.Common.Interfaces;
using ProductCatalog.Common.Orders;
using ProductCatalog.Common.Products;
using ProductCatalog.Common.ProductTypes;
using ProductCatalog.Infrastructure.Models.Database;

namespace ProductCatalog.Infrastructure
{
    /// <summary>
    ///     Performs class-conversion between various entity-models, such as converting back and forth between DTO and EF entities.
    /// </summary>
    internal class EntityAdapter
    {

        /// <summary>
        ///     Converts between different classes that implements the <see cref="ITranslationResult"/> interface. Only the fields implemented 
        ///     by the interface will be converted.
        /// </summary>
        /// <typeparam name="TOut"> The target class that will be returned after the conversion. </typeparam>
        /// <param name="input"> The object that will be converted. </param>
        /// <returns> The converted object. </returns>
        public TOut Convert<TOut>(ITranslationResult input) where TOut : class, ITranslationResult, new()
        {
            return new TOut
            {
                Description = input.Description,
                Language = input.Language,
                Name = input.Name
            };
        }

        /// <summary>
        ///     Converts the elements contained in a enumerator. The elements can be converted between different classes as long as they all
        ///     implement the <see cref="ITranslationResult"/> interface. The converted result will only contain values for the actual fields
        ///     implemented by the interface.
        /// </summary>
        /// <typeparam name="TFrom"> The class or interface that is converted from. </typeparam>
        /// <param name="input"> The list containing the objects to be converted. </param>
        /// <returns> The enumerator containing all converted objects. </returns>
        public IEnumerable<TOut> Convert<TOut>(IEnumerable<ITranslationResult> input) where TOut : class, ITranslationResult, new()
        {
            var results = new List<TOut>();

            foreach (var item in input)
            {
                results.Add(Convert<TOut>(item));
            }

            return results;
        }


        public async IAsyncEnumerable<TOut> ConvertAsync<TOut>(IEnumerable<ITranslationResult> input) where TOut : class, ITranslationResult, new()
        {
            foreach (var item in input)
            {
                yield return await Task.FromResult(Convert<TOut>(item));
            }
        }


        public Translation ToDTO(ITranslationResult input)
        {
            return new Translation(input.Language, input.Name, input.Description);
        }


        public IEnumerable<Translation> ToDTO(IEnumerable<ITranslationResult> input)
        {
            var results = new List<Translation>();

            foreach (var item in input)
            {
                results.Add(ToDTO(item));
            }

            return results;
        }

        public IEnumerable<ProductGet> ToDTO(IEnumerable<Product> input)
        {
            var results = new List<ProductGet>();

            foreach (var item in input)
            {
                results.Add(ToDTO(item));
            }

            return results;
        }


        public IEnumerable<OrderGet> ToDTO(IEnumerable<Order> input)
        {
            var results = new List<OrderGet>();

            foreach (var item in input)
            {
                results.Add(ToDTO(item));
            }

            return results;
        }


        public ProductGet ToDTO(Product input)
        {
            var requirement = new Requirement(input.ExcludesAsIds, input.RequiresAllAsIds, input.RequiresOneAsIds);
            return new ProductGet(input.Id, input.PartnerId, input.ProductTypeId, null, ToDTO(input.Translations), requirement);
        }


        public OrderGet ToDTO(Order input)
        {
            return new OrderGet(input.ExternalId, input.ProductId, input.OrganizationId);
        }


        public ProductTypeGet ToDTO(ProductType input)
        {
            return new ProductTypeGet(input.Id, input.Translations);
        }


        public async IAsyncEnumerable<ProductGet> ToDTOAsync(IEnumerable<Product> input)
        {
            foreach (var item in input)
            {
                yield return await Task.FromResult(ToDTO(item));
            }
        }

        public async IAsyncEnumerable<Translation> ToDTOAsync(IEnumerable<ITranslationResult> input)
        {
            foreach (var item in input)
            {
                yield return await Task.FromResult(ToDTO(item));
            }
        }

    }
}
