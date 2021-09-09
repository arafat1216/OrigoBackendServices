using Common.Seedwork;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CustomerServices.Models
{
    public class Department : Entity
    {
        protected Department() { }

        public Department(string name, string costCenterId, string description, Customer customer, Guid externalDepartmentId, Department parentDepartment = null)
        {
            Name = name;
            CostCenterId = costCenterId;
            Description = description;
            Customer = customer;
            ParentDepartment = parentDepartment;
            ExternalDepartmentId = externalDepartmentId;
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
        public Customer Customer { get; set; }

        [JsonIgnore]
        public IReadOnlyCollection<User> Users { get; set; }
    }
}
