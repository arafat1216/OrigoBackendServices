using System;

namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Request and response object
    /// </summary>
    public class OrigoDepartment
    {
        public Guid DepartmentId { get; init; }

        public string Name { get; init; }

        public string CostCenterId { get; init; }

        public string Description { get; init; }

        public Guid? ParentDepartmentId { get; init; }
    }
}
