using Common.Logging;
using CustomerServices.Models;
using System;

namespace CustomerServices.DomainEvents
{
    class UserAssignedToDepartmentDomainEvent : BaseEvent
    {
        public UserAssignedToDepartmentDomainEvent(User user, Guid departmentId) : base(user.UserId)
        {
            User = user;
            DepartmentId = departmentId;
        }

        public User User { get; protected set; }

        public Guid DepartmentId { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"User assigned department {DepartmentId}.";
        }
    }
}
