namespace Customer.API.CustomerDatasyncModels;

public record DeleteUserEvent
{
    /// <summary>
    /// The external id of the customer
    /// </summary>
    public Guid CustomerId { get; set; }
    
    /// <summary>
    /// The external id of the User
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// NB! This Will be removed in a later version
    /// </summary>
    public Guid CallerId = Guid.Empty;
}