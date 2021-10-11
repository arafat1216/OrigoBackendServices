using Common.Logging;
using CustomerServices.Models;
using System;

namespace CustomerServices.DomainEvents
{
    public class UserUnassignedFromDepartmentDomainEvent : BaseEvent
    {
        public UserUnassignedFromDepartmentDomainEvent(User user, Guid departmentId) : base(user.UserId)
        {
            User = user;
            DepartmentId = departmentId;
        }

        public User User { get; protected set; }

        public Guid DepartmentId { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Removed assigned department {DepartmentId}.";
        }
    }
}
