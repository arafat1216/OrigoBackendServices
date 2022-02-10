namespace SubscriptionManagement.API.ViewModels
{
    public record Operator
    {
        public Operator(SubscriptionManagementServices.Models.Operator @operator)
        {
            Name = @operator.OperatorName;
            Country = @operator.Country;
        }
        public string Name { get; set; }
        public string Country { get; set; }

    }
}
