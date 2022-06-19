using HardwareServiceOrderServices.Models;
using System.ComponentModel.DataAnnotations;

namespace HardwareServiceOrder.API.ViewModels
{
    public class DeliveryAddress
    {
        /// <inheritdoc cref="DeliveryAddressDTO.RecipientType"/>
        public RecipientTypeEnum RecipientType { get; set; }
       
        public string Recipient { get; set; }
      
        public string Address1 { get; set; }
      
        public string? Address2 { get; set; }
        
        [Required]
        [StringLength(maximumLength: 12)]
        public string PostalCode { get; set; }
      
        [StringLength(maximumLength: 85)]
        public string City { get; set; }
      
        [Required]
        [RegularExpression("^[A-Z]{2}$")]
        [StringLength(maximumLength: 2, MinimumLength = 2)]
        public string Country { get; set; }
    }
}
