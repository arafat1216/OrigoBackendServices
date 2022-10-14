using Common.Seedwork;
using CustomerServices.DomainEvents;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Common.Enums;
using System.ComponentModel.DataAnnotations;

#nullable enable

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace CustomerServices.Models
{
    /// <summary>
    ///     Represents a single juridical entity (in a national registry). Although a organization is typically a customer, it may also be
    ///     used for other juridical entities used through the solution, such as partners, service providers, etc.
    /// </summary>
    public class Organization : Entity, IAggregateRoot
    {
        /// <summary>
        ///     The organization's external ID
        /// </summary>
        public Guid OrganizationId { get; protected set; }

        /// <summary>
        ///     If this is a child-organization, then this is the ID of it's parent. If it don't have any parents, this will be <see langword="null"/>.
        /// </summary>
        public Guid? ParentId { get; protected set; }

        /// <summary>
        ///     The <see cref="Partner"/> that "owns" and handles the customer-relations with this organization. <para>   
        /// 
        ///     This value is required whenever <see cref="IsCustomer"/> is <see langword="true"/>. <br/>
        ///     This value will be <see langword="null"/> for special/custom organization entries (e.g. service-providers) that don't have any active
        ///     customer-relationship, and therefore should not be managed by a partner. </para>
        /// </summary>

        /// <summary>
        /// The current status of this organization.
        /// </summary>
        public CustomerStatus CustomerStatus
        {
            get => _customerStatus;
            init => _customerStatus = value;
        }
        private CustomerStatus _customerStatus;

        [JsonIgnore]
        public Partner? Partner { get; protected set; }
        public Location? PrimaryLocation
        {
            get
            {
                return Locations.FirstOrDefault(x => x.IsPrimary) ?? null;
            }
        }
        /// <summary>
        ///     The name of the organization.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        ///     The organization-number as used in the national registry.
        /// </summary>
        public string OrganizationNumber { get; protected set; }

        /// <summary>
        ///     Last day in month to report Salary Deduction.
        /// </summary>
        [Range(1, 28)]
        public int? LastDayForReportingSalaryDeduction { get; protected set; }

        /// <summary>
        ///     Email where notification will ben sent for payroll information.
        /// </summary>
        [MaxLength(320)]
        public string PayrollContactEmail { get; protected set; } = string.Empty;
        /// <summary>
        /// Customers Techstep id.
        /// </summary>
        public long? TechstepCustomerId { get; protected set; }
        /// <summary>
        /// Responsible owner of the customer in the techstep system.
        /// </summary>
        public string AccountOwner { get; protected set; } = string.Empty;

        public Address Address { get; protected set; }

        public ContactPerson ContactPerson { get; protected set; }

        /// <summary>
        ///     If the value is <see langword="false"/>, then the organization is not currently considered a customer. This is typically for special 
        ///     organizations such as service-providers, partners or other legal entities where we need to store the corresponding organization and
        ///     contact details. Please note that these organizations may potentially also become a customer at a later date. <para>
        ///     
        ///     If the value is <see langword="true"/>, the organization is considered an active customer. </para><para>
        ///     
        ///     Note that whenever <see cref="IsCustomer"/> is set to <see langword="true"/>, the organization typically have stricter
        ///     checks and validations on the provided data. In additional there are typically additional requirements for customers that may need to
        ///     be provided before the value is updated. </para>
        /// </summary>
        public bool IsCustomer { get; protected set; }

        [NotMapped]
        public virtual ICollection<Organization> ChildOrganizations { get; set; }

        /// <summary>
        ///     The organization's preferences and settings.
        /// </summary>
        [NotMapped]
        public virtual OrganizationPreferences Preferences { get; set; }

        /// <summary>
        ///     The organization's office locations/addresses.
        /// </summary>
        [JsonIgnore]
        public ICollection<Location> Locations { get; protected set; } = new List<Location>();
        
        /// <summary>
        /// The departments for this organization
        /// </summary>
        [JsonIgnore]
        public IReadOnlyCollection<Department> Departments => _departments.AsReadOnly();

        private readonly List<Department> _departments = new();
        /// <summary>
        /// Should users be automatically be generated in Okta when created?
        /// Federated users will be generated in Okta through Just-In-Time creation.
        /// </summary>
        public bool AddUsersToOkta { get; internal set; }

        /// <summary>
        ///     A default constructor reserved for EntityFramework. This should not be used in-code.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Organization()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {

        }

        public Organization(Guid organizationId, Guid? parentId, string companyName, string orgNumber,
            Address companyAddress, ContactPerson organizationContactPerson,
            OrganizationPreferences organizationPreferences, Location organizationLocation, Partner? partner,
            bool isCustomer, int? lastSalaryReportingDay, long? techstepCustomerId, string? accountOwner, string payrollEmail = "", bool addUsersToOkta = false)
        {
            Name = companyName;
            ParentId = parentId;
            OrganizationNumber = orgNumber;
            Address = companyAddress;
            ContactPerson = organizationContactPerson;
            OrganizationId = organizationId;
            Preferences = organizationPreferences;
            Partner = partner;
            IsCustomer = isCustomer;
            IsDeleted = false;
            _customerStatus = CustomerStatus.BeforeOnboarding;
            AddUsersToOkta = addUsersToOkta;
            organizationLocation.SetPrimaryLocation(true);
            Locations.Add(organizationLocation);
            LastDayForReportingSalaryDeduction = lastSalaryReportingDay;
            PayrollContactEmail = payrollEmail;
            TechstepCustomerId = techstepCustomerId;
            AccountOwner = accountOwner ?? string.Empty;
            AddDomainEvent(new CustomerCreatedDomainEvent(this));
        }

        public void UpdateOrganization(Organization organization)
        {
            if (ParentId != organization.ParentId)
            {
                var previousParentId = ParentId.ToString();
                ParentId = organization.ParentId;
                AddDomainEvent(new CustomerParentIdChangedDomainEvent(this, previousParentId));
            }

            if (organization.PrimaryLocation != null && (PrimaryLocation != null && !PrimaryLocation.Equals(organization.PrimaryLocation) || PrimaryLocation == null))
            {
                var previousPrimaryLocation = PrimaryLocation!.Name;
                Locations.FirstOrDefault()!.UpdateLocation(organization.PrimaryLocation);
                AddDomainEvent(new CustomerPrimaryLocationChangedDomainEvent(this, previousPrimaryLocation));
            }

            if (Name != organization.Name)
            {
                var oldName = Name;
                Name = organization.Name;
                AddDomainEvent(new CustomerNameChangedDomainEvent(this, oldName));
            }

            if (OrganizationNumber != organization.OrganizationNumber)
            {
                var oldNumber = OrganizationNumber;
                OrganizationNumber = organization.OrganizationNumber;
                AddDomainEvent(new OrganizationNumberChangedDomainEvent(this, oldNumber));
            }

            if (!Address.Equals(organization.Address))
            {
                var oldAddress = Address;
                Address = organization.Address;
                AddDomainEvent(new CustomerAddressChangedDomainEvent(this, oldAddress));
            }

            if (!ContactPerson.Equals(organization.ContactPerson))
            {
                var oldContactPerson = ContactPerson;
                ContactPerson = organization.ContactPerson;
                AddDomainEvent(new ContactPersonChangedDomainEvent(this, oldContactPerson));
            }

            if ((organization.Partner != null && (Partner == null || !Partner.Equals(organization.Partner)))
                || organization.Partner == null && Partner != null)
            {
                ChangePartner(organization.Partner);
            }

            if (AddUsersToOkta != organization.AddUsersToOkta)
            {
                AddDomainEvent(new AddUsersToOktaChangedDomainEvent(this));
                AddUsersToOkta = organization.AddUsersToOkta;
            }

            if(organization.LastDayForReportingSalaryDeduction != null &&  (LastDayForReportingSalaryDeduction != organization.LastDayForReportingSalaryDeduction))
            {
                var previousDate = organization.LastDayForReportingSalaryDeduction;
                LastDayForReportingSalaryDeduction = organization.LastDayForReportingSalaryDeduction;
                AddDomainEvent(new SetLastDayOfReportingSalaryDeductionDomainEvent(this, previousDate));
            }

            if (PayrollContactEmail != organization.PayrollContactEmail)
            {
                var previousEmail = organization.PayrollContactEmail;
                PayrollContactEmail = organization.PayrollContactEmail;
                AddDomainEvent(new SetPayrollContactEmailDomainEvent(this, previousEmail));
            }
        }

        public void Delete(Guid callerId)
        {
            IsDeleted = true;
            LastUpdatedDate = DateTime.UtcNow;
            DeletedBy = callerId;
            AddDomainEvent(new CustomerDeletedDomainEvent(this));
        }

        public void AddDepartment(Department department, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            AddDomainEvent(new DepartmentAddedToCustomerDomainEvent(department));
            _departments.Add(department);
        }

        public void RemoveDepartment(Department department, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            department.SetDeletedBy(callerId);
            AddDomainEvent(new DepartmentRemovedDomainEvent(department));
            _departments.Remove(department);
        }
        public void AddLocation(Location location, Guid customerId, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            AddDomainEvent(new LocationAddedToCustomerDomainEvent(location, customerId));
            Locations.Add(location);
        }

        public void RemoveLocation(Location location, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            AddDomainEvent(new LocationRemovedToCustomerDomainEvent(location));
            Locations.Remove(location);
        }

        public void ChangeDepartmentName(Department department, string name, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            department.SetUpdatedBy(callerId);
            var oldDepartmentName = department.Name;
            department.Name = name;
            AddDomainEvent(new DepartmentNameChangedDomainEvent(department, oldDepartmentName));
        }

        public void ChangeDepartmentDescription(Department department, string description, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            department.SetUpdatedBy(callerId);
            var oldDepartmentDescription = department.Description;
            department.Description = description;
            AddDomainEvent(new DepartmentDescriptionChangedDomainEvent(department, oldDepartmentDescription));
        }

        public void ChangeDepartmentCostCenterId(Department department, string costCenterId, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            department.SetUpdatedBy(callerId);
            var oldDepartmentCostCenterId = department.CostCenterId;
            department.CostCenterId = costCenterId;
            AddDomainEvent(new DepartmentCostCenterIdChangedDomainEvent(department, oldDepartmentCostCenterId));
        }

        /// <summary>
        ///     Changes the organizations partner. <para>
        ///     
        ///     When the partner is changed or assigned, <see cref="IsCustomer"/> is set to <see langword="true"/>. 
        ///     If the partner gets removed, then <see cref="IsCustomer"/> will also be set as <see langword="false"/>. </para>
        /// </summary>
        /// <param name="partner"> The new partner. </param>
        public void ChangePartner(Partner? partner)
        {
            var oldPartner = Partner;
            Partner = partner;
            // Update the customer status along with the partner assignment.
            IsCustomer = partner is not null;
            AddDomainEvent(new OrganizationPartnerChangedDomainEvent(this, oldPartner?.ExternalId));
        }

        public void ChangeDepartmentsParentDepartment(Department department, Department parentDepartment, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            department.SetUpdatedBy(callerId);
            var oldParentDepartmentId = department.ParentDepartment?.ExternalDepartmentId;
            department.ParentDepartment = parentDepartment;
            AddDomainEvent(new DepartmentParentDepartmentChangedDomainEvent(department, oldParentDepartmentId));
        }
        public void AddDepartmentManager(Department department, User manager, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            department.AddDepartmentManager(manager, callerId);
            AddDomainEvent(new DepartmentAddDepartmentManagerDomainEvent(department, manager, callerId));
        }
        public void UpdateDepartmentManagers(Department department, IList<User> managers, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            department.UpdateDepartmentManagers(managers, callerId);
            AddDomainEvent(new DepartmentUpdateDepartmentManagersDomainEvent(department, callerId));
        }
        public void RemoveDepartmentManagers(Department department, IList<User> managers, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            department.RemoveDepartmentManagers(managers,callerId);
            AddDomainEvent(new DepartmentRemoveDepartmentManagersDomainEvent(department, callerId));
        }
        public void InitiateOnboarding()
        {
            _customerStatus = CustomerStatus.StartedOnboarding;
            AddDomainEvent(new CustomerStartedOnboardingDomainEvent(this));
        }
        /// <summary>
        /// If customer has Techstep as partner, then all customer information should not be allowed to be changed directly through Origo. 
        /// It needs to come from Techstep core's master date, so that there is a single point of truth. 
        /// This will be the customer id used accross all techstep applications 
        /// </summary>
        public void AddTechstepCustomerId(long techstepCusomerId)
        {
            TechstepCustomerId = techstepCusomerId;
            AddDomainEvent(new CustomerAddedTechstepCustomerIdDomainEvent(this));
        }
        public void AddTechstepAccountOwner(string accountOwner)
        {
            AccountOwner = accountOwner;
            AddDomainEvent(new CustomerAddedAccountOwnerDomainEvent(this));
        }
        public void UpdateOrganizationName(string name)
        {
            var oldName = Name;
            Name = name;
            AddDomainEvent(new CustomerNameChangedDomainEvent(this,oldName));
        }
        public void UpdateOrganizationNumber(string organizationNumber)
        {
            var oldOrganizationNumber = OrganizationNumber;
            OrganizationNumber = organizationNumber;
            AddDomainEvent(new OrganizationNumberChangedDomainEvent(this, oldOrganizationNumber));
        }
        public void UpdateTechstepAccountOwner(string accountOwner)
        {
            var oldAccountOwner = AccountOwner;
            AccountOwner = accountOwner;
            AddDomainEvent(new OrganizationUpdateAccountOwner(this, oldAccountOwner));
        }
    }
}
