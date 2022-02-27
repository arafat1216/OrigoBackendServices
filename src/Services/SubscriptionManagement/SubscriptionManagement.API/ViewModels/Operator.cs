namespace SubscriptionManagement.API.ViewModels
{
    public record Operator
    {
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
