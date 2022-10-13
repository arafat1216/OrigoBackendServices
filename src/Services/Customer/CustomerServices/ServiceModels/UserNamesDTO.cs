#nullable enable
using System;

namespace CustomerServices.ServiceModels;

public class UserNamesDTO
{
    public Guid UserId { get; init; }
    public string UserName { get; init; }
}