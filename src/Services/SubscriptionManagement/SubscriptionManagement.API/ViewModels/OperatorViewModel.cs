using SubscriptionManagementServices.Models;

namespace SubscriptionManagement.API.ViewModels
{
    public record OperatorViewModel
    {
        public OperatorViewModel()
        {
        }

        public OperatorViewModel(SubscriptionManagementServices.Models.Operator operatorObject)
        {
            OperatorName = operatorObject.OperatorName;
            Country = operatorObject.Country;
            CreatedBy = operatorObject.CreatedBy;
            CreatedDate =  operatorObject.CreatedDate;
        }

        public string OperatorName { get; set; }
        public string Country { get; set; }

        public DateTime CreatedDate { get; protected set; }
        public Guid CreatedBy { get; protected set; }

    }
}
