using System;

namespace Asset.API.Controllers;

public record UserChangedDepartmentEvent
{
    public Guid CustomerId { get; set; }
    public Guid UserId { get; set; }
    public Guid DepartmentId { get; set; }
    public DateTime CreatedDate { get; set; }
}