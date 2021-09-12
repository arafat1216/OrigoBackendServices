using Common.Seedwork;
using CustomerServices.DomainEvents;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json.Serialization;
using System.Linq;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace CustomerServices.Models
{
    public class Organization : Entity, IAggregateRoot
    {
        private IList<AssetCategoryType> selectedAssetCategories;
        private ICollection<ProductModule> selectedProductModules;
        private ICollection<ProductModuleGroup> selectedProductModuleGroups;
        private IList<User> users;
        public Guid OrganizationId { get; protected set; }

        public string CompanyName { get; protected set; }

        public string OrgNumber { get; protected set; }

        public Address CompanyAddress { get; protected set; }

        public ContactPerson CustomerContactPerson { get; protected set; }

        [JsonIgnore]
        public IList<User> Users
        {
            get { return users?.ToImmutableList(); }
            protected set { users = value; }
        }

        [JsonIgnore]
        public ICollection<Department> Departments { get; protected set; }

        [JsonIgnore]
        public ICollection<ProductModule> SelectedProductModules
        {
            get { return selectedProductModules?.ToImmutableList(); }
            protected set { selectedProductModules = value.ToList(); }
        }

        [JsonIgnore]
        public ICollection<ProductModuleGroup> SelectedProductModuleGroups
        {
            get { return selectedProductModuleGroups?.ToImmutableList(); }
            protected set { selectedProductModuleGroups = value.ToList(); }
        }

        [JsonIgnore]
        public ICollection<AssetCategoryType> SelectedAssetCategories
        {
            get { return selectedAssetCategories?.ToImmutableList(); }
            protected set { selectedAssetCategories = value.ToList(); }
        }

        protected Organization()
        {

        }

        public Organization(Guid customerId, string companyName, string orgNumber, Address companyAddress,
            ContactPerson customerContactPerson)
        {
            CompanyName = companyName;
            OrgNumber = orgNumber;
            CompanyAddress = companyAddress;
            CustomerContactPerson = customerContactPerson;
            OrganizationId = customerId;
            AddDomainEvent(new CustomerCreatedDomainEvent(this));
        }

        public void AddAssetCategory(AssetCategoryType assetCategory)
        {
            AddDomainEvent(new AssetCategoryAddedDomainEvent(assetCategory));
            selectedAssetCategories.Add(assetCategory);
        }

        public void RemoveAssetCategory(AssetCategoryType assetCategory)
        {
            AddDomainEvent(new AssetCategoryRemovedDomainEvent(assetCategory));
            selectedAssetCategories.Remove(assetCategory);
        }

        public void AddLifecyle(AssetCategoryType assetCategory, AssetCategoryLifecycleType lifecycleType)
        {
            AddDomainEvent(new AssetLifecycleSettingAddedDomainEvent(lifecycleType));
            assetCategory.LifecycleTypes.Add(lifecycleType);
        }

        public void RemoveLifecyle(AssetCategoryType assetCategory, AssetCategoryLifecycleType lifecycleType)
        {
            try
            {
                AddDomainEvent(new AssetLifecycleSettingRemovedDomainEvent(lifecycleType));
                assetCategory.LifecycleTypes.Remove(lifecycleType);
            }
            catch
            {
                // Item may already be removed
            }
        }

        public void AddProductModule(ProductModule productModule)
        {
            AddDomainEvent(new ProductModuleAddedDomainEvent(OrganizationId, productModule));
            selectedProductModules.Add(productModule);
        }

        public void RemoveProductModule(ProductModule productModule)
        {
            AddDomainEvent(new ProductModuleRemovedDomainEvent(OrganizationId, productModule));
            selectedProductModules.Remove(productModule);
        }

        public void AddProductModuleGroup(ProductModuleGroup productModuleGroup)
        {
            AddDomainEvent(new ProductModuleGroupAddedDomainEvent(OrganizationId, productModuleGroup));
            selectedProductModuleGroups.Add(productModuleGroup);
        }

        public void RemoveProductModuleGroup(ProductModuleGroup productModuleGroup)
        {
            AddDomainEvent(new ProductModuleGroupRemovedDomainEvent(OrganizationId, productModuleGroup));
            selectedProductModuleGroups.Remove(productModuleGroup);
        }

        public void AddDepartment(Department department)
        {
            AddDomainEvent(new DepartmentAddedDomainEvent(department));
            Departments.Add(department);
        }

        public void RemoveDepartment(Department department)
        {
            AddDomainEvent(new DepartmentRemovedDomainEvent(department));
            Departments.Remove(department);
        }

        public void ChangeDepartmentName(Department department, string name)
        {
            var oldDepartmentName = department.Name;
            department.Name = name;
            AddDomainEvent(new DepartmentNameChangedDomainEvent(department, oldDepartmentName));
        }

        public void ChangeDepartmentDescription(Department department, string description)
        {
            var oldDepartmentDescription = department.Description;
            department.Description = description;
            AddDomainEvent(new DepartmentDescriptionChangedDomainEvent(department, oldDepartmentDescription));
        }

        public void ChangeDepartmentCostCenterId(Department department, string costCenterId)
        {
            var oldDepartmentCostCenterId = department.CostCenterId;
            department.CostCenterId = costCenterId;
            AddDomainEvent(new DepartmentCostCenterIdChangedDomainEvent(department, oldDepartmentCostCenterId));
        }

        public void ChangeDepartmentsParentDepartment(Department department, Department parentDepartment)
        {
            var oldParentDepartmentId = department.ParentDepartment?.ExternalDepartmentId;
            department.ParentDepartment = parentDepartment;
            AddDomainEvent(new DepartmentParentDepartmentChangedDomainEvent(department, oldParentDepartmentId));
        }
    }
}
