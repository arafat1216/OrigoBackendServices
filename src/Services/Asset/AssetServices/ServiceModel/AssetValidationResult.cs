using System.Collections.Generic;

namespace AssetServices.ServiceModel;

public record AssetValidationResult
{
    public IList<ImportedAsset> ValidAssets { get; set; } = new List<ImportedAsset>();
    public IList<InvalidImportedAsset> InvalidAssets { get; set; } = new List<InvalidImportedAsset>();
}