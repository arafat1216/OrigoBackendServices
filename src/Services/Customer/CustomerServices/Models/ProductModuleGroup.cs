using Common.Seedwork;
using System;
using System.Collections.Generic;

namespace CustomerServices.Models
{
    public class ProductModuleGroup : Entity
    {
        public Guid ProductModuleGroupId { get; protected set; }

        public string Name { get; protected set; }

        public ICollection<Customer> Customers { get; protected set; }
    }
}