using System;

namespace AssetServices.ServiceModel
{
    public class MakeAssetAvailableDTO
    {
        public Guid AssetLifeCycleId { get; init; }
        public EmailPersonAttributeDTO? PreviousUser { get; init; }
        public Guid CallerId { get; set; }
    }
}
