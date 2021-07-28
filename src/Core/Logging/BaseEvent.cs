using System;
using MediatR;

namespace Common.Logging
{
    public class BaseEvent : INotification, IEvent
    {
        protected BaseEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        public Guid Id { get; }
        public DateTime CreationDate { get; }
    }
}