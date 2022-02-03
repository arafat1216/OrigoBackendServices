using SubscriptionManagementServices.Models;

namespace SubscriptionManagement.API.ViewModels
{
    public record OperatorViewModel
    {

        public string OperatorName { get; set; }
        public string Country { get; set; }

        public DateTime CreatedDate { get; protected set; }
        public Guid CreatedBy { get; protected set; }

    }
}
