using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Common.Seedwork;
using CustomerServices.DomainEvents;

namespace CustomerServices.Models
{
    public class User : Entity, IAggregateRoot
    {
        private IList<Department> _departments = new List<Department>();
        private IList<Department> _managesDepartments = new List<Department>();

        public User(Organization customer, Guid userId, string firstName, string lastName, string email, string mobileNumber, string employeeId, UserPreference userPreference)
        {
            Customer = customer;
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            MobileNumber = mobileNumber;
            EmployeeId = employeeId;
            UserPreference = (userPreference == null) ? new UserPreference("EN") : userPreference;
            IsActive = false;
            OktaUserId = "";
            AddDomainEvent(new UserCreatedDomainEvent(this));
        }

        public void UpdateUser(User user)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            MobileNumber = user.MobileNumber;
            EmployeeId = user.EmployeeId;
            UserPreference = user.UserPreference;
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

        public void ActivateUser(string oktaUserId)
        {
            OktaUserId = oktaUserId;
            IsActive = true;
        }

        public void DeactivateUser()
        {
            IsActive = false;
        }

        public IReadOnlyCollection<Department> Departments
        {
            get => new ReadOnlyCollection<Department>(_departments);
            protected set => _departments = new List<Department>(value);
        }

        public IReadOnlyCollection<Department> ManagesDepartments
        {
            get => new ReadOnlyCollection<Department>(_managesDepartments);
            protected set => _managesDepartments = value != null ? new List<Department>(value) : new List<Department>();
        }

        public void AssignDepartment(Department department)
        {
            AddDomainEvent(new UserAssignedToDepartmentDomainEvent(this, department.ExternalDepartmentId));
            _departments.Add(department);
        }

        public void UnassignDepartment(Department department)
        {
            AddDomainEvent(new UserUnassignedFromDepartmentDomainEvent(this, department.ExternalDepartmentId));
            _departments.Remove(department);
        }

        public void AssignManagerToDepartment(Department department)
        {
            if (_managesDepartments == null)
            {
                _managesDepartments = new List<Department>();
            }
            AddDomainEvent(new UserAssignedAsManagerToDepartmentDomainEvent(this, department.ExternalDepartmentId));
            _managesDepartments.Add(department);
        }

        public void UnassignManagerFromDepartment(Department department)
        {
            if (_managesDepartments == null)
            {
                _managesDepartments = new List<Department>();
            }
            AddDomainEvent(new UserAssignedAsManagerToDepartmentDomainEvent(this, department.ExternalDepartmentId));
            _managesDepartments.Remove(department);
        }

        internal void ChangeUserPreferences(UserPreference userPreference)
        {
            AddDomainEvent(new UserPreferenceChangedDomainEvent(userPreference, UserId));

            // Allow old users with no preference object ot be updated.
            if (UserPreference == null)
                UserPreference = new UserPreference("EN");
            UserPreference.Language = userPreference.Language;
        }

        internal void SetDeleteStatus(bool isDeleted)
        {
            AddDomainEvent(new UserDeletedDomainEvent(this));
            IsDeleted = isDeleted;
            UserPreference.SetDeleteStatus(true);
        }

        internal void ChangeFirstName(string firstName)
        {
            var oldNameValue = FirstName;
            AddDomainEvent(new UserUpdateFirstNameDomainEvent(this, oldNameValue));
            FirstName = firstName;
        }

        internal void ChangeLastName(string lastName)
        {
            var oldNameValue = LastName;
            AddDomainEvent(new UserUpdateLastNameDomainEvent(this, oldNameValue));
            LastName = lastName;
        }

        internal void ChangeEmailAddress(string email)
        {
            Email = email;
        }

        // TODO: this will be remove in a later version
        internal void ChangeEmployeeId(string employeeId)
        {
            EmployeeId = employeeId;
        }
    }
}