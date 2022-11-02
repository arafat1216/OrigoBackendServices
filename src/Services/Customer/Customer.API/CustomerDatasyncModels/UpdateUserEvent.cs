using System.ComponentModel.DataAnnotations;
using Customer.API.ViewModels;

namespace Customer.API.CustomerDatasyncModels;

public class UpdateUserEvent
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
    /// User's family name
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// User's given name
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// User's email address
    /// </summary>
    [EmailAddress]
    [MaxLength(320)]
    public string Email { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string EmployeeId { get; set; }

    /// <summary>
    /// User's Phone number
    /// </summary>
    [Phone]
    [MaxLength(15)]
    public string MobileNumber { get; set; }

    /// <summary>
    /// <see cref="UserPreference"/>
    /// </summary>
    public UserPreference UserPreference { get; set; }

    /// <summary>
    /// NB! This Will be removed in a later version
    /// </summary>
    public Guid CallerId = Guid.Empty;
}