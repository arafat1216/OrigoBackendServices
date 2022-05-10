using System;
using System.Collections.Generic;

namespace Customer.API.WriteModels
{
    public class UpdateDepartment
    {
        public Guid DepartmentId { get; set; }

        public string Name { get; set; }

        public string CostCenterId { get; set; }

        public string Description { get; set; }

        public Guid? ParentDepartmentId { get; set; }

        public Guid CallerId { get; set; }
        public IList<Guid> ManagedBy { get; set; } = new List<Guid>();
    }
}
