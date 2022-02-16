using Common.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionManagementServices.Models
{
    public class CustomerSettings : Entity, IAggregateRoot
    {

        public CustomerSettings(Guid customerId, ICollection<CustomerOperatorSettings>? customerOperatorSettings, IReadOnlyCollection<CustomerReferenceField>? customerReferenceFields)
        {
            CustomerId = customerId;
            CustomerOperatorSettings = customerOperatorSettings;
            CustomerReferenceFields = customerReferenceFields;
        }

        public CustomerSettings()
        {
            //CustomerId = customerId;
        }
        
        public Guid CustomerId { get; protected set; }
        public ICollection<CustomerOperatorSettings>? CustomerOperatorSettings { get; protected set; } = new HashSet<CustomerOperatorSettings>();
        public IReadOnlyCollection<CustomerReferenceField>? CustomerReferenceFields { get; protected set; }
    }
}
