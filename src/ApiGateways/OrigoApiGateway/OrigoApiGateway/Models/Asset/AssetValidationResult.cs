using System.Collections.Generic;

namespace OrigoApiGateway.Models.Asset;

public record AssetValidationResult
{
    public IList<ImportedAsset> ValidAssets { get; set; } = new List<ImportedAsset>();
    public IList<InvalidImportedAsset> InvalidAssets { get; set; } = new List<InvalidImportedAsset>();
}