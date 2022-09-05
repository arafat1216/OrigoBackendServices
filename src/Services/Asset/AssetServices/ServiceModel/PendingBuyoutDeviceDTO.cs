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
        /// <summary>
        /// Role of the user making request
        /// </summary>
        public string Role { get; set; } = PredefinedRole.EndUser.ToString();
        /// <summary>
        /// Currency of the Customer
        /// </summary>
        public string Currency { get; set; } = CurrencyCode.NOK.ToString();
        /// <summary>
        /// Name of the user making request if its not an end user.
        /// </summary>
        public string ManagerName { get; set; } = string.Empty;
    }
}
