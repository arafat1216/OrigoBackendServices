namespace OrigoApiGateway.Models.BackendDTO;

public class UserPermissionsDTO
{
    public string Role { get; init; }

    public IList<string> PermissionNames { get; init; }

    public IList<Guid> AccessList { get; init; }

    public Guid UserId { get; init; }
    
    public Guid MainOrganizationId { get; set; }
}