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

        /// <summary>
        /// When using the GetModules endpoint in the ModuleController and a customer id is provided, 
        /// this bool will indicate if the module should be checked or not.
        /// For other endpoints this will be set to false regardless.
        /// </summary>
        public bool IsChecked { get; set; }
    }
}