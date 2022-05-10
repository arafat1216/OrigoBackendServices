using Common.Logging;
using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.DomainEvents
{
    public  class DepartmentUpdateDepartmentManagersDomainEvent : BaseEvent
    {
        public DepartmentUpdateDepartmentManagersDomainEvent(Department department, Guid callerId)
        {
            Department = department;
            CallerId = callerId;
        }

        public Department Department { get; set; }
        public Guid CallerId { get; set; }

        public override string EventMessage()
        {
            return $"New department managers was added for department {Department.Name}";
               
        }
    }
}
