using Common.Enums;
using Common.Seedwork;
using CustomerServices.DomainEvents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace CustomerServices.Models
{
    public class User : Entity, IAggregateRoot
    {
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
            _userStatus = UserStatus.NotInvited;
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
        public DateTime? LastWorkingDay { get; protected set; } = null;

        /// <summary>
        /// TODO: this will be remove in a later version
        /// </summary>
        public string MobileNumber { get; protected set; }

        /// <summary>
        /// TODO: this will be remove in a later version
        /// </summary>
        public string EmployeeId { get; protected set; }

        public UserPreference UserPreference { get; protected set; }

        /// <summary>
        /// The current status of this User.
        /// </summary>
        public UserStatus UserStatus
        {
            get => _userStatus;
            init => _userStatus = value;
        }
        private UserStatus _userStatus;
        /// <summary>
        /// Returns the state of current UserStatus.
        /// </summary>
        public bool IsActiveState => UserStatus is UserStatus.Invited or UserStatus.Activated;

        public string OktaUserId { get; protected set; }

        [JsonIgnore]
        public Organization Customer { get; set; }
        public void ChangeUserStatus(string? oktaUserId, UserStatus newStatus)
        {
            LastUpdatedDate = DateTime.UtcNow;
            OktaUserId = oktaUserId;
            var oldStatus = _userStatus;
            _userStatus = newStatus;
            AddDomainEvent(new UserStatusChangedDomainEvent(this, oldStatus));

        }

        public Department? Department { get; set; }

        public DateTime? LastDayForReportingSalaryDeduction
        {
            get
            {
                if (LastWorkingDay == null || Customer == null || Customer.LastDayForReportingSalaryDeduction == null)
                    return null;
                var deductionDate = new DateTime(LastWorkingDay.Value.Year, LastWorkingDay.Value.Month, Customer.LastDayForReportingSalaryDeduction.Value);
                if ((LastWorkingDay.Value - deductionDate).TotalDays > 0)
                    return deductionDate;
                else
                    return deductionDate.AddMonths(-1);
            }
        }

        public IReadOnlyCollection<Department> ManagesDepartments
        {
            get => new ReadOnlyCollection<Department>(_managesDepartments);
            protected set => _managesDepartments = value != null ? new List<Department>(value) : new List<Department>();
        }
        /// <summary>
        /// The user gets status OnboardInitiated when they log in for the first time. The call stams from get userpermission. 
        /// Only allowed to change from the status Invited, and the user can not be set to invited more then once.
        /// </summary>
        public void OnboardingInitiated()
        {
            if(_userStatus == UserStatus.Invited)
            { 
                ChangeUserStatus(null, UserStatus.OnboardInitiated);
                AddDomainEvent(new UserOnboardingInitiatedDomainEvent(this));
            }
        }
        public void OffboardingInitiated(DateTime lastWorkingDay, Guid callerId)
        {
            UpdatedBy = callerId;
            SetLastWorkingDay(lastWorkingDay, callerId);
            ChangeUserStatus(null, UserStatus.OffboardInitiated);
            AddDomainEvent(new OffboardingInitiatedDomainEvent(this, callerId));
            LastUpdatedDate = DateTime.UtcNow;
        }
        public void OffboardingCancelled(Guid callerId)
        {
            UpdatedBy = callerId;
            LastWorkingDay = null;
            ChangeUserStatus(null, UserStatus.Activated);
            AddDomainEvent(new OffboardingCancelledDomainEvent(this, callerId));
            LastUpdatedDate = DateTime.UtcNow;
        }
        public void OffboardingOverdued(Guid callerId)
        {
            UpdatedBy = callerId;
            ChangeUserStatus(null, UserStatus.OffboardOverdue);
            AddDomainEvent(new OffboardingOverduedDomainEvent(this, callerId));
            LastUpdatedDate = DateTime.UtcNow;
        }
        public void AssignDepartment(Department department, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            AddDomainEvent(new UserAssignedToDepartmentDomainEvent(this, department.ExternalDepartmentId));
            Department = department;
        }

        public void UnassignDepartment(Department department, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            AddDomainEvent(new UserUnassignedFromDepartmentDomainEvent(this, department.ExternalDepartmentId));
            Department = null;
        }

        internal void UnAssignAllDepartments(Guid OrganizationId)
        {
            Department = null;

            var manageToDepartments = _managesDepartments.Where(department => department.Customer.OrganizationId == OrganizationId).ToList();

            foreach (var department in manageToDepartments)
            {
                _managesDepartments.Remove(department);
            }
        }

        public void AssignManagerToDepartment(Department department, Guid callerId)
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

        public void UnassignManagerFromDepartment(Department department, Guid callerId)
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

        internal void SetLastWorkingDay(DateTime? lastWorkingDay, Guid callerId)
        {
            UpdatedBy = callerId;
            AddDomainEvent(new SetLastWorkingDayDomainEvent(this, lastWorkingDay, callerId));
            LastUpdatedDate = DateTime.UtcNow;
            LastWorkingDay = lastWorkingDay;
        }
        internal void ChangeUserPreferences(UserPreference userPreference, Guid callerId)
        {
            AddDomainEvent(new UserPreferenceChangedDomainEvent(userPreference, UserId));

            // Allow old users with no preference object ot be updated.
            if (UserPreference == null)
                UserPreference = new UserPreference("EN", callerId);
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

            UserPreference?.SetDeleteStatus(true);
        }

        internal void ChangeFirstName(string firstName, Guid callerId)
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
            var oldEmail = Email;
            Email = email;
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            AddDomainEvent(new UserUpdateEmailDomainEvent(this, oldEmail));
        }

        internal void ChangeMobileNumber(string mobileNumber, Guid callerId)
        {
            var oldMobileNumber = MobileNumber;
            MobileNumber = mobileNumber;
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            AddDomainEvent(new UserUpdateMobileNumberDomainEvent(this, oldMobileNumber, callerId));
        }

        // TODO: this will be remove in a later version
        internal void ChangeEmployeeId(string employeeId, Guid callerId)
        {
            var oldEmployeeId = EmployeeId;
            EmployeeId = employeeId;
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            AddDomainEvent(new UserUpdateEmployeeIdDomainEvent(this, oldEmployeeId));
        }
    }
}