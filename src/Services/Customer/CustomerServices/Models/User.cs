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
        private IList<Department> _departments = new List<Department>();
        private IList<Department> _managesDepartments = new List<Department>();

        public User(Organization customer, Guid userId, string firstName, string lastName, string email, string mobileNumber, string employeeId, UserPreference userPreference, Guid callerId)
        {
            Customer = customer;
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            MobileNumber = mobileNumber;
            EmployeeId = employeeId;
            CreatedBy = callerId;
            UserPreference = (userPreference == null) ? new UserPreference("EN", callerId) : userPreference;
            IsActive = false;
            OktaUserId = "";
            AddDomainEvent(new UserCreatedDomainEvent(this));
        }

        protected User() { }

        public Guid UserId { get; set; }
        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }
        public string Email { get; protected set; }

        /// <summary>
        /// TODO: this will be remove in a later version
        /// </summary>
        public string MobileNumber { get; protected set; }

        /// <summary>
        /// TODO: this will be remove in a later version
        /// </summary>
        public string EmployeeId { get; protected set; }

        public UserPreference UserPreference { get; protected set; }

        public bool IsActive { get; protected set; }
        public string OktaUserId { get; protected set; }

        [JsonIgnore]
        public Organization Customer { get; set; }

        public void ActivateUser(string oktaUserId,Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            OktaUserId = oktaUserId;
            IsActive = true;
        }

        public void DeactivateUser(Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            IsActive = false;
        }

        public IReadOnlyCollection<Department> Departments { 
            get => new ReadOnlyCollection<Department>(_departments);
            protected set => _departments = new List<Department>(value);
        }

        public IReadOnlyCollection<Department> ManagesDepartments
        {
            get => new ReadOnlyCollection<Department>(_managesDepartments);
            protected set => _managesDepartments = value != null ? new List<Department>(value) : new List<Department>();
        }

        public void AssignDepartment(Department department, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            AddDomainEvent(new UserAssignedToDepartmentDomainEvent(this, department.ExternalDepartmentId));
            _departments.Add(department);
        }

        public void UnassignDepartment(Department department, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            AddDomainEvent(new UserUnassignedFromDepartmentDomainEvent(this, department.ExternalDepartmentId));
            _departments.Remove(department);
        }

        public void AssignManagerToDepartment(Department department,Guid callerId)
        {
            if (_managesDepartments == null)
            {
                _managesDepartments = new List<Department>();
            }
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            AddDomainEvent(new UserAssignedAsManagerToDepartmentDomainEvent(this, department.ExternalDepartmentId));
            _managesDepartments.Add(department);
        }

        public void UnassignManagerFromDepartment(Department department,Guid callerId)
        {
            if (_managesDepartments == null)
            {
                _managesDepartments = new List<Department>();
            }
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            AddDomainEvent(new UserAssignedAsManagerToDepartmentDomainEvent(this, department.ExternalDepartmentId));
            _managesDepartments.Remove(department);
        }

        internal void ChangeUserPreferences(UserPreference userPreference, Guid callerId)
        {
            AddDomainEvent(new UserPreferenceChangedDomainEvent(userPreference, UserId));

            // Allow old users with no preference object ot be updated.
            if (UserPreference == null)
                UserPreference = new UserPreference("EN",callerId);
            UserPreference.Language = userPreference.Language;
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
        }

        internal void SetDeleteStatus(bool isDeleted, Guid callerId)
        {
            DeletedBy = callerId;
            AddDomainEvent(new UserDeletedDomainEvent(this));
            IsDeleted = isDeleted;
            LastUpdatedDate = DateTime.UtcNow;
            UserPreference.SetDeleteStatus(true);
        }

        internal void ChangeFirstName(string firstName,Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            var oldNameValue = FirstName;
            AddDomainEvent(new UserUpdateFirstNameDomainEvent(this, oldNameValue));
            FirstName = firstName;
        }

        internal void ChangeLastName(string lastName, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            var oldNameValue = LastName;
            AddDomainEvent(new UserUpdateLastNameDomainEvent(this, oldNameValue));
            LastName = lastName;
        }

        internal void ChangeEmailAddress(string email, Guid callerId)
        {
            Email = email;
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
        }

        // TODO: this will be remove in a later version
        internal void ChangeEmployeeId(string employeeId, Guid callerId)
        {
            EmployeeId = employeeId;
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
        }
    }
}