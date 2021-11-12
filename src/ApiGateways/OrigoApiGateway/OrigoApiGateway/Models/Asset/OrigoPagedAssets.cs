using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    public record OrigoPagedAssets
    {
        public OrigoPagedAssets()
        {
            Assets = new List<OrigoAsset>();
        }

        public IList<OrigoAsset> Assets { get; set; }
        public int CurrentPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
