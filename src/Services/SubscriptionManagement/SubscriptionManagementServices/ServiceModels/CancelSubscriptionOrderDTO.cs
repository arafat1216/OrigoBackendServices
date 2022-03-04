namespace SubscriptionManagementServices.ServiceModels;

public record CancelSubscriptionOrderDTO
{
    public Guid SubscriptionOrderId { get; set; }
    public string MobileNumber { get; set; }
    public int OperatorId { get; set; }
    public DateTime DateOfTermination { get; set; }
}