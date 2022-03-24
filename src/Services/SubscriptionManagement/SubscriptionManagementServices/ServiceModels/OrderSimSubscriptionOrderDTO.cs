namespace SubscriptionManagementServices.ServiceModels;

public record OrderSimSubscriptionOrderDTO
{
    public OrderSimSubscriptionOrderDTO()
    {
    }

    public OrderSimSubscriptionOrderDTO(OrderSimSubscriptionOrderDTO DTO)
    {
        SendToName = DTO.SendToName;
        Address = DTO.Address;
        OperatorName = DTO.OperatorName;
        Quantity = DTO.Quantity;
        CallerId = DTO.CallerId;
    }
    /// <summary>
    /// The recipient name of the sim card.
    /// </summary>
    public string SendToName { get; set; }
    /// <summary>
    /// Send to either private or business address
    /// </summary>
    public Address Address { get; set; }

    public string OperatorName { get; set; }
    public int Quantity { get; set; }
    public Guid CallerId { get; set; }
}