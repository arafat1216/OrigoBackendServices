namespace Common.Interfaces
{
    /// <summary>
    ///     A paginated list.
    /// </summary>
    /// <typeparam name="TModel"> The datatype we are storing in the retrieved <see cref="Items"/> list. </typeparam>
    public class PagedModel<TModel>
    {
        /// <summary> The highest number of items that can be added in a single page. </summary>
        const int MaxPageSize = 1000;

        /// <summary> Backing-field for <see cref="PageSize"/>. </summary>
        private int _pageSize;

        /// <summary> The number of items to retrieve for each page. </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        /// <summary> The current page number. </summary>
        public int CurrentPage { get; set; }

        /// <summary> The total number of items. </summary>
        public int TotalItems { get; set; }

        /// <summary> The total number of pages. </summary>
        public int TotalPages { get; set; }

        /// <summary> A list that contains the items contained in the current page. </summary>
        public IList<TModel> Items { get; set; }

        /// <summary> Initializes a new instance of the <see cref="PagedModel{TModel}"/> class. </summary>
        public PagedModel()
        {
            Items = new List<TModel>();
        }
    }
}
