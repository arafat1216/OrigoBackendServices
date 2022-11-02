#nullable enable
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    public class FilterOptionsForUser
    {
        /// <summary>
        /// Users role.
        /// </summary>
        [FromQuery(Name = "role")]
        public string[]? Roles { get; set; }
        /// <summary>
        /// Departments assigned to user.
        /// </summary>
        [FromQuery(Name = "assignedToDepartment")]
        public IList<Guid>? AssignedToDepartments { get; set; }
        /// <summary>
        /// User status to filter on.
        /// </summary>
        [FromQuery(Name = "userStatus")]
        public IList<int>? UserStatuses { get; set; }
        /// <summary>
        /// Partner Id.
        /// </summary>
        [FromQuery(Name = "partnerId")]
        public Guid? PartnerId { get; set; }
    }
}
