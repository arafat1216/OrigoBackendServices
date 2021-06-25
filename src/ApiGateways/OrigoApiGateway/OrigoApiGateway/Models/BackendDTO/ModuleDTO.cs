using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class ModuleDTO
    {
        public Guid ProductModuleId { get; set; }

        public string Name { get; set; }

        public IList<ModuleGroupDTO> ProductModuleGroup { get; set; }
    }
}
