using System;
using MediatR;

namespace Common.Logging
{
    public class BaseEvent : INotification, IEvent
    {
        public BaseEvent()
        { }

        protected BaseEvent(Guid id)
        {
            Id = id;
            CreationDate = DateTime.UtcNow;
        }

        public Guid Id { get; protected set; }
        public DateTime CreationDate { get; }
        public virtual string EventMessage(string languageCode = "nb-NO")
        {
            return $"Event for {Id}";
        }
    }
}