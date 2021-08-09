using Common.Seedwork;
using CustomerServices.DomainEvents;
using System;
using System.Collections.Generic;

namespace CustomerServices.Models
{
    public class ProductModuleGroup : Entity
    {
        protected ProductModuleGroup() { }

        public ProductModuleGroup(Guid productModuleGroupId, string name)
        {
            ProductModuleGroupId = productModuleGroupId;
            Name = name;
        }

        public Guid ProductModuleGroupId { get; protected set; }

        public string Name { get; protected set; }

        public ICollection<Customer> Customers { get; protected set; }

        public void LogAddModuleGroup()
        {
            AddDomainEvent(new ProductModuleGroupAddedDomainEvent(this));

        }

        public void LogRemoveModuleGroup()
        {
            AddDomainEvent(new ProductModuleGroupRemovedDomainEvent(this));

        }
    }
}