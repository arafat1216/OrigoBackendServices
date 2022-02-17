using Common.Seedwork;
using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices.Types;

namespace SubscriptionManagementServices.Models
{
    public class CustomerReferenceField : Entity
    {
        public CustomerReferenceField()
        {
        }

        public CustomerReferenceField(string name, CustomerReferenceTypes referenceType, Guid callerId)
        {
            Name = name;
            ReferenceType = referenceType;
            CreatedBy = callerId;
        }

        public string Name { get; protected set; }
        public CustomerReferenceTypes ReferenceType { get; protected set; }
    }
}