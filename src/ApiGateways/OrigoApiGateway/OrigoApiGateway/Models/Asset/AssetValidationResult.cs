using System.Collections.Generic;

namespace OrigoApiGateway.Models.Asset;

public record AssetValidationResult
{
    public List<ImportedAsset> ValidAssets { get; set; } = new List<ImportedAsset>();
    public List<InvalidImportedAsset> InvalidAssets { get; set; } = new List<InvalidImportedAsset>();
}