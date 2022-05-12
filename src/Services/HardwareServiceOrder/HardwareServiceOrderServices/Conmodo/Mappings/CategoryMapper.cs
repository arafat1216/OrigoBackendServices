namespace HardwareServiceOrderServices.Conmodo.Mappings
{
    internal class CategoryMapper
    {
        /// <summary>
        ///     Converts our internal asset category ID to an equivalent value used by Conmodo.
        /// </summary>
        /// <remarks>
        ///     The remapping will only work where our own asset category-IDs is added by the seed-data, and thus will always be fixed!
        /// </remarks>
        /// <param name="assetCategoryId"> Our internal category ID. </param>
        /// <returns> If available, the corresponding category name used by Conmodo. Otherwise it returns <see langword="null"/>. </returns>
        public string? ToConmodo(int? assetCategoryId)
        {
            if (assetCategoryId is null)
                return null;

            int categoryId = assetCategoryId.Value;

            // TODO: For now we don't need the mapping, but this must be recorded later.
            // TODO: This also requires that we have fixed category IDs (e.g. int values added in data-seeds) that we can use when converting.
            switch (categoryId)
            {
                default:
                    return null;
            }
        }

        /// <summary>
        ///     Converts from Conmodo's asset category names to an equivalent value used by our solution.
        /// </summary>
        /// <remarks>
        ///     The remapping will only work where our own asset category-IDs is added by the seed-data, and thus will always be fixed!
        /// </remarks>
        /// <param name="categoryName"> Conmodo's category name. </param>
        /// <returns> If available, the corresponding category name used by our solution. Otherwise it returns <see langword="null"/>. </returns>
        public int? FromConmodo(string? categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
                return null;

            string normalizedCategoryName = categoryName.ToLower();

            // TODO: For now we don't need the mapping, but this must be recorded later.
            // TODO: This also requires that we have fixed category IDs (e.g. int values added in data-seeds) that we can use when converting.
            switch (normalizedCategoryName)
            {
                default:
                    return null;
            }
        }
    }
}
