namespace AssetServices.ServiceModel;

public record AssetFromCsvFile
{
    public string Category { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string PurchaseDate { get; set; } = string.Empty;
    public string Imei { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
}