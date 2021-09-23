using CustomerServices.Models;
using Common.Logging;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace CustomerServices.DomainEvents
{
    public class CustomerCreatedDomainEvent : BaseEvent
    {

        public CustomerCreatedDomainEvent(Organization newCustomer):base(newCustomer.OrganizationId)
        {
            NewCustomer = newCustomer;
        }

        public Organization NewCustomer { get; protected set; }
    }
}
