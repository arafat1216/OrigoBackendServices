using System;

namespace Common.Logging
{
    public class BaseEvent
    {
        public BaseEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        public Guid Id { get; private set; }
        public DateTime CreationDate { get; private set; }
    }
}