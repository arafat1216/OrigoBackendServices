using OrigoApiGateway.Models.BackendDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models
{
    public class OrigoDepartment
    {
        public OrigoDepartment()
        {

        }

        public OrigoDepartment(DepartmentDTO department)
        {
            DepartmentId = department.DepartmentId;
            Name = department.Name;
            CostCenterId = department.CostCenterId;
            ParentDepartmentId = department.ParentDepartmentId;
            Description = department.Description;
        }

        public Guid DepartmentId { get; set; }

        public string Name { get; set; }

        public string CostCenterId { get; set; }

        public string Description { get; set; }

        public Guid? ParentDepartmentId { get; set; }
    }
}
