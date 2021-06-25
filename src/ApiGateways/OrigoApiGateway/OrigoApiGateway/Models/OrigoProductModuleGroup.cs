using OrigoApiGateway.Models.BackendDTO;
using System;

namespace OrigoApiGateway.Models
{
    public class OrigoProductModuleGroup
    {
        public OrigoProductModuleGroup(ModuleGroupDTO moduleGroup)
        {
            ProductModuleGroupId = moduleGroup.ProductModuleGroupId;
            Name = moduleGroup.Name;
        }
        public Guid ProductModuleGroupId { get; set; }

        public string Name { get; set; }
    }
}