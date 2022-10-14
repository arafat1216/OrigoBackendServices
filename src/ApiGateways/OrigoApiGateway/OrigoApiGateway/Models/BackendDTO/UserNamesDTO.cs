namespace OrigoApiGateway.Models.BackendDTO;

public class UserNamesDTO : IComparable<UserNamesDTO>
{
    public Guid UserId { get; init; }
    public string UserName { get; init; }
    
    public int CompareTo(UserNamesDTO other)
    {
        return UserId.CompareTo(other.UserId);
    }
}