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
    public class Customer : Entity, IAggregateRoot
    {
        private IList<AssetCategoryType> selectedAssetCategories;
        private ICollection<ProductModule> selectedProductModules;
        private ICollection<ProductModuleGroup> selectedProductModuleGroups;
        private IList<User> users;
        public Guid CustomerId { get; protected set; }

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

        protected Customer()
        {

        }

        public Customer(Guid customerId, string companyName, string orgNumber, Address companyAddress,
            ContactPerson customerContactPerson)
        {
            CompanyName = companyName;
            OrgNumber = orgNumber;
            CompanyAddress = companyAddress;
            CustomerContactPerson = customerContactPerson;
            CustomerId = customerId;
            AddDomainEvent(new CustomerCreatedDomainEvent(this));
        }

        public void AddAssetCategory(AssetCategoryType assetCategory)
        {
            selectedAssetCategories.Add(assetCategory);
        }

        public void RemoveAssetCategory(AssetCategoryType assetCategory)
        {
            AddDomainEvent(new AssetCategoryRemovedDomainEvent(assetCategory));
            selectedAssetCategories.Remove(assetCategory);
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
            AddDomainEvent(new ProductModuleAddedDomainEvent(CustomerId, productModule));
            selectedProductModules.Add(productModule);
        }
        public void RemoveProductModule(ProductModule productModule)
        {
            AddDomainEvent(new ProductModuleRemovedDomainEvent(CustomerId, productModule));
            selectedProductModules.Remove(productModule);
        }

        public void AddProductModuleGroup(ProductModuleGroup productModuleGroup)
        {
            AddDomainEvent(new ProductModuleGroupAddedDomainEvent(CustomerId, productModuleGroup));
            selectedProductModuleGroups.Add(productModuleGroup);
        }

        public void RemoveProductModuleGroup(ProductModuleGroup productModuleGroup)
        {
            AddDomainEvent(new ProductModuleGroupRemovedDomainEvent(CustomerId, productModuleGroup));
            selectedProductModuleGroups.Remove(productModuleGroup);
        }
    }
}
