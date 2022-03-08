using System;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request;

namespace OrigoApiGateway.Models.SubscriptionManagement.Backend.Request;

public record OrderSimDTO
{
    public string SendToName { get; set; }
    /// <summary>
    /// Send to either private or business address
    /// </summary>
    public Address Address { get; set; }

    public int OperatorId { get; set; }
    public int Quantity { get; set; }
    public Guid CallerId { get; set; }
}