#nullable enable

namespace OrigoApiGateway.Models.BackendDTO;

public class DepartmentNamesDTO : Object
{
    public Guid DepartmentId { get; init; }
    public string DepartmentName { get; init; } = string.Empty;

    public override bool Equals(object? obj)
    {
        if (obj is not DepartmentNamesDTO comp) return false;
        return DepartmentId == comp.DepartmentId;
    }

    public override int GetHashCode()
    {
        return DepartmentId.GetHashCode();
    }
}