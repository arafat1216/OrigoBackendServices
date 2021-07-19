using OrigoApiGateway.Models.BackendDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models
{
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
