namespace Common.Model.EventModels;

/// <summary>
/// Event published when a user is deleted.
/// </summary>
public record UserDeletedEvent
{
    /// <summary>
    /// The id of the customer linked with the user
    /// </summary>
    public Guid CustomerId { get; set; }
    /// <summary>
    /// The unique id of the user
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// The department which will take responsibility for entities previously linked to the user.
    /// </summary>
    public Guid? DepartmentId { get; set; }
    /// <summary>
    /// Event published date
    /// </summary>
    public DateTime CreatedDate { get; set; }
}