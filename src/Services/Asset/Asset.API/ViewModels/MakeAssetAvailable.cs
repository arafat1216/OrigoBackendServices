using System;
using System.ComponentModel.DataAnnotations;

namespace Asset.API.ViewModels
{
    /// <summary>
    /// A Assets ID (Guid), that will be made available.
    /// </summary>
    public class MakeAssetAvailable
    {
        [Required]
        public Guid AssetLifeCycleId { get; set; }

        /// <summary>
        /// id of user making the endpoint call. Can be ignored by frontend.
        /// </summary>
        public Guid CallerId { get; set; }
    }
}
   
