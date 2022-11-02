namespace Customer.API.CustomerDatasyncModels;

public record DeleteUserByPhoneNumberEvent
{
    /// <summary>
    /// The external id of the customer
    /// </summary>
    public Guid CustomerId { get; set; }
    
    /// <summary>
    /// User's phone number
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// NB! This Will be removed in a later version
    /// </summary>
    public Guid CallerId = Guid.Empty;
}