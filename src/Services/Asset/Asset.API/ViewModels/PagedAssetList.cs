﻿using System.Collections.Generic;

namespace Asset.API.ViewModels
{
    public record PagedAssetList
    {
        public IList<Asset> Assets { get; set; }
        public int CurrentPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}