using Common.Seedwork;
using System;
using System.Collections.Generic;

namespace CustomerServices.Models
{
    public class ProductModule : Entity, IAggregateRoot
    {
        public Guid ProductModuleId { get; protected set; }

        public string Name { get; protected set; }

        public IList<ProductModuleGroup> ProductModuleGroup { get; protected set; }

        public ICollection<Customer> Customers { get; protected set; }
    }
}
