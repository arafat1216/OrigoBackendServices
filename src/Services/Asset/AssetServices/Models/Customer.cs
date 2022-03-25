using System;
using Common.Seedwork;

namespace AssetServices.Models;

public class Customer : Entity
{
    public Guid ExternalId { get; init; }
}