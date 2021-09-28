using Common.Logging;
using System;
using System.Collections.Generic;

namespace CustomerServices.DomainEvents
{
    class UserAccessAddedDomainEvent : BaseEvent
    {
        public UserAccessAddedDomainEvent(Guid userId, Guid accessAdded, IList<Guid> accessList) : base(userId)
        {
            UserAccessAdded = accessAdded;
            UserAccessList = accessList;
        }

        public Guid UserAccessAdded { get; protected set; }

        public IList<Guid> UserAccessList { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"User access added.";
        }
    }
}
