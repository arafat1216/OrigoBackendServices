using System;

namespace Asset.API.ViewModels
{
    /// <summary>
    /// Request body for recycle asset lifecycle.
    /// </summary>
    public class RecycleAssetLifecycle
    {
        /// <summary>
        /// The int value of the asset lifecycle status.
        /// </summary>
        public int AssetLifecycleStatus { get; set; }
        /// <summary>
        /// Identification of the user makeing the request. 
        /// </summary>
        public Guid CallerId { get; set; }
    }
}
