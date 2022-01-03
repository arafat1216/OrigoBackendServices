using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Response object.
    /// </summary>
    public class AssetCategoryAttribute
    {
        public string Name { get; init; }
        public bool Required { get; init; }
    }
}
