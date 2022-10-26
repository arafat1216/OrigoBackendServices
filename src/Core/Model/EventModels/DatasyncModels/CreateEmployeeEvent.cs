using System.ComponentModel.DataAnnotations;

namespace Common.Model.EventModels.DatasyncModels;

/// <summary>
/// Event message published when a new employee end user is created 
/// </summary>
public record CreateEmployeeEvent
{

    /// <summary>
    /// The external id of the customer
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Employee's given name
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Employee's family name
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Employee's email address
    /// </summary>
    [EmailAddress]
    [MaxLength(320)]
    public string? Email { get; set; }

    /// <summary>
    /// Employee's mobile phone number
    /// </summary>
    [Phone]
    [MaxLength(15)]
    public string? MobileNumber { get; set; }
}
