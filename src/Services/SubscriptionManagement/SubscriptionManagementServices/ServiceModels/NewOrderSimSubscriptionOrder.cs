namespace SubscriptionManagementServices.ServiceModels;

public record NewOrderSimSubscriptionOrder
{
    /// <summary>
    /// The recipient name of the sim card.
    /// </summary>
    public string SendToName { get; set; }
    /// <summary>
    /// Send to either private or business address
    /// </summary>
    public Address Address { get; set; }

    public int OperatorId { get; set; }
    public int Quantity { get; set; }
    public Guid CallerId { get; set; }
}