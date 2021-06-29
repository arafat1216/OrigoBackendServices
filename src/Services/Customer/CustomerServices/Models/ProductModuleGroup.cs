using Common.Seedwork;
using System;
using System.Collections.Generic;

namespace CustomerServices.Models
{
    public class ProductModuleGroup : Entity
    {
        public Guid ProductModuleGroupId { get; set; }

        public string Name { get; set; }

        public ICollection<Customer> Customers { get; set; }
    }
}