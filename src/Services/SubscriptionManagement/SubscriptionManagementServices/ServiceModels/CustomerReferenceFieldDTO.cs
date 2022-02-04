using SubscriptionManagementServices.Types;

namespace SubscriptionManagementServices.ServiceModels
{
    public class CustomerReferenceFieldDTO
    {
        public CustomerReferenceFieldDTO(string name, CustomerReferenceTypes referenceType)
        {
            Name = name;
            ReferenceType = referenceType;
        }

        public string Name { get; protected set; }
        public CustomerReferenceTypes ReferenceType { get; protected set; }
    }
}
