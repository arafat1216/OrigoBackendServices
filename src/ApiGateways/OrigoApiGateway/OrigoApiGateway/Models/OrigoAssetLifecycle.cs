namespace OrigoApiGateway.Services
{
    /// <summary>
    /// This is an object representation of a enum value
    /// </summary>
    public class OrigoAssetLifecycle
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