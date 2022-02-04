using Common.Seedwork;
using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices.Types;

namespace SubscriptionManagementServices.Models
{
    [Owned]
    public class CustomerReferenceField : ValueObject
    {
        public CustomerReferenceField(string name, CustomerReferenceTypes referenceType)
        {
            Name = name;
            ReferenceType = referenceType;
        }

        public string Name { get; protected set; }
        public CustomerReferenceTypes ReferenceType { get; protected set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Name;
            yield return ReferenceType;

        }
    }
}