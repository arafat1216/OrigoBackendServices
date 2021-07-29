using System;
using MediatR;

namespace Common.Logging
{
    public class BaseEvent : INotification, IEvent
    {
        protected BaseEvent(Guid id)
        {
            Id = id;
            CreationDate = DateTime.UtcNow;
        }

        public Guid Id { get; }
        public DateTime CreationDate { get; }
    }
}