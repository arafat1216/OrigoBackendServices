using System.Collections.Generic;

namespace AssetServices.ServiceModel;

public record InvalidImportedAsset : ImportedAsset
{
    public List<string> Errors { get; set; } = new();
}