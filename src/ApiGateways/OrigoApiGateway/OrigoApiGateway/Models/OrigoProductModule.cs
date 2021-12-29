using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Response object
    /// </summary>
    public class OrigoProductModule
    {
        public Guid ProductModuleId { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// When using the GetModules endpoint in the ModuleController and a customer id is provided, 
        /// this bool will indicate if the module should be checked or not.
        /// For other endpoints this will be set to false regardless.
        /// </summary>
        public bool IsChecked { get; set; }

        public IList<OrigoProductModuleGroup> ProductModuleGroup { get; set; }
    }
}
