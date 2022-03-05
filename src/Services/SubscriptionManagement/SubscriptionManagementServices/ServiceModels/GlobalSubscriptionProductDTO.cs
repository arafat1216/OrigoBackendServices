namespace SubscriptionManagementServices.ServiceModels
{
    public class GlobalSubscriptionProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OperatorId { get; set; }
        public IList<string> Datapackages { get; set; }
        public bool isGlobal { get; set; } = true;
    }
}
