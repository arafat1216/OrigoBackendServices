using System;

namespace Common.Logging
{
    public interface IEvent
    {
        public Guid Id { get;  }
        public DateTime CreationDate { get;  }
    }
}