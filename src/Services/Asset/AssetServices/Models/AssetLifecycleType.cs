﻿namespace AssetServices.Models
{
    public class AssetLifecycleType
    {
        /// <summary>
        /// The display name for this asset lifecycle type
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The enum value of the lifecycle type.
        /// Use this when setting a lifecycle type for assets.
        /// </summary>
        public int EnumValue { get; set; }
    }
}
