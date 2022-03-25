using Common.Enums;

namespace AssetServices.ServiceModel;

public record LabelDTO
{
    public LabelDTO(string text, LabelColor color)
    {
        Text = text;
        Color = color;
    }

    public string Text { get; }
    public LabelColor Color { get; }
}