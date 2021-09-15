using Common.Logging;
using CustomerServices.Models;
using System;

namespace CustomerServices.DomainEvents
{
    class DepartmentParentDepartmentChangedDomainEvent : BaseEvent
    {
        public DepartmentParentDepartmentChangedDomainEvent(Department department, Guid? oldParentDepartmentId) : base(department.ExternalDepartmentId)
        {
            Department = department;
            OldParentDepartmentId = oldParentDepartmentId;
        }
        public Guid? OldParentDepartmentId { get; protected set; }

        public Department Department { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Department parent department id changed from {OldParentDepartmentId} to {Department.ParentDepartment?.Id}.";
        }
    }
}
