using Common.Logging;
using System;
using System.Collections.Generic;

namespace CustomerServices.DomainEvents
{
    class UserAccessRemovedDomainEvent : BaseEvent
    {
        public UserAccessRemovedDomainEvent(Guid userId, Guid accessRemoved, IList<Guid> accessList) : base(userId)
        {
            UserAccessRemoved = accessRemoved;
            UserAccessList = accessList;
        }

        public Guid UserAccessRemoved { get; protected set; }

        public IList<Guid> UserAccessList { get; protected set; }

        public override string EventMessage()
        {
            return $"User access removed.";
        }
    }
}
