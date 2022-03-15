using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class DepartmentAddedToCustomerDomainEvent : BaseEvent
    {
        public DepartmentAddedToCustomerDomainEvent(Department department) : base(department.ExternalDepartmentId)
        {
            AddedDepartment = department;
        }

        public Department AddedDepartment { get; protected set; }

        public override string EventMessage()
        {
            return $"Department {AddedDepartment.ExternalDepartmentId} added to customer {AddedDepartment.Customer}.";
        }
    }
}
