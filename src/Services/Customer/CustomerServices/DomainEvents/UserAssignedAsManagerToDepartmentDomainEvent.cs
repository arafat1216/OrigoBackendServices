using Common.Logging;
using CustomerServices.Models;
using System;

namespace CustomerServices.DomainEvents
{
    public class UserAssignedAsManagerToDepartmentDomainEvent : BaseEvent
    {
        public UserAssignedAsManagerToDepartmentDomainEvent(User user, Guid departmentId) : base(user.UserId)
        {
            User = user;
            DepartmentId = departmentId;
        }

        public User User { get; protected set; }

        public Guid DepartmentId { get; protected set; }

        public override string EventMessage()
        {
            return $"User assigned as manager for department {DepartmentId}.";
        }
    }
}
