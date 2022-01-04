using Common.Seedwork;
using CustomerServices.DomainEvents;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CustomerServices.Models
{
    public class Department : Entity
    {
        protected Department() { }

        public Department(string name, string costCenterId, string description, Organization customer, Guid externalDepartmentId,Guid callerId, Department parentDepartment = null)
        {
            Name = name;
            CostCenterId = costCenterId;
            Description = description;
            Customer = customer;
            ParentDepartment = parentDepartment;
            ExternalDepartmentId = externalDepartmentId;
            CreatedBy = callerId;
            AddDomainEvent(new DepartmentCreatedDomainEvent(this));
        }

        public Guid ExternalDepartmentId { get; protected set; }

        public string Name { get; set; }

        public string CostCenterId { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Can link back to another structur to create a branching tree-structure
        /// </summary>
        public Department ParentDepartment { get; set; }

        /// <summary>
        /// The organization that this structure belongs to
        /// </summary>
        [JsonIgnore]
        public Organization Customer { get; set; }

        /// <summary>
        /// The associated users for this department.
        /// </summary>
        [JsonIgnore]
        public IReadOnlyCollection<User> Users { get; set; }

        /// <summary>
        /// The users allowed to manage this department.
        /// </summary>
        [JsonIgnore]
        public IReadOnlyCollection<User> Managers { get; set; }

        /// <summary>
        /// Checks if the input department is a subdepartment of this department or if the input department is this department.
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        public bool HasSubdepartment(Department department)
        {
            if (department == null)
                return false;
            var tempDepartment = department;
            do
            {
                if (ExternalDepartmentId == tempDepartment.ExternalDepartmentId)
                    return true;
                if (tempDepartment.ParentDepartment != null)
                    tempDepartment = tempDepartment.ParentDepartment;
                else return false;
            } while (true);
        }

        /// <summary>
        /// Returns a list of all subdepartments of this department
        /// </summary>
        /// <returns></returns>
        public IList<Department> Subdepartments(IList<Department> departments)
        {
            List<Department> subdepartments = new List<Department>();
            foreach (var department in departments)
            {
                if (HasSubdepartment(department))
                    subdepartments.Add(department);
            }
            return subdepartments;
        }

        public void SetUpdatedBy(Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
        }
        public void SetDeletedBy(Guid callerId)
        {
            DeletedBy = callerId;
            LastUpdatedDate= DateTime.UtcNow;
        }
    }
}
