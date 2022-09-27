#nullable enable
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Customer.API.ViewModels;

/// <summary>
/// The different options users can be filtered on.
/// </summary>
public class FilterOptionsForUser
{
    /// <summary>
    /// Filter on the roles set for a user.
    /// </summary>
    [FromQuery(Name = "role")]
    public string[]? Roles { get; set; }

    /// <summary>
    /// The list of departments a user must be assigned to in order to be included.
    /// </summary>
    [FromQuery(Name = "assignedToDepartment")]
    public Guid[]? AssignedToDepartments { get; set; }

    /// <summary>
    /// Filter on the statuses for the users.
    /// </summary>
    [FromQuery(Name = "userStatus")]
    public IList<int>? UserStatuses { get; set; }

    /// <summary>
    /// Filter users on partner id.
    /// </summary>
    [FromQuery(Name = "partnerId")]
    public Guid? PartnerId { get; set; }
}