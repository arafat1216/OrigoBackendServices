using Common.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    public class FilterOptionsForAsset
    {
        [FromQuery(Name = "department")]
        public IList<Guid?>? Department { get; set; }
        [FromQuery(Name = "status")]
        public IList<AssetLifecycleStatus>? Status { get; set; }
        [FromQuery(Name = "category")]
        public int[]? Category { get; set; }
        [FromQuery(Name = "label")]
        public Guid[]? Label { get; set; }
    }
}
