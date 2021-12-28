using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Response object
    /// </summary>
    public record OrigoPagedAssets
    {
        public OrigoPagedAssets()
        {
            Assets = new List<object>();
        }

        public IList<object> Assets { get; set; }
        public int CurrentPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
