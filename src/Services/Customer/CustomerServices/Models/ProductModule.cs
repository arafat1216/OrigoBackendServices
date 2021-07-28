using Common.Seedwork;
using System;
using System.Collections.Generic;

namespace CustomerServices.Models
{
    public class ProductModule : Entity, IAggregateRoot
    {
        public Guid ProductModuleId { get; set; }

        public string Name { get; set; }

        public IList<ProductModuleGroup> ProductModuleGroup { get; set; }

        public ICollection<Customer> Customers { get; set; }
    }
}
