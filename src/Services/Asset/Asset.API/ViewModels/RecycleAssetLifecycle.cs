using System;
using System.ComponentModel.DataAnnotations;

namespace Asset.API.ViewModels
{
    /// <summary>
    /// Request body used to do the neccassery checks and changes relatated to a recycle service order made for the asset lifecycle.
    /// Completed request will end up changing asset lifecycle status to "Pending Recycle" or "Recycle".
    /// </summary>
    public class RecycleAssetLifecycle
    {
        /// <summary>
        /// The int value of the asset lifecycle status, statuses that could it could be changed to is either "PendingRecycle" or "Recycle".
        /// <see cref="Common.Enums.AssetLifecycleStatus"/>.
        /// </summary>
        [Required]
        public int AssetLifecycleStatus { get; set; }
        /// <summary>
        /// Identification of the user makeing the request. 
        /// </summary>
        [Required]
        public Guid CallerId { get; set; }
    }
}
