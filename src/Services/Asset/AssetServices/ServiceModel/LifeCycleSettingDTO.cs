using System;
using System.Collections.Generic;

namespace AssetServices.ServiceModel
{
    public record LifeCycleSettingDTO
    {
        public Guid ExternalId { get; init; }
        public Guid CustomerId { get; init; }
        public bool BuyoutAllowed { get; set; }
        public IReadOnlyCollection<CategoryLifeCycleSettingDTO> CategoryLifeCycleSettings { get; init; } = null!;
        public DateTime CreatedDate { get; set; }
    }
}
