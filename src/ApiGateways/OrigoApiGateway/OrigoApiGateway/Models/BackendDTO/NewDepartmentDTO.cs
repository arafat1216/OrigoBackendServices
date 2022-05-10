using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.BackendDTO
{
    public record NewDepartmentDTO
    {
        public string Name { get; set; }

        public string CostCenterId { get; set; }

        public string Description { get; set; }

        public Guid ParentDepartmentId { get; set; }
        public Guid CallerId { get; set; }
        public IList<Guid> ManagedBy { get; init; }


    }
}
