using CustomerServices.Models;
using Common.Logging;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace CustomerServices.DomainEvents
{
    public class CustomerCreatedDomainEvent : BaseEvent
    {

        public CustomerCreatedDomainEvent(Customer newCustomer):base(newCustomer.CustomerId)
        {
            NewCustomer = newCustomer;
        }

        public Customer NewCustomer { get; protected set; }
    }
}
