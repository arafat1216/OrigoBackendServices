using System;
using System.Collections.Generic;

namespace CustomerServices.ServiceModels
{
    public class DepartmentDTO
    {
        public DepartmentDTO()
        {

        }

        public Guid DepartmentId { get; set; }

        public string Name { get; set; }

        public string CostCenterId { get; set; }

        public string Description { get; set; }

        public Guid? ParentDepartmentId { get; set; }

        public Guid CallerId { get; set; }
        public IList<ManagedByDTO> ManagedBy { get; set; }
    }
}
