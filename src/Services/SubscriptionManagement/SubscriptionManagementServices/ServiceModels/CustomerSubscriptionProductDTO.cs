namespace SubscriptionManagementServices.ServiceModels;

public class CustomerSubscriptionProductDTO
{
    public int Id { get; set; }
    public string SubscriptionName { get; set; }
    public OperatorDTO Operator { get; set; }
    public IList<string> Datapackages { get; set; }
    public bool isGlobal { get; set; }
}