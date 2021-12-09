using ProductCatalog.Common.Generic;
using ProductCatalog.Common.Interfaces;

namespace ProductCatalog.Common
{
    internal class TypeConverter
    {
        public Translation ToTranslation(ITranslationResult translation)
        {
            return new Translation(translation.Language, translation.Name, translation.Description);
        }


        public IEnumerable<Translation> ToTranslation(IEnumerable<ITranslationResult> translations)
        {
            var results = new List<Translation>();

            foreach (var translation in translations)
            {
                results.Add(ToTranslation(translation));
            }

            return results;
        }

    }
}
