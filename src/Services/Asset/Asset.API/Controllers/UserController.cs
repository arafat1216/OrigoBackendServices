using System;
using Dapr;
using Microsoft.AspNetCore.Mvc;

namespace Asset.API.Controllers;

public class UserController : Controller
{
    [Topic("customer-pub-sub", "user-deleted")]
    [HttpPost("checkout")]
    public void UserDeleted([FromBody] UserDeletedEvent userDeletedEvent)
    {
        Console.WriteLine("Delete user event received : " + userDeletedEvent.UserId);
    }
}

public record UserDeletedEvent
{
    public Guid CustomerId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedDate { get; set; }
}