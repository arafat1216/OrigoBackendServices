using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request;
using System;

namespace OrigoApiGateway.Models.SubscriptionManagement.Backend.Request;

public record TransferToPrivateSubscriptionOrderDTO
{
    public PrivateSubscription PrivateSubscription { get; set; }
    public string MobileNumber { get; set; }
    public string OperatorName { get; set; }
    public string NewSubscription { get; set; }
    public DateTime OrderExecutionDate { get; set; }
    public Guid CallerId { get; set; }
}