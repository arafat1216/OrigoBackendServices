using System;
using System.ComponentModel.DataAnnotations;

namespace AssetServices.ServiceModel
{
    public class BuyoutDeviceDTO
    {
        [Required]
        public Guid AssetLifeCycleId { get; set; }
        public Guid CallerId { get; set; }
    }
}
