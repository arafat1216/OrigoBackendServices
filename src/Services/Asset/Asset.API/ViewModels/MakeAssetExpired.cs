using System;
using System.ComponentModel.DataAnnotations;

namespace Asset.API.ViewModels
{
    public class MakeAssetExpired
    {
        [Required]
        public Guid AssetLifeCycleId { get; set; }
        public Guid CallerId { get; set; }

    }
}
