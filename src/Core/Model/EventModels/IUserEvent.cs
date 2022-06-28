namespace Common.Model.EventModels;

/// <summary>
/// Common interface for user events.
/// </summary>
public interface IUserEvent
{
    /// <summary>
    /// The id of the customer linked with the user
    /// </summary>
    Guid CustomerId { get; set; }

    /// <summary>
    /// The unique id of the user
    /// </summary>
    Guid UserId { get; set; }

    /// <summary>
    /// The department which will take responsibility for entities previously linked to the user.
    /// </summary>
    Guid? DepartmentId { get; set; }

    /// <summary>
    /// Event published date
    /// </summary>
    DateTime CreatedDate { get; set; }
}