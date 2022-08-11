using MediatR;

namespace Common.Logging
{
    public class BaseEvent : INotification, IEvent
    {
        protected BaseEvent()
        { }

        protected BaseEvent(Guid id)
        {
            Id = id;
            CreationDate = DateTime.UtcNow;
        }

        public Guid Id { get; protected set; }
        public DateTime CreationDate { get; }
        /// <summary>
        /// Event message used by the auditlog.
        /// </summary>
        public virtual string EventMessage()
        {
            return $"Event for {Id}";
        }
    }
}