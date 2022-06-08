using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    public class FilterOptionsForUser
    {
        [FromQuery(Name = "role")]
        public string[]? Role { get; set; }

        [FromQuery(Name = "assignedToDepartment")]
        public Guid[]? AssignedToDepartment { get; set; }

        [FromQuery(Name = "userStatus")]
        public IList<int>? UserStatus { get; set; }
    }
}
