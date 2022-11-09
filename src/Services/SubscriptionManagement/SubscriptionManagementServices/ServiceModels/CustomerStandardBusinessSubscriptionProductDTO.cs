using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionManagementServices.ServiceModels
{
    public class CustomerStandardBusinessSubscriptionProductDTO
    {
        [Required]
        public string OperatorName { get; set; } = string.Empty;
        public int OperatorId { get; set; }
        [Required]
        public string SubscriptionName { get; set; } = string.Empty;
        public string? DataPackage { get; set; }
        public IList<string>? AddOnProducts { get; set; }
    }
}
