using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class DepartmentDescriptionChangedDomainEvent : BaseEvent
    {
        public DepartmentDescriptionChangedDomainEvent(Department department, string oldDescription) : base(department.ExternalDepartmentId)
        {
            Department = department;
            OldDescription = oldDescription;
        }
        public string OldDescription { get; protected set; }

        public Department Department { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Department description changed from \"{OldDescription}\" to \"{Department.Description}\".";
        }
    }
}
