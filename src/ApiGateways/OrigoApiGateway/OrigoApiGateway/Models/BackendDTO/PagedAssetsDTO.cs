using System.Collections.Generic;

namespace OrigoApiGateway.Models.BackendDTO
{
    public record PagedAssetsDTO
    {
        public IList<object> Assets { get; init; }
        public int CurrentPage { get; init; }
        public int TotalItems { get; init; }
        public int TotalPages { get; init; }
    }
}