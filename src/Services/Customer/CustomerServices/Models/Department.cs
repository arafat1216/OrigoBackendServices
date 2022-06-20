using Common.Seedwork;
using CustomerServices.DomainEvents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using System.Linq;

namespace CustomerServices.Models
{
    public class Department : Entity
    {
        private IList<User> _managers = new List<User>();
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

        public void UpdateDepartment(Department department)
        {
            Name = department.Name;
            CostCenterId = department.CostCenterId;
            Description = department.Description;
            ParentDepartment = department.ParentDepartment;
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
        public IReadOnlyCollection<User> Managers 
        {
            get => new ReadOnlyCollection<User>(_managers);
            protected set => _managers = value != null ? new List<User>(value) : new List<User>();
        }

        /// <summary>
        /// Checks if the input department is a sub department of this department or if the input department is this department.
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        public bool HasSubDepartment(Department department)
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
        /// Returns a list of all sub departments of this department
        /// </summary>
        /// <returns></returns>
        public IList<Department> SubDepartments(IList<Department> departments)
        {
            var subDepartments = new List<Department>();
            if (departments != null)
            {
                subDepartments.AddRange(from department in departments
                                        where HasSubDepartment(department)
                                        select department);
            }

            return subDepartments;
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
        public void AddDepartmentManager(User manager, Guid CallerId)
        {
            if (_managers == null) 
            { 
                _managers = new List<User>();
            }
            _managers.Add(manager);
            UpdatedBy = CallerId;
            LastUpdatedDate = DateTime.UtcNow;
        }
        public void UpdateDepartmentManagers(IList<User> managers, Guid CallerId)
        {
            if (_managers == null)
            {
                _managers = new List<User>();
            }
            _managers = managers;
            UpdatedBy = CallerId;
            LastUpdatedDate = DateTime.UtcNow;
        }
        
        public void RemoveDepartmentManagers(IList<User> managers, Guid CallerId)
        {
            if(_managers != null)
            {
                foreach (User manager in managers)
                {
                    _managers.Remove(manager);
                }
            }
            UpdatedBy = CallerId;
            LastUpdatedDate = DateTime.UtcNow;
        }
    }
}
