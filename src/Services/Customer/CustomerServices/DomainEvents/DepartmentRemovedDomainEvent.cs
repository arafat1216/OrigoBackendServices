using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class DepartmentRemovedDomainEvent : BaseEvent
    {
        public DepartmentRemovedDomainEvent(Department department) : base(department.ExternalDepartmentId)
        {
            Department = department;
        }

        public Department Department { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Department {Id} removed.";
        }
    }
}
