using Common.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Asset.API.ViewModels
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

        [FromQuery(Name = "userId")]
        public string UserId { get; set; }

        [FromQuery(Name = "isActiveState")]
        public bool? IsActiveState { get; set; }

        [FromQuery(Name = "isPersonal")]
        public bool? IsPersonal { get; set; }

        [FromQuery(Name = "endPeriodMonth")]
        public DateTime? EndPeriodMonth { get; set; }

        [FromQuery(Name = "purchaseMonth")]
        public DateTime? purchaseMonth { get; set; }

    }
}
