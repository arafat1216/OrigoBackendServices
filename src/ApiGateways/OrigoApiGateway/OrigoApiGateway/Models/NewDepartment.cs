using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Request object
    /// </summary>
    public class NewDepartment
    {
        public string Name { get; set; }

        public string CostCenterId { get; set; }

        public string Description { get; set; }

        public Guid ParentDepartmentId { get; set; }
        public IList<Guid> ManagedBy { get; set; } = new List<Guid>();

    }
}
