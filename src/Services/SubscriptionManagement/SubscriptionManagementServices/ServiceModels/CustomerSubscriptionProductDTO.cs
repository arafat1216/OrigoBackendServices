namespace SubscriptionManagementServices.ServiceModels;

public class CustomerSubscriptionProductDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int OperatorId { get; set; }
    public IList<string> DataPackages { get; set; }
    public bool IsGlobal { get; set; }
}