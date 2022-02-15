namespace SubscriptionManagement.API.ViewModels
{
    public record Operator
    {
        public Operator(SubscriptionManagementServices.Models.Operator @operator)
        {
            Name = @operator.OperatorName;
            Country = @operator.Country;
            Id = @operator.Id;
        }
        /// <summary>
        /// Operator's name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Operator's name
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// Operator's identifer
        /// </summary>
        public int Id { get; set; }
    }
}
