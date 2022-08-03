namespace AssetServices.ServiceModel;

public record AssetFromCsvFile
{
    public string Category { get; set; }
    public string Brand { get; set; }
    public string ProductName { get; set; }
    public string PurchaseDate { get; set; }
    public string Imei { get; set; }
    public string MacAddress { get; set; }
    public string SerialNumber { get; set; }
    public string Alias { get; set; }
}