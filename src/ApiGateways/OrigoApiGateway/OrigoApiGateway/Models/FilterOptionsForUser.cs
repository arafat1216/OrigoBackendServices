#nullable enable
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    public class FilterOptionsForUser
    {
        [FromQuery(Name = "role")]
        public string[]? Roles { get; set; }

        [FromQuery(Name = "assignedToDepartment")]
        public IList<Guid>? AssignedToDepartments { get; set; }

        [FromQuery(Name = "userStatus")]
        public IList<int>? UserStatuses { get; set; }

        [FromQuery(Name = "partnerId")]
        public Guid? PartnerId { get; set; }
    }
}
