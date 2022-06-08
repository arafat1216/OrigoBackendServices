using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Customer.API.ViewModels
{
    public class FilterOptionsForUser
    {
        [FromQuery(Name = "role")]
        public string[]? Roles { get; set; }

        [FromQuery(Name = "assignedToDepartment")]
        public Guid[]? AssignedToDepartments { get; set; }

        [FromQuery(Name = "userStatus")]
        public IList<int>? UserStatuses { get; set; }
    }
}
