#nullable enable
namespace CustomerServices.ServiceModels;

public record UserNamesDTO
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
}