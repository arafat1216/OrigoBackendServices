using Common.Logging;
using CustomerServices.Models;
using System;

namespace CustomerServices.DomainEvents
{
    public class DepartmentRemoveDepartmentManagersDomainEvent : BaseEvent
    {
        public DepartmentRemoveDepartmentManagersDomainEvent(Department department, Guid callerId)
        {
            Department = department;
            CallerId = callerId;
        }

        public Department Department { get; set; }
        public Guid CallerId { get; set; }
        public override string EventMessage()
        {
            return $"Removed managers for department {Department.Name}";

        }
    }
}
