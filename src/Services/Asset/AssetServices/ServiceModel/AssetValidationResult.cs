using System.Collections.Generic;

namespace AssetServices.ServiceModel;

public record AssetValidationResult
{
    public IList<ImportedAsset> ValidAssets { get; set; } = new List<ImportedAsset>();
    public IList<InvalidImportedAsset> InvalidAssets { get; set; } = new List<InvalidImportedAsset>();
}

public record InvalidImportedAsset : ImportedAsset
{
    public List<string> Errors { get; set; } = new();
}

public record ImportedUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

public record ImportedAsset
{
    public string SerialNumber { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Imeis { get; set; } = string.Empty;
    public ImportedUser ImportedUser { get; set; } = new();
}