#nullable enable

namespace OrigoApiGateway.Models.BackendDTO;

public class UserNamesDTO : Object
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;

    public override bool Equals(object? obj)
    {
        if (obj is not UserNamesDTO comp) return false;
        return UserId == comp.UserId;
    }

    public override int GetHashCode()
    {
        return UserId.GetHashCode();
    }
}