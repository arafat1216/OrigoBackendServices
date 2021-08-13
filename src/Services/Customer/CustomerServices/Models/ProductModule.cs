using Common.Seedwork;
using CustomerServices.DomainEvents;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace CustomerServices.Models
{
    public class ProductModule : Entity, IAggregateRoot
    {
        private IList<ProductModuleGroup> productModuleGroups;

        protected ProductModule() { }

        public ProductModule(Guid productModuleId, string name, IList<ProductModuleGroup> productModuleGroups)
        {
            ProductModuleId = productModuleId;
            Name = name;
            ProductModuleGroup = productModuleGroups;
        }

        public Guid ProductModuleId { get; protected set; }

        public string Name { get; protected set; }

        public IList<ProductModuleGroup> ProductModuleGroup
        {
            get { return productModuleGroups; }
            protected set { productModuleGroups = value; }
        }

        [JsonIgnore]
        public ICollection<Customer> Customers { get; protected set; }

        public void AddProductModuleGroup(ProductModuleGroup productModuleGroup)
        {
            productModuleGroups.Add(productModuleGroup);
            productModuleGroup.LogAddModuleGroup();
        }

        public void LogAddProductModule()
        {
            AddDomainEvent(new ProductModuleAddedDomainEvent(this));
        }

        public void LogRemoveProductModule()
        {
            AddDomainEvent(new ProductModuleRemovedDomainEvent(this));
        }
    }
}
