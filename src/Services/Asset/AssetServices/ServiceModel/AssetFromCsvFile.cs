namespace AssetServices.ServiceModel;

// ReSharper disable once ClassNeverInstantiated.Global
/// <summary>
/// Fields in csv file used to import assets.
/// </summary>
public record AssetFromCsvFile
{
    public string Brand { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string PurchaseDate { get; set; } = string.Empty;
    public string PurchaseType { get; set; } = string.Empty;
    public string Imei { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
    public string UserFirstName { get; set; } = string.Empty;
    public string UserLastName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string PurchasePrice { get; set; } = string.Empty;
}