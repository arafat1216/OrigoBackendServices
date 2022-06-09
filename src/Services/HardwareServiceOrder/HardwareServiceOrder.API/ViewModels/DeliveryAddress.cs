using HardwareServiceOrderServices.Models;

namespace HardwareServiceOrder.API.ViewModels
{
    public class DeliveryAddress
    {
        public RecipientTypeEnum RecipientType { get; set; }
        public string Recipient { get; set; }
        public string Address1 { get; set; }
        public string? Address2 { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
