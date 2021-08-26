using Common.Seedwork;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CustomerServices.Models
{
    public class ProductModule : Entity
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
    }
}
