using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.BackendDTO
{
    public record UpdateAssetsStatusDTO
    {
        public IList<Guid> AssetGuidList { get; set; }

        public int AssetStatus { get; set; }
        /// <summary>
        /// id of user making the endpoint call. Can be ignored by frontend.
        /// </summary>
        public Guid CallerId { get; set; }
    }
}
