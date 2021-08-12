﻿using Common.Seedwork;
using System;
using System.Collections.Generic;

namespace CustomerServices.Models
{
    public class ProductModuleGroup : Entity
    {
        protected ProductModuleGroup() { }

        public ProductModuleGroup(Guid productModuleGroupId, Guid productModuleId, string name)
        {
            ProductModuleGroupId = productModuleGroupId;
            ProductModuleExternalId = productModuleId;
            Name = name;
        }

        public Guid ProductModuleGroupId { get; protected set; }

        public Guid ProductModuleExternalId { get; protected set; }

        public string Name { get; protected set; }

        public ICollection<Customer> Customers { get; protected set; }
    }
}