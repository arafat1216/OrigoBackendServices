using CustomerServices.Models;
using Common.Logging;

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
