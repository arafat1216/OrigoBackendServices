namespace Customer.API.CustomerDatasyncModels;

public record CreateDepartmentEvent
{
    /// <summary>
    /// The external id of the customer
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Department Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string CostCenterId { get; set; }

    /// <summary>
    /// Department Description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Parent DepartmentId Id If any
    /// </summary>
    public Guid? ParentDepartmentId { get; set; }

    /// <summary>
    /// This might get obsolete in future
    /// </summary>
    public Guid CallerId { get; set; }

    /// <summary>
    /// Department Managers
    /// </summary>
    public IList<Guid> ManagedBy { get; set; } = new List<Guid>();
}