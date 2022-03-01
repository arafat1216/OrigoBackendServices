using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionManagementServices.ServiceModels
{
    public record NewOperatorList
    {
        public List<int> Operators { get; set; }
        public Guid CallerId { get; set; }
    }
}
