using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.BackendDTO
{
    public record AssetLabelsDTO
    {
      
        public IList<Guid> AssetGuids { get; set; }
     
        public IList<Guid> LabelGuids { get; set; }

        public Guid CallerId { get; set; }
    }
}
