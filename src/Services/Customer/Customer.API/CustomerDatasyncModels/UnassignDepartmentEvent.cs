namespace Customer.API.CustomerDatasyncModels;

public record UnassignDepartmentEvent
{
    /// <summary>
    /// The external id of the customer
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Department Id
    /// </summary>
    public Guid DepartmentId { get; set; }

    /// <summary>
    /// The external id of the User
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// This might get obsolete in future
    /// </summary>
    public Guid CallerId { get; set; }
}