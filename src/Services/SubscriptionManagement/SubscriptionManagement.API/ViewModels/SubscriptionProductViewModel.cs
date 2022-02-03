using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagement.API.ViewModels
{
    public record SubscriptionProductViewModel 
    {
        public string SubscriptionName { get; set; }
        public OperatorDTO Operator { get; set; }
        public IList<DatapackageDTO> Datapackages { get; set; }


    }
}
