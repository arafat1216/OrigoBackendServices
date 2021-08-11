using Common.Seedwork;
using CustomerServices.DomainEvents;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CustomerServices.Models
{
    public class ProductModule : Entity, IAggregateRoot
    {
        protected ProductModule() { }

        public ProductModule(Guid productModuleId, string name, IList<ProductModuleGroup> productModuleGroups)
        {
            ProductModuleId = productModuleId;
            Name = name;
            ProductModuleGroup = productModuleGroups;
        }
        public Guid ProductModuleId { get; protected set; }

        public string Name { get; protected set; }

        public IList<ProductModuleGroup> ProductModuleGroup { get; protected set; }

        [JsonIgnore]
        public ICollection<Customer> Customers { get; protected set; }

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
