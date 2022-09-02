using Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace AssetServices.ServiceModel
{
    public class PendingBuyoutDeviceDTO
    {
        [Required]
        public Guid AssetLifeCycleId { get; set; }
        public DateTime LasWorkingDay { get; set; }
        public EmailPersonAttributeDTO? User { get; set; }
        public Guid CallerId { get; set; }
        public string Role { get; set; } = PredefinedRole.EndUser.ToString();
        public string Currency { get; set; } = CurrencyCode.NOK.ToString();
        public string ManagerName { get; set; } = string.Empty;
    }
}
