using SubscriptionManagementServices.Types;

namespace SubscriptionManagementServices.ServiceModels
{
    public class CustomerReferenceFieldDTO
    {
        public string Name { get; set; }
        public CustomerReferenceTypes ReferenceType { get; set; }
        public int Id { get; set; }
    }
}
