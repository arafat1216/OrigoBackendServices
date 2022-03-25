using System;
using System.Collections.Generic;

namespace AssetServices.ServiceModel;

/// <summary>
/// Has all properties across every asset category
/// </summary>
public record AssetDTO
{
    public Guid ExternalId { get; init; }
    public string Brand { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string SerialNumber { get; protected set; } = string.Empty;
    public IReadOnlyCollection<long> Imeis { get; init; } = new List<long>().AsReadOnly();
    public string MacAddress { get; init; } = string.Empty;
}