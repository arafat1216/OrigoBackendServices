using System;

namespace AssetServices.ServiceModel
{
    public class ReturnLocationDTO
    {
        public Guid ExternalId { get; init; }
        public string Name { get; set; } = string.Empty;
        public string ReturnDescription { get; set; } = string.Empty ;
        public Guid LocationId { get; set; }
    }
}
