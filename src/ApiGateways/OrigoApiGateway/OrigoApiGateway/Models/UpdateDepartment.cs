using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    public class UpdateDepartment
    {
        public Guid DepartmentId { get; init; }

        public string Name { get; init; }

        public string CostCenterId { get; init; }

        public string Description { get; init; }

        public Guid? ParentDepartmentId { get; init; }
        public IList<Guid> ManagedBy { get; set; } = new List<Guid>();
    }
}
