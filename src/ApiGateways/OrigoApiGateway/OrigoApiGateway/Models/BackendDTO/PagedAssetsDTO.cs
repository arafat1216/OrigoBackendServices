using System.Collections.Generic;

namespace OrigoApiGateway.Models.BackendDTO
{
    public record PagedAssetsDTO
    {
        public IList<object> Assets { get; set; }
        public int CurrentPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}