using System;
using Common.Logging;

namespace Asset.API.Events
{
    public class CustomerEvent : BaseEvent
    {
        public CustomerEvent(Guid customerId)
        {
            CustomerId = customerId;
        }

        public Guid CustomerId { get; }
    }
}