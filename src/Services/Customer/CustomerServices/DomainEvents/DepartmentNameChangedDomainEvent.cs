using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class DepartmentNameChangedDomainEvent : BaseEvent
    {
        public DepartmentNameChangedDomainEvent(Department department, string oldDepartmentName) : base(department.ExternalDepartmentId)
        {
            Department = department;
            OldDepartmentName = oldDepartmentName;
        }
        public string OldDepartmentName { get; protected set; }

        public Department Department { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Department name changed from {OldDepartmentName} to {Department.Name}.";
        }
    }
}
