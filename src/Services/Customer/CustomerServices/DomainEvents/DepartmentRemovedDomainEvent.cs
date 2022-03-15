using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class DepartmentRemovedDomainEvent : BaseEvent
    {
        public DepartmentRemovedDomainEvent(Department department) : base(department.ExternalDepartmentId)
        {
            DeletedDepartment = department;
        }

        public Department DeletedDepartment { get; protected set; }

        public override string EventMessage()
        {
            return $"Department {Id} removed.";
        }
    }
}
