using Common.Logging;
using CustomerServices.Models;
using System;

namespace CustomerServices.DomainEvents
{
    public class DepartmentAddDepartmentManagerDomainEvent : BaseEvent
    {
        public DepartmentAddDepartmentManagerDomainEvent(Department department, User manager, Guid callerId) : base(department.ExternalDepartmentId)
        {
            Manager = manager;
            Department = department;
            CallerId = callerId;
        }

        public Department Department { get; set; }
        public User Manager { get; set; }
        public Guid CallerId { get; set; }

        public override string EventMessage()
        {
            return Manager != null
                ? $"User {Manager.FirstName} {Manager.LastName} was assign as manager to department {Department.Name}"
                : $"New user was assign as manager to department {Department.Name}";
        }
    }
}
