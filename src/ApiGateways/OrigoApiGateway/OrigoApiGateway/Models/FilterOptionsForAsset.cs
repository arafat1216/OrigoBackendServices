using Common.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    public class FilterOptionsForAsset
    {
        [FromQuery(Name = "managedByDepartmentId")]
        public IList<Guid?>? Department { get; set; }
        [FromQuery(Name = "assetStatus")]
        public IList<AssetLifecycleStatus>? Status { get; set; }
        [FromQuery(Name = "assetCategoryId")]
        public int[]? Category { get; set; }
        [FromQuery(Name = "labels")]
        public Guid[]? Label { get; set; }
        [FromQuery(Name = "userId")]
        public string UserId { get; set; }
        [FromQuery(Name = "isActiveState")]
        public bool? IsActiveState { get; set; }
        [FromQuery(Name = "isPersonal")]
        public bool? IsPersonal { get; set; }
        [FromQuery(Name = "endPeriodMonth")]
        public DateTime? EndPeriodMonth { get; set; }
        [FromQuery(Name = "role")]
        public string[]? Roles { get; set; }
    }
}
