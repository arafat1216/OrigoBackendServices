﻿using Common.Seedwork;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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

        [JsonIgnore]
        public ICollection<Organization> Customers { get; protected set; }
        public void SetDeletedBy(Guid callerId) 
        {
            DeletedBy = callerId;
        }
        public void SetCreatedBy(Guid callerId)
        {
            CreatedBy = callerId;
        }
    }
}