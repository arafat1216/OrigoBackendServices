using System;

namespace AssetServices.Events
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