using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using Common.Seedwork;
using CustomerServices.DomainEvents;

namespace CustomerServices.Models
{
    public class User : Entity, IAggregateRoot
    {
        protected IList<Department> departments;
        protected IList<Department> managesDepartments;

        public User(Customer customer, Guid userId, string firstName, string lastName, string email, string mobileNumber, string employeeId)
        {
            Customer = customer;
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            MobileNumber = mobileNumber;
            EmployeeId = employeeId;
            AddDomainEvent(new UserCreatedDomainEvent(this));
        }

        protected User() { }

        public Guid UserId { get; set; }
        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }
        public string Email { get; protected set; }
        public string MobileNumber { get; protected set; }
        public string EmployeeId { get; protected set; }
        [JsonIgnore]
        public Customer Customer { get; set; }
        public IReadOnlyCollection<Department> Departments { get { return new ReadOnlyCollection<Department>(departments); } protected set { departments = new List<Department>(value); } }

        public IReadOnlyCollection<Department> ManagesDepartments { get { return new ReadOnlyCollection<Department>(managesDepartments); } protected set { managesDepartments = new List<Department>(value); } }

        public void AssignDepartment(Department department)
        {
            AddDomainEvent(new UserAssignedToDepartmentDomainEvent(this, department.ExternalDepartmentId));
            departments.Add(department);
        }

        public void UnassignDepartment(Department department)
        {
            AddDomainEvent(new UserUnassignedFromDepartmentDomainEvent(this, department.ExternalDepartmentId));
            departments.Remove(department);
        }

        public void AssignManagerToDepartment(Department department)
        {
            if (managesDepartments == null)
            {
                managesDepartments = new List<Department>();
            }
            AddDomainEvent(new UserAssignedAsManagerToDepartmentDomainEvent(this, department.ExternalDepartmentId));
            managesDepartments.Add(department);
        }

        public void UnassignManagerFromDepartment(Department department)
        {
            if (managesDepartments == null)
            {
                managesDepartments = new List<Department>();
            }
            AddDomainEvent(new UserAssignedAsManagerToDepartmentDomainEvent(this, department.ExternalDepartmentId));
            managesDepartments.Remove(department);
        }
    }
}