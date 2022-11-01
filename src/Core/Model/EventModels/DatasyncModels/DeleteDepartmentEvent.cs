namespace Common.Model.EventModels.DatasyncModels;

public record DeleteDepartmentEvent
{
    /// <summary>
    /// The external id of the customer
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// The external id of the department
    /// </summary>
    public Guid DepartmentId { get; set; }

    /// <summary>
    /// This might get obsolete in future
    /// </summary>
    public Guid CallerId { get; set; }
}