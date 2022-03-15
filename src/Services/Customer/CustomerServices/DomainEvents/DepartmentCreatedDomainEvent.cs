using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class DepartmentCreatedDomainEvent : BaseEvent
    {
        public DepartmentCreatedDomainEvent(Department department) : base(department.ExternalDepartmentId)
        {
            NewDepartment = department;
        }

        public Department NewDepartment { get; protected set; }

        public override string EventMessage()
        {
            return $"Department {NewDepartment.ExternalDepartmentId} created.";
        }
    }
}
