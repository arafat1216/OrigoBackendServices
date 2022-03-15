using Common.Logging;
using CustomerServices.Models;
using System;

namespace CustomerServices.DomainEvents
{
    public class UserUnassignedAsManagerFromDepartmentDomainEvent : BaseEvent
    {
        public UserUnassignedAsManagerFromDepartmentDomainEvent(User user, Guid departmentId) : base(user.UserId)
        {
            User = user;
            DepartmentId = departmentId;
        }

        public User User { get; protected set; }

        public Guid DepartmentId { get; protected set; }

        public override string EventMessage()
        {
            return $"User unassigned as manager for department {DepartmentId}.";
        }

    }

}
