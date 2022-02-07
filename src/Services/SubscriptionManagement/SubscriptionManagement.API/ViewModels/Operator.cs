namespace SubscriptionManagement.API.ViewModels
{
    public record Operator
    {
        public Operator(SubscriptionManagementServices.Models.Operator @operator)
        {
            OperatorName = @operator.OperatorName;
            Country = @operator.Country;
        }
        public string OperatorName { get; set; }
        public string Country { get; set; }

        public DateTime CreatedDate { get; protected set; }
        public Guid CreatedBy { get; protected set; }

    }
}
