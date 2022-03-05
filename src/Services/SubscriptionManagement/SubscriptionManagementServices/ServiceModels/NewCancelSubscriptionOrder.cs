namespace SubscriptionManagementServices.ServiceModels;

public record NewCancelSubscriptionOrder
{
    public string MobileNumber { get; set; }
    public int OperatorId { get; set; }
    public DateTime DateOfTermination { get; set; }
    public Guid CallerId { get; set; }
}