using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class AssetCategoryDTO
    {
        /// <summary>
        /// External id of the AssetCategory
        /// </summary>
        public Guid AssetCategoryId { get; set; }

        public string Name { get; set; }

        public bool UsesImei { get; set; }
    }
}
