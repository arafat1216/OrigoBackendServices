using Common.Enums;
using System;

namespace AssetServices.ServiceModel;

public record LabelDTO
{
    public LabelDTO(string text, LabelColor color, Guid externalId)
    {
        Text = text;
        Color = color;
        ExternalId = externalId;
    }

    public LabelDTO()
    {
    }

    public Guid ExternalId { get; init; }
    public string Text { get; init; }
    public LabelColor Color { get; init; }
}