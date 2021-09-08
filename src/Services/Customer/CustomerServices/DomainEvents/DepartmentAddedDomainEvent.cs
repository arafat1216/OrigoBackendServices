using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class DepartmentAddedDomainEvent : BaseEvent
    {
        public DepartmentAddedDomainEvent(Department department) : base(department.ExternalDepartmentId)
        {
            NewDepartment = department;
        }

        public Department NewDepartment { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Department {Id} created.";
        }
    }
}
