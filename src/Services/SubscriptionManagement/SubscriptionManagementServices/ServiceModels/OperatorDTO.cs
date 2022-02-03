using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionManagementServices.ServiceModels
{
    public record OperatorDTO
    {
        public string OperatorName { get; set;}
        public string Country { get; set;}
    }
}
