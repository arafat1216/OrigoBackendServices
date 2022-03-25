using System;

namespace AssetServices.ServiceModel;

public record UserDTO
{
    public UserDTO(Guid externalId, string name)
    {
        ExternalId = externalId;
        Name = name;
    }

    public Guid ExternalId { get; }
    public string Name { get; }
}