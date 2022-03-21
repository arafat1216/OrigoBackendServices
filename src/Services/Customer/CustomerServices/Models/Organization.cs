using Common.Seedwork;
using CustomerServices.DomainEvents;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace CustomerServices.Models
{
    public class Organization : Entity, IAggregateRoot
    {
        private IList<User> users;

        public Guid OrganizationId { get; protected set; }
        public Guid? ParentId { get; protected set; }
        public Guid? PrimaryLocation { get; protected set; }

        public string Name { get; protected set; }

        public string OrganizationNumber { get; protected set; }

        public Address Address { get; protected set; }

        public ContactPerson ContactPerson { get; protected set; }
        
        public bool IsCustomer { get; protected set; }

        [NotMapped]
        public ICollection<Organization> ChildOrganizations { get; set; }
        [NotMapped]
        public OrganizationPreferences Preferences { get; set; }

        [NotMapped]
        public Location Location { get; set; }

        [JsonIgnore]
        public IList<User> Users
        {
            get { return users?.ToImmutableList(); }
            protected set { users = value; }
        }

        [JsonIgnore]
        public ICollection<Department> Departments { get; protected set; }


        protected Organization()
        {

        }

        public Organization(Guid organizationId, Guid callerId, Guid? parentId, string companyName, string orgNumber, Address companyAddress,
            ContactPerson organizationContactPerson, OrganizationPreferences organizationPreferences, Location organizationLocation, bool isCustomer)
        {
            Name = companyName;
            ParentId = parentId;
            OrganizationNumber = orgNumber;
            Address = companyAddress;
            ContactPerson = organizationContactPerson;
            OrganizationId = organizationId;
            Preferences = organizationPreferences;
            Location = organizationLocation;
            PrimaryLocation = (organizationLocation == null) ? Guid.Empty : organizationLocation.LocationId;
            CreatedBy = callerId;
            UpdatedBy = callerId;
            IsDeleted = false;
            IsCustomer = isCustomer;
            AddDomainEvent(new CustomerCreatedDomainEvent(this));
        }

        public void UpdateOrganization(Organization organization)
        {
            bool isUpdated = false;
            if (ParentId != organization.ParentId)
            {
                var previousparentId = ParentId.ToString();
                ParentId = organization.ParentId;
                AddDomainEvent(new CustomerParentIdChangedDomainEvent(this, previousparentId));
                isUpdated = true;
            }

            if (PrimaryLocation != organization.PrimaryLocation)
            {
                var previousPrimaryLocation = PrimaryLocation.ToString();
                PrimaryLocation = (organization.PrimaryLocation == null) ? Guid.Empty : organization.PrimaryLocation;
                AddDomainEvent(new CustomerPrimaryLocationChangedDomainEvent(this, previousPrimaryLocation));
                isUpdated = true;
            }
            
            if (Name != organization.Name)
            {
                var oldName = Name;
                Name = (organization.Name == null) ? "" : organization.Name;
                AddDomainEvent(new CustomerNameChangedDomainEvent(this, oldName));
                isUpdated = true;
            }
            
            if (OrganizationNumber != organization.OrganizationNumber)
            {
                var oldNumber = OrganizationNumber;
                OrganizationNumber = (organization.OrganizationNumber == null) ? "" : organization.OrganizationNumber;
                AddDomainEvent(new OrganizationNumberChangedDomainEvent(this, oldNumber));
                isUpdated = true;
            }

            if (Address != organization.Address)
            {
                var oldAddress = Address;
                Address = organization.Address;
                AddDomainEvent(new CustomerAddressChangedDomainEvent(this, oldAddress));
                isUpdated = true;
            }
            
            if (ContactPerson != organization.ContactPerson)
            {
                var oldContactPerson = ContactPerson;
                ContactPerson = organization.ContactPerson;
                AddDomainEvent(new ContactPersonChangedDomainEvent(this, oldContactPerson));
                isUpdated = true;
            }

            
            Preferences = organization.Preferences; // preferences cannot be changed here
            Location = organization.Location; // Is either a new empty location object, or an existing one. Not modified.
            
            if (isUpdated)
            {
                LastUpdatedDate = DateTime.UtcNow;
                UpdatedBy = organization.CreatedBy;
            }
        }

        public void PatchOrganization(Organization organization)
        {
            bool isUpdated = false;
            if (ParentId != organization.ParentId && organization.ParentId != null)
            {
                var previousparentId = ParentId.ToString();
                ParentId = organization.ParentId;
                AddDomainEvent(new CustomerParentIdChangedDomainEvent(this, previousparentId));
                isUpdated = true;
            }

            if (PrimaryLocation != organization.PrimaryLocation && organization.PrimaryLocation != null)
            {
                var previousPrimaryLocation = PrimaryLocation.ToString();
                PrimaryLocation = organization.PrimaryLocation;
                AddDomainEvent(new CustomerPrimaryLocationChangedDomainEvent(this, previousPrimaryLocation));
                isUpdated = true;
            }

            if (Name != organization.Name && organization.Name != null)
            {
                var oldName = Name;
                Name = organization.Name;
                AddDomainEvent(new CustomerNameChangedDomainEvent(this, oldName));
                isUpdated = true;
            }

            if (OrganizationNumber != organization.OrganizationNumber && organization.OrganizationNumber != null)
            {
                var oldNumber = OrganizationNumber;
                OrganizationNumber = organization.OrganizationNumber;
                AddDomainEvent(new OrganizationNumberChangedDomainEvent(this, oldNumber));
                isUpdated = true;
            }

            if (Address != organization.Address && organization.Address != null)
            {
                var oldAddress = Address;
                Address = organization.Address;
                AddDomainEvent(new CustomerAddressChangedDomainEvent(this, oldAddress));
                isUpdated = true;
            }

            if (ContactPerson != organization.ContactPerson && organization.ContactPerson != null)
            {
                var oldContactPerson = ContactPerson;
                ContactPerson = organization.ContactPerson;
                AddDomainEvent(new ContactPersonChangedDomainEvent(this, oldContactPerson));
                isUpdated = true;
            }

            if (isUpdated)
            {
                LastUpdatedDate = DateTime.UtcNow;
                UpdatedBy = organization.CreatedBy;
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
            Departments.Add(department);
        }

        public void RemoveDepartment(Department department, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate= DateTime.UtcNow;
            department.SetDeletedBy(callerId);
            AddDomainEvent(new DepartmentRemovedDomainEvent(department));
            Departments.Remove(department);
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
            LastUpdatedDate= DateTime.UtcNow;
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

        public void ChangeDepartmentsParentDepartment(Department department, Department parentDepartment, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            department.SetUpdatedBy(callerId);
            var oldParentDepartmentId = department.ParentDepartment?.ExternalDepartmentId;
            department.ParentDepartment = parentDepartment;
            AddDomainEvent(new DepartmentParentDepartmentChangedDomainEvent(department, oldParentDepartmentId));
        }
    }
}
