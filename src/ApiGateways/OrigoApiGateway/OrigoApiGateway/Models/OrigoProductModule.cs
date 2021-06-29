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

        public IList<OrigoProductModuleGroup> ProductModuleGroup { get; set; }
    }
}
