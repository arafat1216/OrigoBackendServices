using System;

namespace Asset.API.Events
{
    public class CustomerEvent : BaseEvent
    {
        public CustomerEvent(Guid customerId){
            EventType = "Customer";
            CustomerId = customerId;
        }
        public Guid CustomerId { get; }
    }
}