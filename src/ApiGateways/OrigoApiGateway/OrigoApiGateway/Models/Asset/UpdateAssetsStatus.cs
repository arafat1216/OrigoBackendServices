using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models.Asset
{
    /// <summary>
    /// Request object.
    /// A list of Assets ID (Guid), and an integer representing the status these assets will be set to.
    /// </summary>
    public class UpdateAssetsStatus
    {
        [Required]
        public IList<Guid> AssetGuidList { get; set; }

        [Required]
        public int AssetStatus { get; set; }
    }
}
