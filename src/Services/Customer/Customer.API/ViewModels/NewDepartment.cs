using System;

namespace Customer.API.ViewModels
{
    public class NewDepartment
    {
        public string Name { get; set; }

        public string CostCenterId { get; set; }

        public string Description { get; set; }

        public Guid? ParentDepartmentId { get; set; }
        public Guid CallerId { get; set; }
    }
}
