using System;

namespace AssetServices.Models;

public record TechstepProduct
{
    public string Description { get; init; } = string.Empty;
    public string TechstepPartNumber { get; init; } = string.Empty;
}