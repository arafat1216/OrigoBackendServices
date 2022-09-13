using System.Collections.Generic;

namespace OrigoApiGateway.Models.Asset;

public record InvalidImportedAsset : ImportedAsset
{
    public List<string> Errors { get; set; } = new();
}