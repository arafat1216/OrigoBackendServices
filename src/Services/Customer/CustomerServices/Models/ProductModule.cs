using Common.Seedwork;
using System;
using System.Collections.Generic;

namespace CustomerServices.Models
{
    public class ProductModule : Entity, IAggregateRoot
    {
        protected ProductModule() { }

        public ProductModule(Guid productModuleId, string name, IList<ProductModuleGroup> productModuleGroup)
        {
            ProductModuleId = productModuleId;
            Name = name;
            ProductModuleGroup = productModuleGroup;
        }

        public Guid ProductModuleId { get; protected set; }

        public string Name { get; protected set; }

        public IList<ProductModuleGroup> ProductModuleGroup { get; protected set; }

        public ICollection<Customer> Customers { get; protected set; }
    }
}
