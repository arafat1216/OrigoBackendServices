using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class DepartmentCostCenterIdChangedDomainEvent : BaseEvent
    {
        public DepartmentCostCenterIdChangedDomainEvent(Department department, string oldCostCenterId) : base(department.ExternalDepartmentId)
        {
            Department = department;
            OldCostCenterId = oldCostCenterId;
        }
        public string OldCostCenterId { get; protected set; }

        public Department Department { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Department cost center id changed from {OldCostCenterId} to {Department.CostCenterId}.";
        }
    }
}
