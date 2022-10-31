namespace OrigoApiGateway.Models.Asset;

public record ImportedAsset
{
    public string SerialNumber { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Imeis { get; set; } = string.Empty;
    public ImportedUser ImportedUser { get; set; } = new();
    public DateTime PurchaseDate { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
    public Guid? MatchedUserId { get; set; }
    public string PurchasePrice { get; set; } = string.Empty;
}