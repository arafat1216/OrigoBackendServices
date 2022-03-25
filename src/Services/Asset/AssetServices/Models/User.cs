using System;
using Common.Seedwork;

namespace AssetServices.Models;

public class User : Entity
{
    public Guid ExternalId { get; init; }
    public string Name { get; set; } = string.Empty;
}