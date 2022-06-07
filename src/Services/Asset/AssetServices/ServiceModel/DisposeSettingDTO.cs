using System;
using System.ComponentModel.DataAnnotations;

namespace AssetServices.ServiceModel
{
    public class DisposeSettingDTO
    {
        public Guid ExternalId { get; init; }
        public Guid CustomerId { get; init; }
        public string PayrollContactEmail { get; init; }
        public DateTime CreatedDate { get; set; }
    }
}
