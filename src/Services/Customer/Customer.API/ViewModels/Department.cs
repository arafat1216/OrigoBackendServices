﻿using System;

namespace Customer.API.ViewModels
{
    public class Department
    {
        public Department()
        {

        }

        public Department(CustomerServices.Models.Department department)
        {
            DepartmentId = department.ExternalDepartmentId;
            Name = department.Name;
            CostCenterId = department.CostCenterId;
            ParentDepartmentId = department?.ParentDepartment?.ExternalDepartmentId;
            Description = department.Description;
        }

        public Guid DepartmentId { get; set; }

        public string Name { get; set; }

        public string CostCenterId { get; set; }

        public string Description { get; set; }

        public Guid? ParentDepartmentId { get; set; }
    }
}
