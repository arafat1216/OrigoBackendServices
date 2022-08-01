using System;
using System.Collections.Generic;

namespace AssetServices.ServiceModel
{
    public class DisposeSettingDTO
    {
        public Guid ExternalId { get; init; }
        public IList<ReturnLocationDTO> ReturnLocations { get; init; }
        public DateTime CreatedDate { get; set; }
    }
}
