using ProductCatalog.Domain.Generic;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Service
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
                yield return Convert<TOut>(item);
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


        public async IAsyncEnumerable<Translation> ToDTOAsync(IEnumerable<ITranslationResult> input)
        {
            foreach (var item in input)
            {
                yield return ToDTO(item);
            }
        }


    }
}
