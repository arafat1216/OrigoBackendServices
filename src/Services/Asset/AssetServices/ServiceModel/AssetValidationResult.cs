using System.Collections.Generic;

namespace AssetServices.ServiceModel;

public record AssetValidationResult
{
    public IList<object> ValidAssets { get; set; } = new List<object>();
    public IList<object> InvalidAssets { get; set; } = new List<object>();
}